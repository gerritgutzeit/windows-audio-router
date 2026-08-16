using System.Runtime.InteropServices;
using System.Windows;
using AudioPresetSwitcher.Models;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AudioPresetSwitcher.Services;

public sealed class NotificationService : INotificationService
{
    public const string AppUserModelId = AppIdentity.AppUserModelId;

    private readonly ISettingsService _settings;
    private readonly ShortcutService _shortcuts;

    public NotificationService(ISettingsService settings, ShortcutService shortcuts)
    {
        _settings = settings;
        _shortcuts = shortcuts;
    }

    public void Initialize()
    {
        try
        {
            NativeMethods.SetCurrentProcessExplicitAppUserModelID(AppUserModelId);
            _shortcuts.EnsureStartMenuShortcut(AppUserModelId, AppIdentity.Name);
        }
        catch
        {
            // Toasts may still work; tray balloon is the fallback.
        }
    }

    public void ShowPresetResult(PresetActivationResult result)
    {
        if (!_settings.Current.ShowToastNotifications)
        {
            return;
        }

        Show(result.AllRequestedSucceeded ? "Audio preset switched" : "Audio preset", result.Summary);
    }

    public void Show(string title, string message)
    {
        if (!_settings.Current.ShowToastNotifications)
        {
            return;
        }

        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
        catch
        {
            try
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    TrayNotification?.Invoke(title, message);
                });
            }
            catch
            {
                // best-effort
            }
        }
    }

    public event Action<string, string>? TrayNotification;
}

internal static class NativeMethods
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int SetCurrentProcessExplicitAppUserModelID(string appID);
}
