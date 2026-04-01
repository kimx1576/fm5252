namespace Project4;

/// <summary>
/// Provides basic descriptive statistics used to summarise samples generated
/// by the normal distribution methods.
/// </summary>
public static class StatisticsHelper
{
    /// <summary>
    /// Computes the arithmetic mean of <paramref name="data"/>.
    /// </summary>
    public static double Mean(double[] data)
    {
        if (data.Length == 0) return double.NaN;
        double sum = 0.0;
        foreach (double v in data) sum += v;
        return sum / data.Length;
    }

    /// <summary>
    /// Computes the sample standard deviation (divides by N-1) of
    /// <paramref name="data"/>.
    /// </summary>
    public static double StdDev(double[] data)
    {
        if (data.Length < 2) return double.NaN;
        double mean = Mean(data);
        double sumSq = 0.0;
        foreach (double v in data) sumSq += (v - mean) * (v - mean);
        return Math.Sqrt(sumSq / (data.Length - 1));
    }

    /// <summary>Returns the minimum value in <paramref name="data"/>.</summary>
    public static double Min(double[] data)
    {
        if (data.Length == 0) return double.NaN;
        double min = double.MaxValue;
        foreach (double v in data) if (v < min) min = v;
        return min;
    }

    /// <summary>Returns the maximum value in <paramref name="data"/>.</summary>
    public static double Max(double[] data)
    {
        if (data.Length == 0) return double.NaN;
        double max = double.MinValue;
        foreach (double v in data) if (v > max) max = v;
        return max;
    }

    /// <summary>
    /// Computes the Pearson sample correlation coefficient between two arrays
    /// of equal length.
    /// </summary>
    public static double Correlation(double[] x, double[] y)
    {
        if (x.Length != y.Length || x.Length < 2) return double.NaN;

        double meanX = Mean(x);
        double meanY = Mean(y);

        double num = 0.0, sumSqX = 0.0, sumSqY = 0.0;
        for (int i = 0; i < x.Length; i++)
        {
            double dx = x[i] - meanX;
            double dy = y[i] - meanY;
            num    += dx * dy;
            sumSqX += dx * dx;
            sumSqY += dy * dy;
        }

        double denom = Math.Sqrt(sumSqX * sumSqY);
        return denom == 0.0 ? double.NaN : num / denom;
    }

    /// <summary>
    /// Builds a histogram over <paramref name="data"/> with
    /// <paramref name="binCount"/> equal-width bins spanning the data range.
    /// Returns the bin left edges and counts.
    /// </summary>
    public static (double[] BinEdges, int[] Counts) Histogram(double[] data, int binCount = 30)
    {
        if (data.Length == 0 || binCount <= 0)
            return (Array.Empty<double>(), Array.Empty<int>());

        double min = Min(data);
        double max = Max(data);
        if (min == max)
        {
            // All values identical — put everything in one bin.
            return (new[] { min }, new[] { data.Length });
        }

        double width = (max - min) / binCount;
        double[] edges = new double[binCount];
        int[] counts   = new int[binCount];

        for (int b = 0; b < binCount; b++)
            edges[b] = min + b * width;

        foreach (double v in data)
        {
            int bin = (int)((v - min) / width);
            if (bin >= binCount) bin = binCount - 1; // clamp the max value
            counts[bin]++;
        }

        return (edges, counts);
    }
}
