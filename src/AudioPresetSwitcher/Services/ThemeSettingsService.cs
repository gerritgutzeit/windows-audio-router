using System.Windows;
using System.Windows.Media;
using AudioPresetSwitcher.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.Services;

public sealed class ThemeSettingsService
{
    private bool _applying;

    public ThemeSettingsService()
    {
        ApplicationThemeManager.Changed += OnApplicationThemeChanged;
    }

    public void Apply(AppThemeMode mode)
    {
        var theme = ResolveTheme(mode);

        _applying = true;
        try
        {
            ApplicationThemeManager.Apply(theme, WindowBackdropType.Mica, updateAccent: false);
            ApplyStudioChrome(theme);
        }
        finally
        {
            _applying = false;
        }
    }

    private void OnApplicationThemeChanged(ApplicationTheme theme, Color _)
    {
        if (_applying || theme is ApplicationTheme.Unknown)
        {
            return;
        }

        // SystemThemeWatcher (and other callers) may re-apply system blue; keep studio brass.
        ApplyStudioChrome(theme);
    }

    private static ApplicationTheme ResolveTheme(AppThemeMode mode) =>
        mode switch
        {
            AppThemeMode.Dark => ApplicationTheme.Dark,
            AppThemeMode.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Light
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark
        };

    private static void ApplyStudioChrome(ApplicationTheme theme)
    {
        ApplicationAccentColorManager.Apply(StudioPalette.Brass, theme);

        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        if (theme == ApplicationTheme.Dark)
        {
            resources["ApplicationBackgroundBrush"] = new SolidColorBrush(StudioPalette.DarkBackground);
        }
        else
        {
            // Reveal Fluent light background from the theme dictionary again.
            resources.Remove("ApplicationBackgroundBrush");
        }
    }
}
