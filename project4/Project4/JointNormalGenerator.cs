namespace Project4;

/// <summary>
/// Generates pairs (X, Y) of jointly normally distributed random variables
/// with a specified correlation ρ (rho) using the Cholesky decomposition of
/// the 2×2 correlation matrix:
///
///     Σ = [ 1   ρ ]
///         [ ρ   1 ]
///
/// Cholesky factorisation: Σ = L · Lᵀ  where
///     L = [ 1          0          ]
///         [ ρ   sqrt(1 - ρ²)  ]
///
/// Given two independent standard normals Z₁, Z₂:
///     X = Z₁
///     Y = ρ · Z₁ + sqrt(1 - ρ²) · Z₂
///
/// X and Y are each marginally N(0,1) and have correlation ρ.
/// </summary>
public static class JointNormalGenerator
{
    /// <summary>
    /// Generates <paramref name="count"/> pairs of jointly normal (X, Y)
    /// variates with correlation <paramref name="rho"/>.
    /// </summary>
    /// <param name="count">Number of (X, Y) pairs to generate.</param>
    /// <param name="rho">Desired correlation, must be in (-1, 1).</param>
    /// <param name="rng">Random number generator used for uniform draws.</param>
    /// <returns>
    /// A tuple of two double arrays (X, Y), each of length
    /// <paramref name="count"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="rho"/> is not in [-1, 1].
    /// </exception>
    public static (double[] X, double[] Y) Generate(int count, double rho, Random rng)
    {
        if (rho < -1.0 || rho > 1.0)
            throw new ArgumentOutOfRangeException(nameof(rho), "Correlation must be between -1 and 1.");

        // Cholesky coefficient: L[1,1] = sqrt(1 - ρ²).
        // Clamp to avoid tiny negative values from floating-point rounding.
        double cholFactor = Math.Sqrt(Math.Max(0.0, 1.0 - rho * rho));

        // Generate 2 × count independent standard normals using Box-Muller.
        double[] z1 = NormalGenerators.BoxMuller(count, rng);
        double[] z2 = NormalGenerators.BoxMuller(count, rng);

        double[] x = new double[count];
        double[] y = new double[count];

        for (int i = 0; i < count; i++)
        {
            x[i] = z1[i];
            y[i] = rho * z1[i] + cholFactor * z2[i];
        }

        return (x, y);
    }
}
