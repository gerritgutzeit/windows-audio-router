using System.Windows.Media;

namespace AudioPresetSwitcher.Models;

/// <summary>
/// Fixed studio look: dark charcoal metal + brass accent (not Windows system blue).
/// </summary>
public static class StudioPalette
{
    /// <summary>Base muted brass / gold.</summary>
    public static Color Brass { get; } = Color.FromRgb(0xC5, 0xA0, 0x59);

    /// <summary>Lighter brass for dark-theme primary accents.</summary>
    public static Color BrassLight { get; } = Color.FromRgb(0xD4, 0xB5, 0x6E);

    /// <summary>Soft highlight brass.</summary>
    public static Color BrassLighter { get; } = Color.FromRgb(0xE2, 0xC9, 0x8A);

    /// <summary>Deeper brass for light-theme accents.</summary>
    public static Color BrassDark { get; } = Color.FromRgb(0xA8, 0x84, 0x3E);

    /// <summary>Darker brass.</summary>
    public static Color BrassDarker { get; } = Color.FromRgb(0x8A, 0x6B, 0x2E);

    /// <summary>Darkest brass.</summary>
    public static Color BrassDarkest { get; } = Color.FromRgb(0x6B, 0x52, 0x22);

    /// <summary>Warm near-black charcoal for dark-mode window background.</summary>
    public static Color DarkBackground { get; } = Color.FromRgb(0x1C, 0x19, 0x16);
}
