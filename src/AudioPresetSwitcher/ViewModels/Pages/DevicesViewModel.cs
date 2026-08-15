using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class DevicesViewModel : ObservableObject, IDisposable
{
    private readonly AudioDeviceService _audio;
    private readonly DispatcherTimer _timer;

    public DevicesViewModel(AudioDeviceService audio)
    {
        _audio = audio;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _timer.Tick += (_, _) => RefreshPeaks();
        _audio.DevicesChanged += OnDevicesChanged;
        Reload();
        _timer.Start();
    }

    [ObservableProperty]
    private ObservableCollection<LiveDeviceCardViewModel> _playbackDevices = [];

    [ObservableProperty]
    private ObservableCollection<LiveDeviceCardViewModel> _recordingDevices = [];

    public void Dispose()
    {
        _timer.Stop();
        _audio.DevicesChanged -= OnDevicesChanged;
    }

    private void OnDevicesChanged(object? sender, EventArgs e)
    {
        Application.Current?.Dispatcher.BeginInvoke(Reload);
    }

    private void Reload()
    {
        var live = _audio.GetLiveDevices();
        PlaybackDevices = new ObservableCollection<LiveDeviceCardViewModel>(
            live.Where(d => d.Flow == DataFlow.Render).Select(d => new LiveDeviceCardViewModel(d)));
        RecordingDevices = new ObservableCollection<LiveDeviceCardViewModel>(
            live.Where(d => d.Flow == DataFlow.Capture).Select(d => new LiveDeviceCardViewModel(d)));
    }

    private void RefreshPeaks()
    {
        foreach (var device in PlaybackDevices.Concat(RecordingDevices))
        {
            device.Peak = _audio.GetPeak(device.Id) * 100d;
        }
    }
}
