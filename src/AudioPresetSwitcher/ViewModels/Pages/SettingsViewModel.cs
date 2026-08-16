using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IThemeSettingsService _theme;
    private readonly StartupService _startup;
    private readonly UpdateService _updates;
    private bool _suppress;

    public SettingsViewModel(
        ISettingsService settings,
        IThemeSettingsService theme,
        StartupService startup,
        UpdateService updates)
    {
        _settings = settings;
        _theme = theme;
        _startup = startup;
        _updates = updates;
        ThemeOptions = [AppThemeMode.System, AppThemeMode.Dark, AppThemeMode.Light];
        Load();
    }

    public AppThemeMode[] ThemeOptions { get; }

    [ObservableProperty]
    private bool _runAtStartup;

    [ObservableProperty]
    private bool _showToastNotifications;

    [ObservableProperty]
    private AppThemeMode _selectedTheme = AppThemeMode.System;

    [ObservableProperty]
    private string _versionText = "";

    [ObservableProperty]
    private string _updateStatusText = "";

    [ObservableProperty]
    private bool _isCheckingUpdates;

    [ObservableProperty]
    private bool _canApplyUpdate;

    [ObservableProperty]
    private bool _updatesSupported;

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

    partial void OnSelectedThemeChanged(AppThemeMode value)
    {
        if (_suppress)
        {
            return;
        }

        _settings.Update(s => s.Theme = value);
        _theme.Apply(value);
    }

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        if (IsCheckingUpdates)
        {
            return;
        }

        IsCheckingUpdates = true;
        UpdateStatusText = "Checking…";
        try
        {
            var ready = await _updates.CheckAndDownloadAsync(showStatus: true);
            RefreshUpdateState();
            if (!ready && UpdatesSupported)
            {
                UpdateStatusText = "Up to date";
            }
        }
        catch
        {
            UpdateStatusText = "Check failed";
        }
        finally
        {
            IsCheckingUpdates = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyUpdate))]
    private void RestartAndUpdate()
    {
        _updates.ApplyAndRestart();
    }

    private void Load()
    {
        _suppress = true;
        RunAtStartup = _startup.IsEnabled();
        if (_settings.Current.RunAtStartup != RunAtStartup)
        {
            _settings.Update(s => s.RunAtStartup = RunAtStartup);
        }

        ShowToastNotifications = _settings.Current.ShowToastNotifications;
        SelectedTheme = _settings.Current.Theme;
        _suppress = false;
        RefreshUpdateState();
    }

    private void RefreshUpdateState()
    {
        UpdatesSupported = _updates.IsInstalled;
        VersionText = $"Version {_updates.CurrentVersionText}";
        CanApplyUpdate = _updates.UpdateReady;
        RestartAndUpdateCommand.NotifyCanExecuteChanged();

        if (!UpdatesSupported)
        {
            UpdateStatusText = "Install Setup.exe to enable auto-updates";
        }
        else if (CanApplyUpdate)
        {
            UpdateStatusText = "Update downloaded — restart to apply";
        }
        else
        {
            UpdateStatusText = "Auto-updates from GitHub Releases";
        }
    }
}
