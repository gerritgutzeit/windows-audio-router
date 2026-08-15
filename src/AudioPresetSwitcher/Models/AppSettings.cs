namespace AudioPresetSwitcher.Models;

public sealed class AppSettings
{
    public AppThemeMode Theme { get; set; } = AppThemeMode.System;

    public bool RunAtStartup { get; set; }

    public bool ShowToastNotifications { get; set; } = true;

    public Guid? LastActivePresetId { get; set; }

    public List<AudioPreset> Presets { get; set; } = [];
}
