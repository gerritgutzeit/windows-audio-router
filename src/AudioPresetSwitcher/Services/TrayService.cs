using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AudioPresetSwitcher.Models;
using H.NotifyIcon;
using H.NotifyIcon.Core;

namespace AudioPresetSwitcher.Services;

public sealed class TrayService : IDisposable
{
    private readonly SettingsService _settings;
    private readonly AudioDeviceService _audio;
    private readonly NotificationService _notifications;
    private readonly WindowService _windows;
    private TaskbarIcon? _icon;
    private Icon? _trayIcon;
    private EventHandler? _settingsChangedHandler;
    private EventHandler<PresetActivationResult>? _presetActivatedHandler;

    public TrayService(
        SettingsService settings,
        AudioDeviceService audio,
        NotificationService notifications,
        WindowService windows)
    {
        _settings = settings;
        _audio = audio;
        _notifications = notifications;
        _windows = windows;
    }

    public void Initialize()
    {
        try
        {
            _trayIcon = LoadTrayIcon();
            _icon = new TaskbarIcon
            {
                ToolTipText = "AudioPresetSwitcher",
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
                _notifications.Show("AudioPresetSwitcher", $"Tray icon failed: {ex.Message}");
            }
            catch
            {
                // ignore secondary failures during startup
            }
        }
    }

    public void Dispose()
    {
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
            var presets = _settings.Current.Presets;
            if (presets.Count == 0)
            {
                menu.Items.Add(new MenuItem { Header = "No presets yet", IsEnabled = false });
            }
            else
            {
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

            menu.Items.Add(new Separator());

            var settings = new MenuItem { Header = "Settings" };
            settings.Click += (_, _) => _windows.ShowSettings();
            menu.Items.Add(settings);

            var exit = new MenuItem { Header = "Exit" };
            exit.Click += (_, _) => _windows.Exit();
            menu.Items.Add(exit);

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

    private void Activate(AudioPreset preset)
    {
        var result = _audio.ActivatePreset(preset);
        if (result.AnySuccess)
        {
            _settings.Update(s => s.LastActivePresetId = preset.Id);
        }

        _notifications.ShowPresetResult(result);
        RebuildMenu();
    }
}
