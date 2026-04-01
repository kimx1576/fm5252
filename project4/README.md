# Project 4 — Normal Distribution Random Value Generator

A **C# Windows Forms** application (targeting .NET 8) that generates normally
distributed random values using three classic algorithms and produces pairs of
jointly (bivariately) normally distributed values for any user-specified
correlation ρ ∈ (−1, 1).

---

## What the Application Does

### Three Standard-Normal Generation Methods

| # | Method | Description |
|---|--------|-------------|
| 1 | **Box-Muller Transform** | Uses two U(0,1) uniforms and the trigonometric form to produce two independent N(0,1) values. |
| 2 | **Polar (Marsaglia) Method** | Rejection-sampling variant of Box-Muller that avoids trigonometric functions; accepts ~78.5% of candidate pairs. |
| 3 | **Inverse Transform (Acklam)** | Inverts the standard normal CDF Φ using the Acklam rational approximation (accurate to ~9 decimal places). No library functions used. |

Each method is implemented from scratch in `NormalGenerators.cs`.

### Joint (Bivariate) Normal Generator

Given a user-specified correlation ρ ∈ (−1, 1), pairs (X, Y) are generated
using the **Cholesky decomposition** of the 2×2 correlation matrix:

```
X = Z₁
Y = ρ · Z₁ + √(1 − ρ²) · Z₂
```

where Z₁, Z₂ are independent standard normals.

---

## How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows required for WinForms)
- Windows OS (WinForms is Windows-only)

### Build and Run

```bash
cd project4
dotnet run --project Project4/Project4.csproj
```

Or open `Project4.sln` in **Visual Studio 2022** (or later) and press F5.

---

## User Interface

| Control | Purpose |
|---------|---------|
| **Number of Samples** | How many random values to generate (default 10,000) |
| **Correlation ρ** | Target correlation for the bivariate pair (default 0.7, range −1 to 1) |
| **Mean μ** | Desired mean of generated values (default 0) |
| **Std Dev σ** | Desired standard deviation (default 1) |
| **Random Seed** | Optional integer seed for reproducible results (leave blank for random) |
| **Generate** | Runs all three methods and the joint generator |
| **Summary Grid** | Shows mean, std dev, min, max for each method |
| **Joint Results** | Shows input ρ, sample correlation, and X/Y summary stats |
| **Histogram** | Drop-down selects which method's histogram to display |

---

## Project Structure

```
project4/
├── Project4.sln
├── README.md
└── Project4/
    ├── Project4.csproj          (.NET 8 WinForms project)
    ├── Program.cs               Entry point
    ├── MainForm.cs              UI event logic
    ├── MainForm.Designer.cs     Designer-generated layout code
    ├── NormalGenerators.cs      Box-Muller, Polar, Inverse Transform methods
    ├── JointNormalGenerator.cs  Bivariate normal via Cholesky decomposition
    └── StatisticsHelper.cs      Mean, std dev, min, max, correlation, histogram
```

---

## Extending This Code

The generation logic is fully separated from the UI:

- To add a new generation method, add a static method to `NormalGenerators.cs`
  and call it from `MainForm.cs`.
- To add more statistics (e.g. skewness, kurtosis), add methods to
  `StatisticsHelper.cs`.
- To display scatter plots, add a new `Panel` with a `Paint` handler in
  `MainForm.cs`/`MainForm.Designer.cs`.
- To support multi-variate normals (dimension > 2), extend
  `JointNormalGenerator.cs` with a full Cholesky factorisation of an n×n
  correlation matrix.
