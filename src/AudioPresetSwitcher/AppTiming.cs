namespace AudioPresetSwitcher;

/// <summary>
/// Shared timing intervals. Values must stay identical to preserve UI/device refresh behavior.
/// </summary>
public static class AppTiming
{
    public static readonly TimeSpan PeakMeterInterval = TimeSpan.FromMilliseconds(150);

    public static readonly TimeSpan DeviceChangeDebounce = TimeSpan.FromMilliseconds(250);
}
