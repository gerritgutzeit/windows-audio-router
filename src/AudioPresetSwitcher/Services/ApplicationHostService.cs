using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.ViewModels.Pages;
using AudioPresetSwitcher.Views.Pages;
using AudioPresetSwitcher.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AudioPresetSwitcher.Services;

public sealed class ApplicationHostService(
    IServiceProvider services,
    StartupOptions options,
    SettingsService settings,
    AudioDeviceService audio,
    NotificationService notifications,
    ThemeSettingsService theme,
    StartupService startup,
    UpdateService updates,
    WindowService windows,
    TrayService tray) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        notifications.Initialize();
        theme.Apply(settings.Current.Theme);
        if (settings.Current.RunAtStartup != startup.IsEnabled())
        {
            settings.Update(s => s.RunAtStartup = startup.IsEnabled());
        }

        var window = services.GetRequiredService<MainWindow>();
        windows.Attach(window);
        tray.Initialize();

        if (options.HasPresetRequest)
        {
            var result = audio.ActivatePresetFromOptions(settings, options);
            if (result is not null)
            {
                if (result.AnySuccess)
                {
                    settings.Update(s => s.LastActivePresetId = result.Preset.Id);
                }

                notifications.ShowPresetResult(result);
            }
            else
            {
                notifications.Show("AudioPresetSwitcher", "Preset not found");
            }
        }

        window.Navigate(typeof(PresetsPage));
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
        tray.Dispose();
        audio.Dispose();
        return Task.CompletedTask;
    }
}

internal static class ActivationExtensions
{
    public static PresetActivationResult? ActivatePresetFromOptions(
        this AudioDeviceService audio,
        SettingsService settings,
        StartupOptions options)
    {
        AudioPreset? preset = null;
        if (!string.IsNullOrWhiteSpace(options.PresetName))
        {
            preset = settings.Current.Presets.FirstOrDefault(p =>
                p.Name.Equals(options.PresetName, StringComparison.OrdinalIgnoreCase));
        }
        else if (options.PresetIndex is >= 0 && options.PresetIndex.Value < settings.Current.Presets.Count)
        {
            preset = settings.Current.Presets[options.PresetIndex.Value];
        }

        return preset is null ? null : audio.ActivatePreset(preset);
    }
}
