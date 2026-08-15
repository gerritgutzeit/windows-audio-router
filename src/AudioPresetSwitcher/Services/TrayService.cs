using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        _icon = new TaskbarIcon
        {
            ToolTipText = "AudioPresetSwitcher",
            IconSource = new BitmapImage(new Uri("pack://application:,,,/Assets/app.ico")),
            NoLeftClickDelay = true,
            MenuActivation = PopupActivationMode.RightClick,
            LeftClickCommand = new RelayCommand(_windows.ShowDashboard)
        };

        RebuildMenu();
        _settings.Changed += (_, _) => RebuildMenu();
        _audio.PresetActivated += (_, _) => RebuildMenu();
        _notifications.TrayNotification += OnTrayNotification;
    }

    public void Dispose()
    {
        _notifications.TrayNotification -= OnTrayNotification;
        _icon?.Dispose();
        _icon = null;
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
            var open = new MenuItem { Header = "Open AudioPresetSwitcher" };
            open.Click += (_, _) => _windows.ShowDashboard();
            menu.Items.Add(open);

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
