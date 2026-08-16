using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public sealed class PresetActivationService(
    IAudioDeviceService audio,
    ISettingsService settings,
    INotificationService notifications) : IPresetActivationService
{
    public AudioPreset? Resolve(string? name, int? index)
    {
        var presets = settings.Current.Presets;
        if (!string.IsNullOrWhiteSpace(name))
        {
            return presets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        if (index is >= 0 && index.Value < presets.Count)
        {
            return presets[index.Value];
        }

        return null;
    }

    public PresetActivationResult ActivateAndRemember(AudioPreset preset)
    {
        var result = audio.ActivatePreset(preset);
        if (result.AnySuccess)
        {
            settings.Update(s => s.LastActivePresetId = preset.Id);
        }

        notifications.ShowPresetResult(result);
        return result;
    }

    public PresetActivationResult? ActivateFromOptions(StartupOptions options) =>
        ActivateResolved(options.PresetName, options.PresetIndex);

    public PresetActivationResult? ActivateFromRequest(string? name, int? index) =>
        ActivateResolved(name, index);

    private PresetActivationResult? ActivateResolved(string? name, int? index)
    {
        var preset = Resolve(name, index);
        return preset is null ? null : ActivateAndRemember(preset);
    }
}
