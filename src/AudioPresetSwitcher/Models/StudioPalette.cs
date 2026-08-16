using System.Windows.Media;

namespace AudioPresetSwitcher.Models;

/// <summary>
/// Fixed studio look: dark charcoal metal + brass accent (not Windows system blue).
/// </summary>
public static class StudioPalette
{
    /// <summary>Muted brass / gold used for primary actions and accents.</summary>
    public static Color Brass { get; } = Color.FromRgb(0xC5, 0xA0, 0x59);

    /// <summary>Warm near-black charcoal for dark-mode window background.</summary>
    public static Color DarkBackground { get; } = Color.FromRgb(0x1C, 0x19, 0x16);
}
