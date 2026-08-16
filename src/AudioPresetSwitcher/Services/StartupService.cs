using Microsoft.Win32;
using Velopack.Locators;

namespace AudioPresetSwitcher.Services;

public sealed class StartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "AudioPresetSwitcher";
    private const string TrayArgument = "--tray";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return key?.GetValue(ValueName) is string;
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);

        if (enabled)
        {
            key.SetValue(ValueName, BuildLaunchCommand());
        }
        else if (key.GetValue(ValueName) is not null)
        {
            key.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }

    /// <summary>
    /// Rewrites the Run entry so older installs pick up the current launch path/args.
    /// </summary>
    public void RefreshIfEnabled()
    {
        if (IsEnabled())
        {
            SetEnabled(true);
        }
    }

    private static string BuildLaunchCommand()
    {
        if (VelopackLocator.IsCurrentSet)
        {
            var root = VelopackLocator.Current.RootAppDir;
            if (!string.IsNullOrWhiteSpace(root))
            {
                var stub = Path.Combine(root, "AudioPresetSwitcher.exe");
                if (File.Exists(stub))
                {
                    return $"\"{stub}\" {TrayArgument}";
                }
            }

            var updateExe = VelopackLocator.Current.UpdateExePath;
            if (!string.IsNullOrWhiteSpace(updateExe) && File.Exists(updateExe))
            {
                return $"\"{updateExe}\" start -- {TrayArgument}";
            }
        }

        var exe = Environment.ProcessPath ?? Path.Combine(AppContext.BaseDirectory, "AudioPresetSwitcher.exe");
        return $"\"{exe}\" {TrayArgument}";
    }
}
