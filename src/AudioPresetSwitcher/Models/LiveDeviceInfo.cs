using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Models;

public sealed class LiveDeviceInfo
{
    public required string Id { get; init; }

    public required string FriendlyName { get; init; }

    public required DataFlow Flow { get; init; }

    public bool IsDefaultMultimedia { get; init; }

    public bool IsDefaultCommunications { get; init; }

    public float Volume { get; set; }

    public bool IsMuted { get; set; }

    public float Peak { get; set; }
}
