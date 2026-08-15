namespace AudioPresetSwitcher.Models;

public sealed class StartupOptions
{
    public string? PresetName { get; set; }

    public int? PresetIndex { get; set; }

    public bool ShowWindow { get; set; } = true;

    public bool HasPresetRequest => PresetName is not null || PresetIndex is not null;
}
