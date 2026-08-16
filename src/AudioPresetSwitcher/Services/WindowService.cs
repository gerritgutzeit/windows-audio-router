using System.Windows;
using AudioPresetSwitcher.Views.Pages;
using AudioPresetSwitcher.Views.Windows;

namespace AudioPresetSwitcher.Services;

public sealed class WindowService : IWindowService
{
    private MainWindow? _window;

    public bool ExitRequested { get; private set; }

    public event Action? Exiting;

    public void Attach(MainWindow window) => _window = window;

    public void ShowDashboard()
    {
        if (_window is null)
        {
            return;
        }

        if (_window.WindowState == WindowState.Minimized)
        {
            _window.WindowState = WindowState.Normal;
        }

        _window.ShowInTaskbar = true;
        _window.Show();
        _ = _window.Activate();
        _ = _window.Focus();
    }

    public void ShowSettings()
    {
        ShowDashboard();
        _ = _window?.Navigate(typeof(SettingsPage));
    }

    public void HideDashboard()
    {
        if (_window is null)
        {
            return;
        }

        if (_window.WindowState == WindowState.Minimized)
        {
            _window.WindowState = WindowState.Normal;
        }

        _window.ShowInTaskbar = false;
        _window.Hide();
    }

    public void Exit()
    {
        if (ExitRequested)
        {
            return;
        }

        ExitRequested = true;

        void ShutdownApp()
        {
            Exiting?.Invoke();
            Application.Current.Shutdown();
        }

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            Environment.Exit(0);
            return;
        }

        if (dispatcher.CheckAccess())
        {
            ShutdownApp();
        }
        else
        {
            dispatcher.Invoke(ShutdownApp);
        }
    }
}
