namespace AudioPresetSwitcher.Models;

public sealed class AudioPreset
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "New preset";

    public string Icon { get; set; } = "Headphones";

    public string PlaybackKeyword { get; set; } = string.Empty;

    public string RecordingKeyword { get; set; } = string.Empty;

    public AudioPreset Clone()
    {
        return new AudioPreset
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Icon = Icon,
            PlaybackKeyword = PlaybackKeyword,
            RecordingKeyword = RecordingKeyword
        };
    }
}
