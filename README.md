# FM 5252 - Financial Mathematics

This repository contains coursework and assignments for FM 5252.

## Course Information
- **Course:** FM 5252 - Financial Mathematics
- **Semester:** Spring 2026

## Repository Contents

This repo includes homework assignments and projects related to financial modeling and quantitative analysis.

### Project 1 - Black-Scholes Option Pricing Model

Implementation of the Black-Scholes model for European options pricing with Greeks calculations.

**Features:**
- Option pricing functions (Call & Put)
- Greeks calculations (Delta, Gamma, Vega, Theta, Rho)
- Interactive visualizations using Plotly
- Supports both scalar and vector inputs

### Project 2 - CRR Binomial Option Pricing Calculator

Prices American and European call and put options using the Cox-Ross-Rubinstein (CRR) binomial tree model. Calculates Greeks: Delta, Gamma, Theta, Vega, and Rho. Built with recursion only — no loops.

**Inputs**
- Stock price (S0)
- Strike price (K)
- Time to expiration in years (T)
- Risk-free rate (r)
- Dividend yield (d)
- Volatility (sigma)
- Number of steps (N)

**Example**
```
Stock price (S0): 100
Strike price (K): 100
Time to expiration (yrs): 1
Risk-free rate (ex: 0.05): 0.05
Dividend yield (ex: 0.02): 0.0
Volatility (ex: 0.30): 0.2
Number of steps (N): 100
```

**Methods**
- **Pricing:** Recursive backward induction through the binomial tree with memoization
- **Delta:** From option values at nodes (1,1) and (1,0)
- **Gamma:** From option values at nodes (2,2), (2,1), and (2,0)
- **Theta:** From option values at nodes (0,0) and (2,1)
- **Vega:** Central finite difference on sigma
- **Rho:** Central finite difference on r

### Project 3 - Implied Volatility & SVI Skew Fit


**Part 1 — Implied Volatility Finder**

Finds implied volatility from a given option price using two methods:
- Bisection (binary search over sigma)
- Newton-Raphson (uses vega as the derivative)

Both methods solve for the sigma that makes the Black-Scholes price match the observed market price.

Output shows the implied vol from both methods for a call and a put example, plus a verification check.

**Part 2 — Gatheral SVI Skew Fit**

Fits the SVI (Stochastic Volatility Inspired) model to a set of implied volatility points across different strikes. The SVI formula is:

```
w(x) = a + b * [rho * (x - m) + sqrt((x - m)^2 + sigma^2)]
```

where x = log(K/F) and w = IV^2 * T.

The script finds the five parameters (a, b, rho, m, sigma) that best fit the data using least-squares optimization, then plots the result.



