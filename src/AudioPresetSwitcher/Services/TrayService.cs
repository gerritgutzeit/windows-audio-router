using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using AudioPresetSwitcher.Models;
using H.NotifyIcon;
using H.NotifyIcon.Core;

namespace AudioPresetSwitcher.Services;

public sealed class TrayService : IDisposable
{
    private readonly ISettingsService _settings;
    private readonly IAudioDeviceService _audio;
    private readonly IPresetActivationService _activation;
    private readonly INotificationService _notifications;
    private readonly IWindowService _windows;
    private TaskbarIcon? _icon;
    private Icon? _trayIcon;
    private EventHandler? _settingsChangedHandler;
    private EventHandler<PresetActivationResult>? _presetActivatedHandler;

    public TrayService(
        ISettingsService settings,
        IAudioDeviceService audio,
        IPresetActivationService activation,
        INotificationService notifications,
        IWindowService windows)
    {
        _settings = settings;
        _audio = audio;
        _activation = activation;
        _notifications = notifications;
        _windows = windows;
    }

    public void Initialize()
    {
        _windows.Exiting += Dispose;
        try
        {
            _trayIcon = LoadTrayIcon();
            _icon = new TaskbarIcon
            {
                ToolTipText = AppIdentity.Name,
                Icon = _trayIcon,
                NoLeftClickDelay = true,
                MenuActivation = PopupActivationMode.RightClick,
                LeftClickCommand = new RelayCommand(_windows.ShowDashboard)
            };

            RebuildMenu();
            _icon.ForceCreate(enablesEfficiencyMode: false);

            _settingsChangedHandler = (_, _) => RebuildMenu();
            _presetActivatedHandler = (_, _) => RebuildMenu();
            _settings.Changed += _settingsChangedHandler;
            _audio.PresetActivated += _presetActivatedHandler;
            _notifications.TrayNotification += OnTrayNotification;
        }
        catch (Exception ex)
        {
            try
            {
                _notifications.Show(AppIdentity.Name, $"Tray icon failed: {ex.Message}");
            }
            catch
            {
                // ignore secondary failures during startup
            }
        }
    }

    public void Dispose()
    {
        _windows.Exiting -= Dispose;
        _notifications.TrayNotification -= OnTrayNotification;
        if (_settingsChangedHandler is not null)
        {
            _settings.Changed -= _settingsChangedHandler;
        }

        if (_presetActivatedHandler is not null)
        {
            _audio.PresetActivated -= _presetActivatedHandler;
        }

        _icon?.Dispose();
        _icon = null;
        _trayIcon?.Dispose();
        _trayIcon = null;
    }

    private static Icon LoadTrayIcon()
    {
        var uri = new Uri("pack://application:,,,/Assets/app.ico");
        var streamInfo = Application.GetResourceStream(uri)
            ?? throw new InvalidOperationException("Tray icon resource Assets/app.ico was not found.");

        using var stream = streamInfo.Stream;
        using var loaded = new Icon(stream);
        return (Icon)loaded.Clone();
    }

    private void OnTrayNotification(string title, string message)
    {
        _icon?.ShowNotification(title, message);
    }

    private void RebuildMenu()
    {
        if (_icon is null)
        {
            return;
        }

        void Build()
        {
            var menu = new ContextMenu();
            AddPresetItems(menu);
            menu.Items.Add(new Separator());
            AddFooterItems(menu);
            _icon.ContextMenu = menu;
        }

        if (_icon.Dispatcher.CheckAccess())
        {
            Build();
        }
        else
        {
            _ = _icon.Dispatcher.BeginInvoke(Build);
        }
    }

    private void AddPresetItems(ContextMenu menu)
    {
        var presets = _settings.Current.Presets;
        if (presets.Count == 0)
        {
            menu.Items.Add(new MenuItem { Header = "No presets yet", IsEnabled = false });
            return;
        }

        foreach (var preset in presets)
        {
            var item = new MenuItem
            {
                Header = preset.Name,
                IsCheckable = true,
                IsChecked = preset.Id == _settings.Current.LastActivePresetId,
                StaysOpenOnClick = false
            };
            var captured = preset;
            item.Click += (_, _) => Activate(captured);
            menu.Items.Add(item);
        }
    }

    private void AddFooterItems(ContextMenu menu)
    {
        var settings = new MenuItem { Header = "Settings" };
        settings.Click += (_, _) => _windows.ShowSettings();
        menu.Items.Add(settings);

        var exit = new MenuItem { Header = "Exit" };
        exit.Click += (_, _) => _windows.Exit();
        menu.Items.Add(exit);
    }

    private void Activate(AudioPreset preset)
    {
        _activation.ActivateAndRemember(preset);
        RebuildMenu();
    }
}
