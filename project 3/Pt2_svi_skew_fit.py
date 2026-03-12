import numpy as np
from scipy.optimize import minimize
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt

#SVI total variance formula
def svi_total_variance(x, a, b, rho, m, sigma):
    x = np.asarray(x)
    return a + b * (rho * (x - m) + np.sqrt((x - m)**2 + sigma**2))

#Sum of squared errors
def svi_objective(params, xs, ws):
    a, b, rho, m, sigma = params
    w_model = svi_total_variance(xs, a, b, rho, m, sigma)
    return np.sum((w_model - ws)**2)

#SVI parameters to total variance data
def fit_svi(xs, ws):
    x0 = [np.min(ws), 0.2, -0.4, 0.0, 0.1]

    bnds = [
        (-1.0, 1.0),
        (1e-6, 5.0),
        (-0.999, 0.999),
        (-2.0, 2.0),
        (1e-6, 2.0),
    ]

    res = minimize(
        svi_objective,
        x0,
        args=(xs, ws),
        method="L-BFGS-B",
        bounds=bnds
    )

    return res


if __name__ == "__main__":
    S = 100.0
    r = 0.02
    T = 0.5
    F = S * np.exp(r * T)

    #Non flat skew data
    strikes = np.array([80, 85, 90, 95, 100, 105, 110, 115, 120])
    ivs = np.array([0.38, 0.33, 0.28, 0.25, 0.22, 0.215, 0.22, 0.235, 0.26])

    xs = np.log(strikes / F)
    ws = ivs**2 * T

    res = fit_svi(xs, ws)
    a, b, rho, m, sigma = res.x

    print("SVI Skew Fit Results")
    print(f"converged: {res.success}")
    print(f"SSE:       {res.fun:.2e}")
    print()
    print(f"a     = {a:.6f}")
    print(f"b     = {b:.6f}")
    print(f"rho   = {rho:.6f}")
    print(f"m     = {m:.6f}")
    print(f"sigma = {sigma:.6f}")

    x_fine = np.linspace(xs.min() - 0.05, xs.max() + 0.05, 200)
    w_fine = svi_total_variance(x_fine, a, b, rho, m, sigma)
    iv_fine = np.sqrt(w_fine / T)

    plt.figure(figsize=(8, 5))
    plt.scatter(xs, ivs * 100, zorder=5, label="Market IV")
    plt.plot(x_fine, iv_fine * 100, label="SVI fit")
    plt.xlabel("Log-moneyness ln(K/F)")
    plt.ylabel("Implied Volatility (%)")
    plt.title("SVI Volatility Skew Fit")
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig("svi_fit_plot.png", dpi=150)
    print("\nPlot saved to svi_fit_plot.png")
