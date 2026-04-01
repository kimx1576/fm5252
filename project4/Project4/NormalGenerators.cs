namespace Project4;

/// <summary>
/// Provides three methods for generating standard normal N(0,1) random variates
/// from uniform U(0,1) samples. All methods are implemented from scratch without
/// any built-in library inverse-normal functions.
///
/// Methods implemented:
///   1. Box-Muller Transform
///   2. Polar (Marsaglia) Method
///   3. Inverse Transform Method (Acklam rational approximation)
/// </summary>
public static class NormalGenerators
{
    // -------------------------------------------------------------------------
    // 1. Box-Muller Transform
    // -------------------------------------------------------------------------
    // Given two independent U(0,1) values U1 and U2:
    //   Z0 = sqrt(-2 * ln(U1)) * cos(2π * U2)
    //   Z1 = sqrt(-2 * ln(U1)) * sin(2π * U2)
    // Both Z0 and Z1 are independent standard normal.
    // -------------------------------------------------------------------------

    /// <summary>
    /// Generates <paramref name="count"/> standard normal samples using the
    /// Box-Muller transform.
    /// </summary>
    public static double[] BoxMuller(int count, Random rng)
    {
        double[] result = new double[count];
        int i = 0;

        while (i < count)
        {
            // Draw two independent uniforms, avoiding U1=0 to prevent log(0).
            double u1 = DrawNonZeroUniform(rng);
            double u2 = rng.NextDouble();

            double magnitude = Math.Sqrt(-2.0 * Math.Log(u1));

            double z0 = magnitude * Math.Cos(2.0 * Math.PI * u2);
            result[i++] = z0;

            // Reuse the second variate if we still need more samples.
            if (i < count)
            {
                double z1 = magnitude * Math.Sin(2.0 * Math.PI * u2);
                result[i++] = z1;
            }
        }

        return result;
    }

    // -------------------------------------------------------------------------
    // 2. Polar (Marsaglia) Method
    // -------------------------------------------------------------------------
    // Generate U1, U2 uniform on (-1, 1). Compute S = U1^2 + U2^2.
    // Reject if S >= 1 or S = 0 (to stay inside the unit circle and avoid
    // log(0)).  Otherwise:
    //   Z0 = U1 * sqrt(-2 * ln(S) / S)
    //   Z1 = U2 * sqrt(-2 * ln(S) / S)
    // Both Z0 and Z1 are independent standard normal.
    // Expected acceptance rate is π/4 ≈ 78.5%.
    // -------------------------------------------------------------------------

    /// <summary>
    /// Generates <paramref name="count"/> standard normal samples using the
    /// Polar (Marsaglia) method.
    /// </summary>
    public static double[] Polar(int count, Random rng)
    {
        double[] result = new double[count];
        int i = 0;

        while (i < count)
        {
            double u1, u2, s;

            // Rejection loop — keep drawing until the point falls strictly
            // inside the unit disk.
            do
            {
                u1 = rng.NextDouble() * 2.0 - 1.0; // uniform on (-1, 1)
                u2 = rng.NextDouble() * 2.0 - 1.0;
                s = u1 * u1 + u2 * u2;
            }
            while (s >= 1.0 || s == 0.0);

            double factor = Math.Sqrt(-2.0 * Math.Log(s) / s);

            result[i++] = u1 * factor;

            if (i < count)
                result[i++] = u2 * factor;
        }

        return result;
    }

    // -------------------------------------------------------------------------
    // 3. Inverse Transform Method — Rational approximation of Φ⁻¹(u)
    // -------------------------------------------------------------------------
    // Given U ~ U(0,1), compute Z = Φ⁻¹(U) where Φ is the standard normal CDF.
    //
    // We use the rational approximation attributed to Peter Acklam
    // (sometimes called the Beasley-Springer-Moro family of approximations).
    // The algorithm is accurate to approximately 9 significant decimal places
    // for p in (0, 1).
    //
    // Reference: Peter J. Acklam, "An algorithm for computing the inverse normal
    // cumulative distribution function," 2003.
    // https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/
    //
    // The algorithm uses three regions:
    //   Low tail:    p in (0, p_low)          — rational approx on log scale
    //   Central:     p in [p_low, p_high]     — rational approximation
    //   High tail:   p in (p_high, 1)         — symmetry: Φ⁻¹(p) = -Φ⁻¹(1-p)
    // -------------------------------------------------------------------------

    // Coefficients for the rational approximation (Acklam 2003).
    // Central region numerator / denominator coefficients.
    private static readonly double[] A =
    {
        -3.969683028665376e+01,  2.209460984245205e+02,
        -2.759285104469687e+02,  1.383577518672690e+02,
        -3.066479806614716e+01,  2.506628277459239e+00
    };

    private static readonly double[] B =
    {
        -5.447609879822406e+01,  1.615858368580409e+02,
        -1.556989798598866e+02,  6.680131188771972e+01,
        -1.328068155288572e+01
    };

    // Tail region numerator / denominator coefficients.
    private static readonly double[] C =
    {
        -7.784894002430293e-03, -3.223964580411365e-01,
        -2.400758277161838e+00, -2.549732539343734e+00,
         4.374664141464968e+00,  2.938163982698783e+00
    };

    private static readonly double[] D =
    {
         7.784695709041462e-03,  3.224671290700398e-01,
         2.445134137142996e+00,  3.754408661907416e+00
    };

    // Break-points separating tail from central region.
    private const double P_LOW  = 0.02425;
    private const double P_HIGH = 1.0 - P_LOW;

    /// <summary>
    /// Generates <paramref name="count"/> standard normal samples using the
    /// Inverse Transform method (Acklam rational approximation of Φ⁻¹).
    /// </summary>
    public static double[] InverseTransform(int count, Random rng)
    {
        double[] result = new double[count];
        for (int i = 0; i < count; i++)
        {
            double u = DrawNonZeroUniformOpenInterval(rng);
            result[i] = NormalQuantile(u);
        }
        return result;
    }

    /// <summary>
    /// Computes Φ⁻¹(p) — the standard normal quantile function — using the
    /// Acklam rational approximation. Accurate to ~9 decimal places.
    /// </summary>
    public static double NormalQuantile(double p)
    {
        if (p <= 0.0) return double.NegativeInfinity;
        if (p >= 1.0) return double.PositiveInfinity;

        double q, r, x;

        if (p < P_LOW)
        {
            // Rational approximation for the lower tail.
            q = Math.Sqrt(-2.0 * Math.Log(p));
            x = (((((C[0] * q + C[1]) * q + C[2]) * q + C[3]) * q + C[4]) * q + C[5]) /
                ((((D[0] * q + D[1]) * q + D[2]) * q + D[3]) * q + 1.0);
        }
        else if (p <= P_HIGH)
        {
            // Rational approximation for the central region.
            q = p - 0.5;
            r = q * q;
            x = (((((A[0] * r + A[1]) * r + A[2]) * r + A[3]) * r + A[4]) * r + A[5]) * q /
                (((((B[0] * r + B[1]) * r + B[2]) * r + B[3]) * r + B[4]) * r + 1.0);
        }
        else
        {
            // Rational approximation for the upper tail (use symmetry).
            q = Math.Sqrt(-2.0 * Math.Log(1.0 - p));
            x = -(((((C[0] * q + C[1]) * q + C[2]) * q + C[3]) * q + C[4]) * q + C[5]) /
                 ((((D[0] * q + D[1]) * q + D[2]) * q + D[3]) * q + 1.0);
        }

        return x;
    }

    // -------------------------------------------------------------------------
    // Helper utilities
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns a uniform draw from (0, 1) — strictly positive so that
    /// Math.Log() is safe.
    /// </summary>
    private static double DrawNonZeroUniform(Random rng)
    {
        double u;
        do { u = rng.NextDouble(); } while (u == 0.0);
        return u;
    }

    /// <summary>
    /// Returns a uniform draw from the open interval (0, 1) — neither 0 nor 1.
    /// </summary>
    private static double DrawNonZeroUniformOpenInterval(Random rng)
    {
        double u;
        do { u = rng.NextDouble(); } while (u == 0.0 || u == 1.0);
        return u;
    }
}
