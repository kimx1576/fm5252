namespace Project4;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support — do not modify the contents
    /// of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // -----------------------------------------------------------------------
        // Controls declared here
        // -----------------------------------------------------------------------
        grpInputs        = new GroupBox();
        lblSamples       = new Label();
        txtSamples       = new TextBox();
        lblRho           = new Label();
        txtRho           = new TextBox();
        lblMean          = new Label();
        txtMean          = new TextBox();
        lblSigma         = new Label();
        txtSigma         = new TextBox();
        lblSeed          = new Label();
        txtSeed          = new TextBox();
        btnGenerate      = new Button();

        grpSummary       = new GroupBox();
        dgvSummary       = new DataGridView();
        colMethod        = new DataGridViewTextBoxColumn();
        colMean          = new DataGridViewTextBoxColumn();
        colStdDev        = new DataGridViewTextBoxColumn();
        colMin           = new DataGridViewTextBoxColumn();
        colMax           = new DataGridViewTextBoxColumn();

        grpJoint         = new GroupBox();
        txtJointResults  = new TextBox();

        grpHistogram     = new GroupBox();
        lblHistMethod    = new Label();
        cmbHistMethod    = new ComboBox();
        histogramPanel   = new Panel();

        lblStatus        = new Label();

        // -----------------------------------------------------------------------
        // grpInputs
        // -----------------------------------------------------------------------
        grpInputs.Text     = "Input Parameters";
        grpInputs.Location = new Point(12, 12);
        grpInputs.Size     = new Size(380, 160);

        int row = 22, col1 = 10, col2 = 160, rowH = 30;

        lblSamples.Text     = "Number of Samples:";
        lblSamples.Location = new Point(col1, row);
        lblSamples.Size     = new Size(140, 20);
        txtSamples.Text     = "10000";
        txtSamples.Location = new Point(col2, row);
        txtSamples.Size     = new Size(100, 23);

        row += rowH;
        lblRho.Text     = "Correlation ρ (−1 to 1):";
        lblRho.Location = new Point(col1, row);
        lblRho.Size     = new Size(145, 20);
        txtRho.Text     = "0.7";
        txtRho.Location = new Point(col2, row);
        txtRho.Size     = new Size(100, 23);

        row += rowH;
        lblMean.Text     = "Mean μ:";
        lblMean.Location = new Point(col1, row);
        lblMean.Size     = new Size(80, 20);
        txtMean.Text     = "0";
        txtMean.Location = new Point(col2, row);
        txtMean.Size     = new Size(100, 23);

        row += rowH;
        lblSigma.Text     = "Std Dev σ:";
        lblSigma.Location = new Point(col1, row);
        lblSigma.Size     = new Size(80, 20);
        txtSigma.Text     = "1";
        txtSigma.Location = new Point(col2, row);
        txtSigma.Size     = new Size(100, 23);

        row += rowH;
        lblSeed.Text     = "Random Seed (optional):";
        lblSeed.Location = new Point(col1, row);
        lblSeed.Size     = new Size(145, 20);
        txtSeed.Text     = "";
        txtSeed.Location = new Point(col2, row);
        txtSeed.Size     = new Size(100, 23);

        grpInputs.Controls.AddRange(new Control[]
        {
            lblSamples, txtSamples,
            lblRho, txtRho,
            lblMean, txtMean,
            lblSigma, txtSigma,
            lblSeed, txtSeed
        });

        // -----------------------------------------------------------------------
        // Generate button
        // -----------------------------------------------------------------------
        btnGenerate.Text     = "Generate";
        btnGenerate.Location = new Point(400, 40);
        btnGenerate.Size     = new Size(110, 35);
        btnGenerate.Font     = new Font("Arial", 10, FontStyle.Bold);
        btnGenerate.BackColor = Color.SteelBlue;
        btnGenerate.ForeColor = Color.White;
        btnGenerate.FlatStyle = FlatStyle.Flat;
        btnGenerate.Click    += btnGenerate_Click;

        // -----------------------------------------------------------------------
        // grpSummary — DataGridView with stats for the three methods
        // -----------------------------------------------------------------------
        grpSummary.Text     = "Method Summary Statistics";
        grpSummary.Location = new Point(12, 185);
        grpSummary.Size     = new Size(760, 140);

        colMethod.HeaderText = "Method";
        colMethod.Width      = 180;
        colMean.HeaderText   = "Mean";
        colMean.Width        = 120;
        colStdDev.HeaderText = "Std Dev";
        colStdDev.Width      = 120;
        colMin.HeaderText    = "Min";
        colMin.Width         = 120;
        colMax.HeaderText    = "Max";
        colMax.Width         = 120;

        dgvSummary.Columns.AddRange(colMethod, colMean, colStdDev, colMin, colMax);
        dgvSummary.ReadOnly             = true;
        dgvSummary.AllowUserToAddRows   = false;
        dgvSummary.RowHeadersVisible    = false;
        dgvSummary.AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSummary.Location             = new Point(10, 20);
        dgvSummary.Size                 = new Size(740, 110);
        dgvSummary.BackgroundColor      = Color.White;
        dgvSummary.BorderStyle          = BorderStyle.None;
        dgvSummary.SelectionMode        = DataGridViewSelectionMode.FullRowSelect;

        grpSummary.Controls.Add(dgvSummary);

        // -----------------------------------------------------------------------
        // grpJoint — bivariate/joint results text area
        // -----------------------------------------------------------------------
        grpJoint.Text     = "Joint (Bivariate) Normal Results";
        grpJoint.Location = new Point(12, 335);
        grpJoint.Size     = new Size(370, 265);

        txtJointResults.Multiline   = true;
        txtJointResults.ReadOnly    = true;
        txtJointResults.ScrollBars  = ScrollBars.Vertical;
        txtJointResults.Font        = new Font("Courier New", 9);
        txtJointResults.Location    = new Point(10, 20);
        txtJointResults.Size        = new Size(350, 235);
        txtJointResults.BackColor   = Color.WhiteSmoke;

        grpJoint.Controls.Add(txtJointResults);

        // -----------------------------------------------------------------------
        // grpHistogram
        // -----------------------------------------------------------------------
        grpHistogram.Text     = "Histogram";
        grpHistogram.Location = new Point(395, 185);
        grpHistogram.Size     = new Size(380, 415);

        lblHistMethod.Text     = "Method:";
        lblHistMethod.Location = new Point(10, 22);
        lblHistMethod.Size     = new Size(55, 20);

        cmbHistMethod.Location = new Point(70, 19);
        cmbHistMethod.Size     = new Size(185, 23);
        cmbHistMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbHistMethod.Items.AddRange(new object[] { "Box-Muller", "Polar (Marsaglia)", "Inverse Transform" });
        cmbHistMethod.SelectedIndex = 0;
        cmbHistMethod.SelectedIndexChanged += cmbHistMethod_SelectedIndexChanged;

        histogramPanel.Location    = new Point(10, 50);
        histogramPanel.Size        = new Size(360, 355);
        histogramPanel.BorderStyle = BorderStyle.FixedSingle;
        histogramPanel.BackColor   = Color.White;
        histogramPanel.Paint       += histogramPanel_Paint;

        grpHistogram.Controls.AddRange(new Control[]
        {
            lblHistMethod, cmbHistMethod, histogramPanel
        });

        // -----------------------------------------------------------------------
        // Status label
        // -----------------------------------------------------------------------
        lblStatus.Text      = "Ready. Enter parameters and click Generate.";
        lblStatus.Location  = new Point(12, 610);
        lblStatus.Size      = new Size(760, 20);
        lblStatus.ForeColor = Color.DarkSlateGray;

        // -----------------------------------------------------------------------
        // Form
        // -----------------------------------------------------------------------
        this.Text            = "FM 5252 — Project 4: Normal Distribution Generator";
        this.Size            = new Size(810, 680);
        this.MinimumSize     = new Size(810, 680);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox     = false;

        this.Controls.AddRange(new Control[]
        {
            grpInputs,
            btnGenerate,
            grpSummary,
            grpJoint,
            grpHistogram,
            lblStatus
        });
    }

    #endregion

    // -----------------------------------------------------------------------
    // Designer fields
    // -----------------------------------------------------------------------
    private GroupBox         grpInputs       = null!;
    private Label            lblSamples      = null!;
    private TextBox          txtSamples      = null!;
    private Label            lblRho          = null!;
    private TextBox          txtRho          = null!;
    private Label            lblMean         = null!;
    private TextBox          txtMean         = null!;
    private Label            lblSigma        = null!;
    private TextBox          txtSigma        = null!;
    private Label            lblSeed         = null!;
    private TextBox          txtSeed         = null!;
    private Button           btnGenerate     = null!;

    private GroupBox         grpSummary      = null!;
    private DataGridView     dgvSummary      = null!;
    private DataGridViewTextBoxColumn colMethod  = null!;
    private DataGridViewTextBoxColumn colMean    = null!;
    private DataGridViewTextBoxColumn colStdDev  = null!;
    private DataGridViewTextBoxColumn colMin     = null!;
    private DataGridViewTextBoxColumn colMax     = null!;

    private GroupBox         grpJoint        = null!;
    private TextBox          txtJointResults = null!;

    private GroupBox         grpHistogram    = null!;
    private Label            lblHistMethod   = null!;
    private ComboBox         cmbHistMethod   = null!;
    private Panel            histogramPanel  = null!;

    private Label            lblStatus       = null!;
}
