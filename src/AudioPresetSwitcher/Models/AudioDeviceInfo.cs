using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Models;

public sealed class AudioDeviceInfo
{
    public required string Id { get; init; }

    public required string FriendlyName { get; init; }

    public required DataFlow Flow { get; init; }

    public override string ToString() => FriendlyName;
}
