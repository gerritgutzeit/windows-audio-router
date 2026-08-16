using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public interface ISettingsService
{
    AppSettings Current { get; }

    event EventHandler? Changed;

    void Update(Action<AppSettings> mutate);

    void SaveNow();
}
