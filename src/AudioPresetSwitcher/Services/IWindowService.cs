using AudioPresetSwitcher.Views.Windows;

namespace AudioPresetSwitcher.Services;

public interface IWindowService
{
    bool ExitRequested { get; }

    event Action? Exiting;

    void Attach(MainWindow window);

    void ShowDashboard();

    void ShowSettings();

    void HideDashboard();

    void Exit();
}
