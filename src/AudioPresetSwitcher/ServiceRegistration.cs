using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Pages;
using AudioPresetSwitcher.ViewModels.Windows;
using AudioPresetSwitcher.Views.Pages;
using AudioPresetSwitcher.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace AudioPresetSwitcher;

internal static class ServiceRegistration
{
    public static IServiceCollection AddAudioPresetSwitcher(this IServiceCollection services, StartupOptions options)
    {
        services.AddSingleton(options);
        services.AddNavigationViewPageProvider();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();

        services.AddSingleton<SettingsService>();
        services.AddSingleton<ISettingsService>(sp => sp.GetRequiredService<SettingsService>());
        services.AddSingleton<AudioDeviceService>();
        services.AddSingleton<IAudioDeviceService>(sp => sp.GetRequiredService<AudioDeviceService>());
        services.AddSingleton<PresetActivationService>();
        services.AddSingleton<IPresetActivationService>(sp => sp.GetRequiredService<PresetActivationService>());
        services.AddSingleton<ShortcutService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<INotificationService>(sp => sp.GetRequiredService<NotificationService>());
        services.AddSingleton<ThemeSettingsService>();
        services.AddSingleton<IThemeSettingsService>(sp => sp.GetRequiredService<ThemeSettingsService>());
        services.AddSingleton<StartupService>();
        services.AddSingleton<UpdateService>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<IWindowService>(sp => sp.GetRequiredService<WindowService>());
        services.AddSingleton<TrayService>();
        services.AddSingleton<IpcService>();
        services.AddHostedService(sp => sp.GetRequiredService<IpcService>());
        services.AddHostedService<ApplicationHostService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<CurrentAudioStatusViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<PresetsViewModel>();
        services.AddSingleton<PresetsPage>();
        services.AddSingleton<DevicesViewModel>();
        services.AddSingleton<DevicesPage>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsPage>();

        return services;
    }
}
