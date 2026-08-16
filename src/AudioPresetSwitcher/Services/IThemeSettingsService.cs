using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public interface IThemeSettingsService
{
    void Apply(AppThemeMode mode);

    void Refresh();
}
