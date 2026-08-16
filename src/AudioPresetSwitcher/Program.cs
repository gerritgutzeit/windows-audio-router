using Velopack;

namespace AudioPresetSwitcher;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Keep pending updates until Settings → Restart. Auto-apply on boot would
        // relaunch without the Run-key args (e.g. --tray) and open the main window.
        VelopackApp.Build().SetAutoApplyOnStartup(false).Run();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
