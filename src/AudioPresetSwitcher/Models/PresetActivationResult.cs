using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Models;

public sealed class PresetActivationResult
{
    public required AudioPreset Preset { get; init; }

    public bool PlaybackSucceeded { get; init; }

    public bool RecordingSucceeded { get; init; }

    public bool PlaybackSkipped { get; init; }

    public bool RecordingSkipped { get; init; }

    public string? PlaybackError { get; init; }

    public string? RecordingError { get; init; }

    public bool AnySuccess => PlaybackSucceeded || RecordingSucceeded;

    public bool AllRequestedSucceeded =>
        (PlaybackSkipped || PlaybackSucceeded) && (RecordingSkipped || RecordingSucceeded);

    public string Summary
    {
        get
        {
            if (AllRequestedSucceeded)
            {
                return $"Switched to {Preset.Name}";
            }

            var parts = new List<string>();
            if (!PlaybackSkipped && !PlaybackSucceeded)
            {
                parts.Add(PlaybackError ?? "playback device not found");
            }

            if (!RecordingSkipped && !RecordingSucceeded)
            {
                parts.Add(RecordingError ?? "recording device not found");
            }

            if (AnySuccess)
            {
                return $"Partially switched to {Preset.Name}: {string.Join("; ", parts)}";
            }

            return $"Could not switch to {Preset.Name}: {string.Join("; ", parts)}";
        }
    }
}
