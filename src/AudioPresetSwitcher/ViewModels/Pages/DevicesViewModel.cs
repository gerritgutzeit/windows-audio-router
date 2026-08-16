using AudioPresetSwitcher.Services;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class DevicesViewModel : ObservableObject, IDisposable
{
    private readonly IAudioDeviceService _audio;
    private readonly LiveAudioMeterHost _meterHost;

    public DevicesViewModel(IAudioDeviceService audio)
    {
        _audio = audio;
        _meterHost = new LiveAudioMeterHost(audio, Reload, RefreshPeaks);
        _meterHost.Start();
    }

    [ObservableProperty]
    private ObservableCollection<LiveDeviceCardViewModel> _playbackDevices = [];

    [ObservableProperty]
    private ObservableCollection<LiveDeviceCardViewModel> _recordingDevices = [];

    public void Dispose() => _meterHost.Dispose();

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
            device.Peak = LiveAudioMeterHost.ToPercent(_audio.GetPeak(device.Id));
        }
    }
}
