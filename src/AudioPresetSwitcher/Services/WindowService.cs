using System.Windows;
using AudioPresetSwitcher.Views.Windows;

namespace AudioPresetSwitcher.Services;

public sealed class WindowService
{
    public const string ExitTag = "exit";

    private MainWindow? _window;

    public bool ExitRequested { get; private set; }

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

    public void HideDashboard()
    {
        if (_window is null)
        {
            return;
        }

        _window.ShowInTaskbar = false;
        _window.Hide();
    }

    public void Exit()
    {
        ExitRequested = true;
        Application.Current.Shutdown();
    }
}
