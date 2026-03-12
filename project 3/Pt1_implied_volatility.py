import numpy as np
from scipy.stats import norm

#Black-Scholes price of a European call or put
def black_scholes_price(S, K, T, r, sigma, option_type="call"):
    d1 = (np.log(S / K) + (r + 0.5 * sigma**2) * T) / (sigma * np.sqrt(T))
    d2 = d1 - sigma * np.sqrt(T)

    if option_type == "call":
        return S * norm.cdf(d1) - K * np.exp(-r * T) * norm.cdf(d2)
    elif option_type == "put":
        return K * np.exp(-r * T) * norm.cdf(-d2) - S * norm.cdf(-d1)
    else:
        raise ValueError("option_type must be 'call' or 'put'")

#Black-Scholes vega
def vega(S, K, T, r, sigma):
    d1 = (np.log(S / K) + (r + 0.5 * sigma**2) * T) / (sigma * np.sqrt(T))
    return S * np.sqrt(T) * norm.pdf(d1)

#Compute implied volatility using the bisection method
def implied_vol_bisection(S, K, T, r, market_price, option_type="call",
                          lo=1e-6, hi=5.0, tol=1e-8, max_iter=200):

    f_lo = black_scholes_price(S, K, T, r, lo, option_type) - market_price
    f_hi = black_scholes_price(S, K, T, r, hi, option_type) - market_price

    if f_lo * f_hi > 0:
        raise ValueError("Initial interval does not bracket a root.")

    for _ in range(max_iter):
        mid = 0.5 * (lo + hi)
        f_mid = black_scholes_price(S, K, T, r, mid, option_type) - market_price

        if abs(f_mid) < tol or (hi - lo) < tol:
            return mid

        if f_lo * f_mid < 0:
            hi = mid
        else:
            lo = mid
            f_lo = f_mid

    return 0.5 * (lo + hi)

#Compute implied volatility using Newton's method
def implied_vol_newton(S, K, T, r, market_price, option_type="call",
                       sig0=0.2, tol=1e-8, max_iter=100):

    sig = sig0

    for _ in range(max_iter):
        price = black_scholes_price(S, K, T, r, sig, option_type)
        diff = price - market_price

        if abs(diff) < tol:
            return sig

        v = vega(S, K, T, r, sig)
        if abs(v) < 1e-12:
            break

        sig = sig - diff / v

        if sig <= 0:
            sig = 0.5 * sig0

    return sig


if __name__ == "__main__":
    S = 100.0
    K = 100.0
    r = 0.05
    T = 1.0
    market_price = 10.4506
    option_type = "call"

    iv_bisect = implied_vol_bisection(S, K, T, r, market_price, option_type)
    iv_newton = implied_vol_newton(S, K, T, r, market_price, option_type)

    print("Implied Volatility Results")
    print(f"S = {S}, K = {K}, r = {r}, T = {T}")
    print(f"Option type: {option_type}")
    print(f"Market price: {market_price}")
    print(f"Bisection IV: {iv_bisect:.6f}")
    print(f"Newton IV:    {iv_newton:.6f}")

    check_price = black_scholes_price(S, K, T, r, iv_newton, option_type)
    print(f"Check price from Newton IV: {check_price:.4f}")

    print("\nPut option examples:")
    K2 = 105.0
    r2 = 0.03
    T2 = 0.5
    put_price = 8.20

    iv_put_b = implied_vol_bisection(S, K2, T2, r2, put_price, "put")
    iv_put_n = implied_vol_newton(S, K2, T2, r2, put_price, "put")

    print(f"Bisection IV: {iv_put_b:.6f}")
    print(f"Newton IV:    {iv_put_n:.6f}")
