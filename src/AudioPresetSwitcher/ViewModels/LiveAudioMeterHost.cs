using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Services;

namespace AudioPresetSwitcher.ViewModels;

/// <summary>
/// Shared 150 ms peak timer and DevicesChanged → UI reload wiring for live audio views.
/// </summary>
internal sealed class LiveAudioMeterHost : IDisposable
{
    public static readonly TimeSpan PeakInterval = AppTiming.PeakMeterInterval;

    private readonly IAudioDeviceService _audio;
    private readonly Action _reload;
    private readonly Action _refreshPeaks;
    private readonly DispatcherTimer _timer;

    public LiveAudioMeterHost(IAudioDeviceService audio, Action reload, Action refreshPeaks)
    {
        _audio = audio;
        _reload = reload;
        _refreshPeaks = refreshPeaks;
        _timer = new DispatcherTimer { Interval = PeakInterval };
        _timer.Tick += (_, _) => _refreshPeaks();
        _audio.DevicesChanged += OnDevicesChanged;
    }

    public static double ToPercent(float peak) => peak * 100d;

    public void Start()
    {
        _reload();
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _audio.DevicesChanged -= OnDevicesChanged;
    }

    private void OnDevicesChanged(object? sender, EventArgs e)
    {
        Application.Current?.Dispatcher.BeginInvoke(_reload);
    }
}
