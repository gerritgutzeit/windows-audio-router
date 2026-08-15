using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Pages;
using AudioPresetSwitcher.ViewModels.Windows;
using AudioPresetSwitcher.Views.Pages;
using AudioPresetSwitcher.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace AudioPresetSwitcher;

public partial class App
{
    private IHost? _host;
    private Mutex? _mutex;

    public static IServiceProvider Services =>
        ((App)Current)._host?.Services ?? throw new InvalidOperationException("The application host is not running.");

    public static T GetRequiredService<T>() where T : notnull => Services.GetRequiredService<T>();

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        var options = CliParser.Parse(e.Args);
        _mutex = new Mutex(true, IpcService.MutexName, out var createdNew);
        if (!createdNew)
        {
            Shutdown(IpcService.Send(options));
            return;
        }

        _host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(options);
                services.AddNavigationViewPageProvider();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<IContentDialogService, ContentDialogService>();

                services.AddSingleton<SettingsService>();
                services.AddSingleton<AudioDeviceService>();
                services.AddSingleton<NotificationService>();
                services.AddSingleton<ThemeSettingsService>();
                services.AddSingleton<StartupService>();
                services.AddSingleton<WindowService>();
                services.AddSingleton<TrayService>();
                services.AddSingleton<IpcService>();
                services.AddHostedService(sp => sp.GetRequiredService<IpcService>());
                services.AddHostedService<ApplicationHostService>();

                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<PresetsViewModel>();
                services.AddSingleton<PresetsPage>();
                services.AddSingleton<DevicesViewModel>();
                services.AddSingleton<DevicesPage>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<SettingsPage>();
            })
            .Build();

        await _host.StartAsync();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        try
        {
            _mutex?.ReleaseMutex();
        }
        catch (ApplicationException)
        {
            // mutex not owned
        }

        _mutex?.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        try
        {
            GetRequiredService<NotificationService>().Show("AudioPresetSwitcher", e.Exception.Message);
        }
        catch
        {
            // ignore
        }
    }
}
