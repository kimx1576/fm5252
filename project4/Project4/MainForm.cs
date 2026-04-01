namespace Project4;

/// <summary>
/// Main application window.
///
/// Allows the user to:
///   • Specify the number of samples, correlation ρ, mean μ and std-dev σ.
///   • Click "Generate" to run all three normal-generation methods and the
///     joint bivariate normal generator.
///   • View per-method summary statistics in a DataGridView.
///   • View the bivariate results (sample correlation + X/Y stats).
///   • See a simple histogram drawn with GDI+ for the currently-selected method.
/// </summary>
public partial class MainForm : Form
{
    // -------------------------------------------------------------------------
    // Cached results — populated on each Generate click
    // -------------------------------------------------------------------------
    private double[]? _boxMullerSamples;
    private double[]? _polarSamples;
    private double[]? _inverseSamples;
    private double[]? _jointX;
    private double[]? _jointY;

    private Random _rng = new Random();

    public MainForm()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Generate button handler
    // -------------------------------------------------------------------------
    private void btnGenerate_Click(object sender, EventArgs e)
    {
        // --- Validate inputs --------------------------------------------------
        if (!int.TryParse(txtSamples.Text, out int n) || n <= 0)
        {
            MessageBox.Show("Please enter a positive integer for the number of samples.",
                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!double.TryParse(txtRho.Text, out double rho) || rho < -1.0 || rho > 1.0)
        {
            MessageBox.Show("Please enter a correlation ρ between −1 and 1.",
                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!double.TryParse(txtMean.Text, out double mu))
        {
            MessageBox.Show("Please enter a valid number for the mean μ.",
                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!double.TryParse(txtSigma.Text, out double sigma) || sigma <= 0)
        {
            MessageBox.Show("Please enter a positive number for the std-dev σ.",
                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Optionally seed for reproducibility when a seed is provided.
        if (int.TryParse(txtSeed.Text, out int seed))
            _rng = new Random(seed);
        else
            _rng = new Random();

        // --- Generate ---------------------------------------------------------
        try
        {
            // Generate standard normal samples with each method, then scale.
            _boxMullerSamples = Scale(NormalGenerators.BoxMuller(n, _rng), mu, sigma);
            _polarSamples     = Scale(NormalGenerators.Polar(n, _rng),     mu, sigma);
            _inverseSamples   = Scale(NormalGenerators.InverseTransform(n, _rng), mu, sigma);

            // Joint bivariate normal (standard, then scale independently).
            var (rawX, rawY) = JointNormalGenerator.Generate(n, rho, _rng);
            _jointX = Scale(rawX, mu, sigma);
            _jointY = Scale(rawY, mu, sigma);

            // --- Populate the summary grid ------------------------------------
            PopulateSummaryGrid();

            // --- Display joint results ----------------------------------------
            DisplayJointResults(rho);

            // --- Redraw histogram ---------------------------------------------
            histogramPanel.Invalidate();

            lblStatus.Text = $"Generated {n:N0} samples successfully.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred during generation:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Scales standard normal samples to N(μ, σ²).</summary>
    private static double[] Scale(double[] z, double mu, double sigma)
    {
        double[] result = new double[z.Length];
        for (int i = 0; i < z.Length; i++)
            result[i] = mu + sigma * z[i];
        return result;
    }

    // -------------------------------------------------------------------------
    // Summary statistics grid
    // -------------------------------------------------------------------------
    private void PopulateSummaryGrid()
    {
        dgvSummary.Rows.Clear();

        AddSummaryRow("Box-Muller",       _boxMullerSamples!);
        AddSummaryRow("Polar (Marsaglia)", _polarSamples!);
        AddSummaryRow("Inverse Transform", _inverseSamples!);
    }

    private void AddSummaryRow(string method, double[] data)
    {
        dgvSummary.Rows.Add(
            method,
            StatisticsHelper.Mean(data).ToString("F6"),
            StatisticsHelper.StdDev(data).ToString("F6"),
            StatisticsHelper.Min(data).ToString("F6"),
            StatisticsHelper.Max(data).ToString("F6")
        );
    }

    // -------------------------------------------------------------------------
    // Joint distribution results
    // -------------------------------------------------------------------------
    private void DisplayJointResults(double rhoInput)
    {
        double sampleRho = StatisticsHelper.Correlation(_jointX!, _jointY!);
        txtJointResults.Text =
            $"=== Joint (Bivariate) Normal Results ==={Environment.NewLine}" +
            $"Input ρ:         {rhoInput:F4}{Environment.NewLine}" +
            $"Sample ρ(X,Y):   {sampleRho:F6}{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"--- X statistics ---{Environment.NewLine}" +
            $"  Mean:    {StatisticsHelper.Mean(_jointX!):F6}{Environment.NewLine}" +
            $"  Std Dev: {StatisticsHelper.StdDev(_jointX!):F6}{Environment.NewLine}" +
            $"  Min:     {StatisticsHelper.Min(_jointX!):F6}{Environment.NewLine}" +
            $"  Max:     {StatisticsHelper.Max(_jointX!):F6}{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"--- Y statistics ---{Environment.NewLine}" +
            $"  Mean:    {StatisticsHelper.Mean(_jointY!):F6}{Environment.NewLine}" +
            $"  Std Dev: {StatisticsHelper.StdDev(_jointY!):F6}{Environment.NewLine}" +
            $"  Min:     {StatisticsHelper.Min(_jointY!):F6}{Environment.NewLine}" +
            $"  Max:     {StatisticsHelper.Max(_jointY!):F6}";
    }

    // -------------------------------------------------------------------------
    // Histogram painting (GDI+)
    // -------------------------------------------------------------------------
    private void histogramPanel_Paint(object sender, PaintEventArgs e)
    {
        double[]? data = cmbHistMethod.SelectedIndex switch
        {
            0 => _boxMullerSamples,
            1 => _polarSamples,
            2 => _inverseSamples,
            _ => null
        };

        if (data == null || data.Length == 0) return;

        Graphics g      = e.Graphics;
        Rectangle rect  = histogramPanel.ClientRectangle;
        int margin      = 40;
        int plotWidth   = rect.Width  - 2 * margin;
        int plotHeight  = rect.Height - 2 * margin;

        if (plotWidth <= 0 || plotHeight <= 0) return;

        var (edges, counts) = StatisticsHelper.Histogram(data, 40);
        int maxCount = 0;
        foreach (int c in counts) if (c > maxCount) maxCount = c;
        if (maxCount == 0) return;

        int binCount  = counts.Length;
        float binW    = (float)plotWidth / binCount;

        // Draw axes
        using var axisPen = new Pen(Color.Black, 1.5f);
        g.DrawLine(axisPen, margin, margin, margin, margin + plotHeight);           // Y axis
        g.DrawLine(axisPen, margin, margin + plotHeight, margin + plotWidth, margin + plotHeight); // X axis

        // Draw bars
        using var brush = new SolidBrush(Color.SteelBlue);
        using var outlinePen = new Pen(Color.DarkBlue, 0.5f);
        for (int i = 0; i < binCount; i++)
        {
            float barH  = (float)counts[i] / maxCount * plotHeight;
            float x     = margin + i * binW;
            float y     = margin + plotHeight - barH;
            g.FillRectangle(brush, x, y, binW - 1, barH);
            g.DrawRectangle(outlinePen, x, y, binW - 1, barH);
        }

        // Axis labels
        using var font  = new Font("Arial", 8);
        using var brush2 = new SolidBrush(Color.Black);

        // X-axis: show left edge, middle, and right edge values
        string leftLabel  = edges[0].ToString("F2");
        string midLabel   = edges[binCount / 2].ToString("F2");
        string rightLabel = (edges[binCount - 1] + (edges[1] - edges[0])).ToString("F2");

        g.DrawString(leftLabel,  font, brush2, margin - 5, margin + plotHeight + 5);
        g.DrawString(midLabel,   font, brush2, margin + plotWidth / 2 - 10, margin + plotHeight + 5);
        g.DrawString(rightLabel, font, brush2, margin + plotWidth - 15, margin + plotHeight + 5);

        // Title
        string methodName = cmbHistMethod.Text;
        using var titleFont = new Font("Arial", 9, FontStyle.Bold);
        g.DrawString($"Histogram — {methodName}", titleFont, brush2, margin, 5);
    }

    private void cmbHistMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        histogramPanel.Invalidate();
    }
}
