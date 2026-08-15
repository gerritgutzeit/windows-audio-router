using AudioPresetSwitcher.Models;
using Wpf.Ui.Appearance;

namespace AudioPresetSwitcher.Services;

public sealed class ThemeSettingsService
{
    public void Apply(AppThemeMode mode)
    {
        var theme = mode switch
        {
            AppThemeMode.Dark => ApplicationTheme.Dark,
            AppThemeMode.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Light
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark
        };

        ApplicationThemeManager.Apply(theme);
    }
}
