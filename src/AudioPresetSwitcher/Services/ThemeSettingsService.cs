using System.Windows;
using System.Windows.Media;
using AudioPresetSwitcher.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.Services;

public sealed class ThemeSettingsService
{
    private bool _applying;
    private AppThemeMode _mode = AppThemeMode.System;

    public ThemeSettingsService()
    {
        ApplicationThemeManager.Changed += OnApplicationThemeChanged;
    }

    public void Apply(AppThemeMode mode)
    {
        _mode = mode;
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

    /// <summary>
    /// Re-apply after the window visual tree is ready (WPF-UI theme brushes load late).
    /// </summary>
    public void Refresh() => ApplyStudioChrome(ResolveTheme(_mode));

    private void OnApplicationThemeChanged(ApplicationTheme theme, Color _)
    {
        if (_applying || theme is ApplicationTheme.Unknown)
        {
            return;
        }

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
        var brass = StudioPalette.Brass;
        // Explicit variants — Primary buttons use AccentFillColorDefault via AccentButtonBackground;
        // NavigationView / toggles use SystemAccentColorPrimary.
        var primary = theme == ApplicationTheme.Dark
            ? StudioPalette.BrassLight
            : StudioPalette.BrassDark;
        var secondary = theme == ApplicationTheme.Dark
            ? StudioPalette.Brass
            : StudioPalette.BrassDarker;
        var tertiary = theme == ApplicationTheme.Dark
            ? StudioPalette.BrassLighter
            : StudioPalette.BrassDarkest;

        ApplicationAccentColorManager.Apply(brass, primary, secondary, tertiary);

        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        // Color keys that DynamicResource brushes (AccentButtonBackground, nav pill, etc.) resolve.
        SetColor(resources, "SystemAccentColor", brass);
        SetColor(resources, "SystemAccentColorPrimary", primary);
        SetColor(resources, "SystemAccentColorSecondary", secondary);
        SetColor(resources, "SystemAccentColorTertiary", tertiary);
        SetColor(resources, "AccentFillColorDefault", secondary);
        SetColor(resources, "AccentFillColorSecondary", Color.FromArgb(0xE5, secondary.R, secondary.G, secondary.B));
        SetColor(resources, "AccentFillColorTertiary", Color.FromArgb(0xCC, secondary.R, secondary.G, secondary.B));

        // Brush keys — replace StaticResource-bound brushes from Accent.xaml that never update.
        SetBrush(resources, "SystemAccentBrush", brass);
        SetBrush(resources, "SystemAccentColorBrush", brass);
        SetBrush(resources, "SystemAccentColorPrimaryBrush", primary);
        SetBrush(resources, "SystemAccentColorSecondaryBrush", secondary);
        SetBrush(resources, "SystemAccentColorTertiaryBrush", tertiary);
        SetBrush(resources, "SystemFillColorAttentionBrush", brass);
        SetBrush(resources, "AccentFillColorDefaultBrush", secondary);
        SetBrush(resources, "AccentFillColorSecondaryBrush", secondary, 0.9);
        SetBrush(resources, "AccentFillColorTertiaryBrush", secondary, 0.8);
        SetBrush(resources, "AccentFillColorSelectedTextBackgroundBrush", brass);
        SetBrush(resources, "AccentTextFillColorPrimaryBrush", secondary);
        SetBrush(resources, "AccentTextFillColorSecondaryBrush", tertiary);
        SetBrush(resources, "AccentTextFillColorTertiaryBrush", primary);
        SetBrush(resources, "AccentButtonBackground", secondary);
        SetBrush(resources, "AccentButtonBackgroundPointerOver", Color.FromArgb(0xE5, secondary.R, secondary.G, secondary.B));
        SetBrush(resources, "AccentButtonBackgroundPressed", Color.FromArgb(0xCC, secondary.R, secondary.G, secondary.B));
        SetBrush(resources, "NavigationViewSelectionIndicatorForeground", primary);

        // Brass is light enough for dark label text on accent fills.
        SetColor(resources, "TextOnAccentFillColorPrimary", Colors.Black);
        SetBrush(resources, "AccentButtonForeground", Colors.Black);
        SetBrush(resources, "AccentButtonForegroundPointerOver", Colors.Black);

        if (theme == ApplicationTheme.Dark)
        {
            SetBrush(resources, "ApplicationBackgroundBrush", StudioPalette.DarkBackground);
        }
        else
        {
            resources.Remove("ApplicationBackgroundBrush");
        }
    }

    private static void SetColor(ResourceDictionary resources, string key, Color color) =>
        resources[key] = color;

    private static void SetBrush(ResourceDictionary resources, string key, Color color, double opacity = 1.0)
    {
        var brush = new SolidColorBrush(color) { Opacity = opacity };
        brush.Freeze();
        resources[key] = brush;
    }
}
