namespace Project4;

/// <summary>
/// Application entry point. Configures the WinForms runtime and opens the
/// main window.
/// </summary>
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
