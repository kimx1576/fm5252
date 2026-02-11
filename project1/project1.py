'''
Project 1 - Black-Scholes Option Pricing Model
'''

import math as math
import numpy as np
from scipy.stats import norm
import plotly.graph_objs as go
from plotly.offline import plot

# d1 and d2 calculations
d1_func = lambda s, k, r, sigma, T: (math.log(s/k) + (r + (sigma ** 2) / 2) * T) / (sigma * math.sqrt(T))
d2_func = lambda s, k, r, sigma, T: d1_func(s, k, r, sigma, T) - sigma * math.sqrt(T)

# Call and Put price functions
call_price = lambda s, k, r, sigma, T: s * norm.cdf(d1_func(s, k, r, sigma, T)) - k * math.exp(-r * T) * norm.cdf(d2_func(s, k, r, sigma, T))
put_price = lambda s, k, r, sigma, T: k * math.exp(-r * T) * norm.cdf(-d2_func(s, k, r, sigma, T)) - s * norm.cdf(-d1_func(s, k, r, sigma, T))

# Delta
call_delta = lambda s, k, r, sigma, T: norm.cdf(d1_func(s, k, r, sigma, T))
put_delta = lambda s, k, r, sigma, T: norm.cdf(d1_func(s, k, r, sigma, T)) - 1

# Gamma
gamma = lambda s, k, r, sigma, T: norm.pdf(d1_func(s, k, r, sigma, T)) / (s * sigma * math.sqrt(T))

# Vega
vega = lambda s, k, r, sigma, T: s * math.sqrt(T) * norm.pdf(d1_func(s, k, r, sigma, T))

# Theta
call_theta = lambda s, k, r, sigma, T: -(s * norm.pdf(d1_func(s, k, r, sigma, T)) * sigma) / (2 * math.sqrt(T)) - r * k * math.exp(-r*T) * norm.cdf(d2_func(s, k, r, sigma, T))
put_theta = lambda s, k, r, sigma, T: -(s * norm.pdf(d1_func(s, k, r, sigma, T)) * sigma) / (2 * math.sqrt(T)) + r * k * math.exp(-r*T) * norm.cdf(-d2_func(s, k, r, sigma, T))

# Rho
call_rho = lambda s, k, r, sigma, T: k * T * math.exp(-r*T) * norm.cdf(d2_func(s, k, r, sigma, T))
put_rho = lambda s, k, r, sigma, T: -k * T * math.exp(-r*T) * norm.cdf(-d2_func(s, k, r, sigma, T))


def black_shores_call(s,d_1,k,r,T,t,d_2):
  call = s * norm.cdf(d_1) - k * math.exp(-r * (T-t)) * norm.cdf(d_2)
  return call

def black_shores_put(k,r,T,t,d_2,d_1,s):
  put = k * math.exp(-r * (T-t)) * norm.cdf(-d_2) - s * norm.cdf(-d_1)
  return put

def get_d1(s,k,r,sigma, T, t):
  d1 = (math.log(s/k) + (r + (sigma ** 2) / 2) * T) / (sigma * math.sqrt(T-t))
  return d1

def get_d2(d_1, sigma, T, t):
  d2 = d_1 - sigma * math.sqrt(T-t)
  return d2

def get_delta_put(d1):
  # delta가 0.5라면, 기초자산이 1% 움직일 때, 옵션의 가격은 0.5% 변동한다는 의미
  # 콜 옵션 매수 포지션의 경우 0 ~ 1 사이의 값으로 나타냄, 내가격 (ITM)일수록 1에 가깝고
  # 외가격 (OTM)일수록 0에 가까운 델타값을 가지게 된다.
  # 내가격이란, 시장가격 > 행사가격 -> 권리 포기
  # 외가격이란, 시장가격 < 행사가격 -> 권리 사용
  # 행사 가능성이 높을수록 기초 자산 가격변동에 옵션 가격이 민감하게 움직이고
  # 행사 가능성이 낮을수록 기초 자산과 옵션 가격의 correlation이 낮다는 말이 됨
  return norm.cdf(d1) - 1

def get_delta_call(d1):
  return norm.cdf(d1)

def get_gamma(s,d_1,sigma,T,t):
  # 기초 자산 가격의 변동 대비 델타의 변동을 나타냄
  # 기초 자산의 가격과 델타와의 상관관계를 나타냄
  return (norm.pdf(d_1) / (s * sigma * math.sqrt(T-t)))

def get_setta_call(s,d_1,sigma,T,t,r,k,d_2):
  # 시간에 따른 옵션가격의 변동률을 나타냄
  # 블랙 숄즈 방정식을 t(시간)으로 편미분하여 세타 구함
  # t가 0에 가까울수록, 만기가 가까워질수록 세타는 가파르게 떨어짐
  setta_call = -(s * norm.pdf(d_1) * sigma) / (2 * math.sqrt(T-t)) - r * k * math.exp(-r*(T-t)*norm.cdf(d_2))
  return setta_call

def get_setta_put(s, d_1, sigma, T, t, r, k, d_2):
  setta_put = -(s * norm.pdf(d_1) * sigma) / (2 * math.sqrt(T-t)) + r * k * math.exp(-r*(T-t))*(norm.cdf(-d_2))
  return setta_put

def get_vega(s,T,t,d_1):
  # 기초 자산의 변동성의 변화 대비 옵션가격의 변동률을 나타냄
  # 베가는 변동성이 낮을수록 옵션가격의 민감도가 높음
  vega = s*math.sqrt(T-t)*(norm.pdf(d_1))
  return vega

def get_rho_call(k,T,r,t,d_2):
  # 이자율의 변동 대비 옵션 가격의 변동률을 나타내줌
  rho_call = k*T*math.exp(-r*(T-t))*norm.cdf(d_2)
  return rho_call

def get_rho_put(k,T,r,t,d_2):
  # 이자율의 변동은 자주 일어나는 일은 아니어서 실제 트레이딩에서는 거의 사용 x이지만,
  # 알아두는 것 중요
  rho_put = -k * T * math.exp(-r*(T-t))*norm.cdf(-d_2)
  return rho_put


if __name__ == "__main__":
    
    # Base parameters
    k = 150
    r = 0.045
    sigma = 0.27
    
    # For vectorized calculations
    S = np.linspace(80, 220, 110)
    
    # Plot 1: Call option prices with different maturities
    print("Generating plots...")
    data1 = []
    for i in range(2, 11, 2):
        T = i
        Z = np.array([call_price(s, k, r, sigma, T) for s in S])
        trace = go.Scatter(x=S, y=Z, name='Maturity=' + str(T))
        data1.append(trace)
    
    layout = go.Layout(width=900, height=500,
                       title='Call Option Price vs Spot Price',
                       xaxis=dict(title='Spot Price'),
                       yaxis=dict(title='Option Value'))
    fig1 = dict(data=data1, layout=layout)
    plot(fig1, filename='plot1_call_maturity.html')
    print("Plot 1 done")
    
    
    # Plot 2: Gamma with shorter maturities
    data2 = []
    for i in range(1, 11, 2):
        T = i / 10
        Z = np.array([gamma(s, k, r, sigma, T) for s in S])
        trace = go.Scatter(x=S, y=Z, name='Maturity=' + str(T))
        data2.append(trace)
    
    layout = go.Layout(width=900, height=500,
                       title='Gamma vs Spot Price',
                       xaxis=dict(title='Spot Price'),
                       yaxis=dict(title='Gamma'))
    fig2 = dict(data=data2, layout=layout)
    plot(fig2, filename='plot2_gamma.html')
    print("Plot 2 done")
    
    
    # Plot 3: Delta comparison between call and put
    T = 1.0
    call_d = np.array([call_delta(s, k, r, sigma, T) for s in S])
    put_d = np.array([put_delta(s, k, r, sigma, T) for s in S])
    
    trace1 = go.Scatter(x=S, y=call_d, name='Call Delta')
    trace2 = go.Scatter(x=S, y=put_d, name='Put Delta')
    
    layout = go.Layout(width=900, height=500,
                       title='Delta Comparison',
                       xaxis=dict(title='Spot Price'),
                       yaxis=dict(title='Delta'))
    fig3 = dict(data=[trace1, trace2], layout=layout)
    plot(fig3, filename='plot3_delta.html')
    print("Plot 3 done")
    
    
    # Plot 4: Vega across different volatilities
    sigma_range = np.linspace(0.1, 0.5, 100)
    s_val = 100
    
    data4 = []
    for mat in [0.5, 1.0, 1.5, 2.0]:
        vega_vals = np.array([vega(s_val, k, r, sig, mat) for sig in sigma_range])
        trace = go.Scatter(x=sigma_range*100, y=vega_vals, name=f'T={mat}')
        data4.append(trace)
    
    layout = go.Layout(width=900, height=500,
                       title='Vega vs Volatility',
                       xaxis=dict(title='Volatility (%)'),
                       yaxis=dict(title='Vega'))
    fig4 = dict(data=data4, layout=layout)
    plot(fig4, filename='plot4_vega.html')
    print("Plot 4 done")
    
    
    # Plot 5: Theta decay
    T_range = np.linspace(0.1, 2, 100)
    s_val = 100
    
    call_t = np.array([call_theta(s_val, k, r, sigma, t) for t in T_range])
    put_t = np.array([put_theta(s_val, k, r, sigma, t) for t in T_range])
    
    trace1 = go.Scatter(x=T_range, y=call_t, name='Call Theta')
    trace2 = go.Scatter(x=T_range, y=put_t, name='Put Theta')
    
    layout = go.Layout(width=900, height=500,
                       title='Theta vs Time to Maturity',
                       xaxis=dict(title='Time to Maturity'),
                       yaxis=dict(title='Theta'))
    fig5 = dict(data=[trace1, trace2], layout=layout)
    plot(fig5, filename='plot5_theta.html')
    print("Plot 5 done")
    
    
    # Sample calculations
    print("\n" + "="*60)
    print("Sample calculations:")
    s_test = 100
    T_test = 1.0
    print(f"S={s_test}, K={k}, T={T_test}, r={r}, sigma={sigma}")
    print("-"*60)
    print(f"Call Price: {call_price(s_test, k, r, sigma, T_test):.4f}")
    print(f"Put Price: {put_price(s_test, k, r, sigma, T_test):.4f}")
    print(f"Call Delta: {call_delta(s_test, k, r, sigma, T_test):.4f}")
    print(f"Put Delta: {put_delta(s_test, k, r, sigma, T_test):.4f}")
    print(f"Gamma: {gamma(s_test, k, r, sigma, T_test):.4f}")
    print(f"Vega: {vega(s_test, k, r, sigma, T_test):.4f}")
    print(f"Call Theta: {call_theta(s_test, k, r, sigma, T_test):.4f}")
    print(f"Put Theta: {put_theta(s_test, k, r, sigma, T_test):.4f}")
    print(f"Call Rho: {call_rho(s_test, k, r, sigma, T_test):.4f}")
    print(f"Put Rho: {put_rho(s_test, k, r, sigma, T_test):.4f}")
    print("="*60)
    
    print("\nAll plots generated successfully!")