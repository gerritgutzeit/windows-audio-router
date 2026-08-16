using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.ViewModels.Pages;
using AudioPresetSwitcher.ViewModels.Windows;
using AudioPresetSwitcher.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AudioPresetSwitcher.Services;

public sealed class ApplicationHostService(
    IServiceProvider services,
    StartupOptions options,
    ISettingsService settings,
    IAudioDeviceService audio,
    IPresetActivationService activation,
    INotificationService notifications,
    IThemeSettingsService theme,
    StartupService startup,
    UpdateService updates,
    IWindowService windows,
    TrayService tray) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        notifications.Initialize();
        startup.RefreshIfEnabled();
        if (settings.Current.RunAtStartup != startup.IsEnabled())
        {
            settings.Update(s => s.RunAtStartup = startup.IsEnabled());
        }

        var window = services.GetRequiredService<MainWindow>();
        // Re-apply after MainWindow: SystemThemeWatcher.Watch may ApplySystemTheme on first watch.
        theme.Apply(settings.Current.Theme);
        windows.Attach(window);
        tray.Initialize();

        if (options.HasPresetRequest)
        {
            var result = activation.ActivateFromOptions(options);
            if (result is null)
            {
                notifications.Show(AppIdentity.Name, "Preset not found");
            }
        }

        if (options.ShowWindow)
        {
            windows.ShowDashboard();
        }

        _ = updates.CheckAndDownloadInBackgroundAsync(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        settings.SaveNow();
        services.GetService<DevicesViewModel>()?.Dispose();
        services.GetService<CurrentAudioStatusViewModel>()?.Dispose();
        tray.Dispose();
        audio.Dispose();
        return Task.CompletedTask;
    }
}
