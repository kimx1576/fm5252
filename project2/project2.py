#Project 2 - CRR Binomial Model

import math

def up_multiplier (sigma, dt):
    return math.exp(sigma * math.sqrt(dt))

def down_multiplier(sigma, dt):
    return math.exp(-sigma * math.sqrt(dt))

def risk_neutral_prob(r, q, dt, u, d):
    return (math.exp((r-q)*dt) - d) / (u-d)

#test 
'''
signma = 0.2
dt = 1/252
up = up_multiplier(signma, dt)
down = down_multiplier(signma, dt)
print(risk_neutral_prob(0.05, 0.4, dt, up, down))
'''

#Stock price in any node(i,j)
def stock_prices (S0, u, d, i, j):
    return S0 * (u**j) * (d **(i-j))

#Payoff at tenor
def payoff(S_node, K, option_type):
    if option_type == "call":
        if S_node > K:
            return S_node - K
        else: 
            return 0.0
    else:
        if K > S_node:
            return K - S_node
        else:
            return 0.0

def option_value(S0, K, r, d, sigma, T, N, option_type, style, 
                 i, j, dt, u, dn, p, memo):
    
    if (i, j) in memo:
        return memo[(i, j)]
    
    #base case
    if i == N:
        s = stock_prices(S0, u, dn, i, j)
        val = payoff(s, K, option_type)
        memo[(i, j)] = val
        return val
    #recusive step
    v_up = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        i + 1, j + 1, dt, u, dn, p, memo)
    v_dn = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        i + 1, j, dt, u, dn, p, memo)
    #discounted expected value
    hold = math.exp(-r * dt) * (p * v_up + (1 - p) * v_dn)
    if style == "american":
        s = stock_prices(S0, u, dn, i, j)
        ex = payoff(s, K, option_type)
        if ex > hold:
            hold = ex
    memo[(i, j)] = hold
    return hold
# sets up tree and starts recursion at base node
def get_price(S0, K, r, d, sigma, T, N, option_type, style):
    dt = T/N
    u = up_multiplier(sigma, dt)
    dn = down_multiplier(sigma, dt)
    p = risk_neutral_prob(r, d, dt, u, dn)
    memo = {}
    return option_value (S0, K, r, d, sigma, T, N, option_type, style,
                         0, 0, dt, u, dn, p, memo)

#greeks by computing the tree. Delta, Gamma, Tehta from the tree nodes, and vega and rho use finite difference

#delta
def get_delta(S0, K, r, d, sigma, T, N, option_type, style):
    dt = T/N
    u = up_multiplier(sigma, dt)
    dn = down_multiplier(sigma, dt)
    p = risk_neutral_prob(r, d, dt, u, dn)
    
    m = {} #memo update
    f_up = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        1, 1, dt, u, dn, p, m)
    m = {}
    f_dn = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        1, 0, dt, u, dn, p, m)
    s_up = stock_prices(S0, u, dn, 1, 1)
    s_dn = stock_prices(S0, u, dn, 1, 0)
    delta = (f_up - f_dn) / (s_up - s_dn)
    return delta
#gamma
def get_gamma(S0, K, r, d, sigma, T, N, option_type, style):
    dt = T/N
    u = up_multiplier(sigma, dt)
    dn = down_multiplier(sigma, dt)
    p = risk_neutral_prob(r, d, dt, u, dn)

    m = {}
    f_up_up = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                           2, 2, dt, u, dn, p, m)
    m = {}
    f_up_dn = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                           2, 1, dt, u, dn, p, m)
    m = {}
    f_dn_dn = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                            2 ,0 ,dt ,u ,dn ,p ,m)
    
    s_up_up = stock_prices(S0,u,dn ,2 ,2)
    s_up_dn = stock_prices(S0,u,dn ,2 ,1)
    s_dn_dn = stock_prices(S0,u,dn ,2 ,0)

    d_up= (f_up_up - f_up_dn) / (s_up_up - s_up_dn)
    d_dn = (f_up_dn - f_dn_dn) / (s_up_dn - s_dn_dn)
    h = 0.5*(s_up_up - s_dn_dn)
    return (d_up - d_dn) / h
#theta
def get_theta(S0, K, r, d, sigma, T, N, option_type, style):
    dt = T/N
    u = up_multiplier(sigma, dt)
    dn = down_multiplier(sigma, dt)
    p = risk_neutral_prob(r, d, dt, u, dn)

    m = {}
    f_now = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        0, 0, dt, u, dn, p, m)
    m = {}
    f_mid = option_value(S0, K, r, d, sigma, T, N, option_type, style,
                        2 ,1 ,dt ,u ,dn ,p ,m)    
    return (f_mid - f_now)/(2*dt)
#vega
def get_vega(S0, K, r, d, sigma, T, N, option_type, style):
    bump = 0.01
    p_up = get_price(S0, K, r, d, sigma + bump, T, N, option_type, style)
    p_dn = get_price(S0, K, r, d, sigma - bump, T, N, option_type, style)
    return (p_up - p_dn) / (2 * bump)
#rho
def get_rho(S0, K, r, d, sigma, T, N, option_type, style):
    bump = 0.01
    p_up = get_price(S0, K, r + bump, d, sigma, T, N, option_type, style)
    p_dn = get_price(S0, K, r - bump, d, sigma, T, N, option_type, style)
    return (p_up - p_dn) / (2 * bump)

# output
def show_results(label, price, delta, gamma, theta, vega, rho):
    print("  " + label)
    print("    Price : " + str(round(price, 6)))
    print("    Delta : " + str(round(delta, 6)))
    print("    Gamma : " + str(round(gamma, 6)))
    print("    Theta : " + str(round(theta, 6)))
    print("    Vega  : " + str(round(vega, 6)))
    print("    Rho   : " + str(round(rho, 6)))
    print("")

def run_all(S0, K, r, d, sigma, T, N, otype, style):
    price = get_price(S0, K, r, d, sigma, T, N, otype, style)
    delta = get_delta(S0, K, r, d, sigma, T, N, otype, style)
    gamma = get_gamma(S0, K, r, d, sigma, T, N, otype, style)
    theta = get_theta(S0, K, r, d, sigma, T, N, otype, style)
    vega = get_vega(S0, K, r, d, sigma, T, N, otype, style)
    rho = get_rho(S0, K, r, d, sigma, T, N, otype, style)
    return price, delta, gamma, theta, vega, rho

def main():
    print("")

    S0 = float(input("Stock price (S0): "))
    K = float(input("Strike price (K): "))
    T = float(input("Time to expiration (yrs): "))
    r = float(input("Risk-free rate (ex: 0.05): "))
    d = float(input("Dividend yield (ex: 0.02): "))
    sigma = float(input("Volatility (ex: 0.30): "))
    N = int(input("Number of steps (N): "))

    print("")
    print("European Option")
    print("")
    p, dl, g, t, v, rh = run_all(S0, K, r, d, sigma, T, N, "call", "european")
    show_results("European Call", p, dl, g, t, v, rh)

    p, dl, g, t, v, rh = run_all(S0, K, r, d, sigma, T, N, "put", "european")
    show_results("European Put", p, dl, g, t, v, rh)

    print("American Option")
    print("")
    p, dl, g, t, v, rh = run_all(S0, K, r, d, sigma, T, N, "call", "american")
    show_results("American Call", p, dl, g, t, v, rh)

    p, dl, g, t, v, rh = run_all(S0, K, r, d, sigma, T, N, "put", "american")
    show_results("American Put", p, dl, g, t, v, rh)

main()