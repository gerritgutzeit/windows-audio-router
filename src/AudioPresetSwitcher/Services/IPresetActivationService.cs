using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public interface IPresetActivationService
{
    AudioPreset? Resolve(string? name, int? index);

    PresetActivationResult ActivateAndRemember(AudioPreset preset);

    PresetActivationResult? ActivateFromOptions(StartupOptions options);

    PresetActivationResult? ActivateFromRequest(string? name, int? index);
}
