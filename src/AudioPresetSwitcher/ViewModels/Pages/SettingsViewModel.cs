using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private readonly ThemeSettingsService _theme;
    private readonly StartupService _startup;
    private bool _suppress;

    public SettingsViewModel(SettingsService settings, ThemeSettingsService theme, StartupService startup)
    {
        _settings = settings;
        _theme = theme;
        _startup = startup;
        ThemeOptions = ["System", "Dark", "Light"];
        Load();
    }

    public string[] ThemeOptions { get; }

    [ObservableProperty]
    private bool _runAtStartup;

    [ObservableProperty]
    private bool _showToastNotifications;

    [ObservableProperty]
    private string _selectedTheme = "System";

    partial void OnRunAtStartupChanged(bool value)
    {
        if (_suppress)
        {
            return;
        }

        _startup.SetEnabled(value);
        _settings.Update(s => s.RunAtStartup = value);
    }

    partial void OnShowToastNotificationsChanged(bool value)
    {
        if (_suppress)
        {
            return;
        }

        _settings.Update(s => s.ShowToastNotifications = value);
    }

    partial void OnSelectedThemeChanged(string value)
    {
        if (_suppress)
        {
            return;
        }

        var mode = value switch
        {
            "Dark" => AppThemeMode.Dark,
            "Light" => AppThemeMode.Light,
            _ => AppThemeMode.System
        };
        _settings.Update(s => s.Theme = mode);
        _theme.Apply(mode);
    }

    private void Load()
    {
        _suppress = true;
        RunAtStartup = _settings.Current.RunAtStartup;
        ShowToastNotifications = _settings.Current.ShowToastNotifications;
        SelectedTheme = _settings.Current.Theme switch
        {
            AppThemeMode.Dark => "Dark",
            AppThemeMode.Light => "Light",
            _ => "System"
        };
        _suppress = false;
    }
}
