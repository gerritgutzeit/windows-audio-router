using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public interface INotificationService
{
    event Action<string, string>? TrayNotification;

    void Initialize();

    void ShowPresetResult(PresetActivationResult result);

    void Show(string title, string message);
}
