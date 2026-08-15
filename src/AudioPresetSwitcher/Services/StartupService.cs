using Microsoft.Win32;
using Velopack.Locators;

namespace AudioPresetSwitcher.Services;

public sealed class StartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "AudioPresetSwitcher";

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

    private static string BuildLaunchCommand()
    {
        if (VelopackLocator.IsCurrentSet)
        {
            var updateExe = VelopackLocator.Current.UpdateExePath;
            if (!string.IsNullOrWhiteSpace(updateExe) && File.Exists(updateExe))
            {
                return $"\"{updateExe}\" start";
            }
        }

        var exe = Environment.ProcessPath ?? Path.Combine(AppContext.BaseDirectory, "AudioPresetSwitcher.exe");
        return $"\"{exe}\"";
    }
}
