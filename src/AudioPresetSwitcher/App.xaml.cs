using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) => services.AddAudioPresetSwitcher(options))
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
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppIdentity.Name);
            Directory.CreateDirectory(logDir);
            File.AppendAllText(
                Path.Combine(logDir, "error.log"),
                $"{DateTime.Now:o}{Environment.NewLine}{e.Exception}{Environment.NewLine}{Environment.NewLine}");
        }
        catch
        {
            // ignore
        }

        try
        {
            GetRequiredService<INotificationService>().Show(AppIdentity.Name, e.Exception.Message);
        }
        catch
        {
            // ignore
        }
    }
}
