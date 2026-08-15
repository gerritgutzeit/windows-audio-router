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

public partial class LiveDeviceCardViewModel : ObservableObject
{
    public LiveDeviceCardViewModel(LiveDeviceInfo info)
    {
        Id = info.Id;
        Name = info.FriendlyName;
        IsPlayback = info.Flow == DataFlow.Render;
        IsDefaultMultimedia = info.IsDefaultMultimedia;
        IsDefaultCommunications = info.IsDefaultCommunications;
        VolumePercent = info.IsMuted ? 0 : Math.Round(info.Volume * 100);
        IsMuted = info.IsMuted;
        Peak = info.Peak * 100d;
        StatusText = info.IsMuted ? "Muted" : $"Volume {VolumePercent:0}%";
        IconSymbol = IsPlayback ? Wpf.Ui.Controls.SymbolRegular.Speaker224 : Wpf.Ui.Controls.SymbolRegular.Mic24;
    }

    public string Id { get; }

    public string Name { get; }

    public bool IsPlayback { get; }

    public bool IsDefaultMultimedia { get; }

    public bool IsDefaultCommunications { get; }

    public double VolumePercent { get; }

    public bool IsMuted { get; }

    public string StatusText { get; }

    public Wpf.Ui.Controls.SymbolRegular IconSymbol { get; }

    public string DefaultBadge =>
        IsPlayback
            ? IsDefaultMultimedia ? "Default Playback" : string.Empty
            : IsDefaultMultimedia ? "Default Mic" : string.Empty;

    public string CommunicationsBadge => IsDefaultCommunications ? "Default Communications" : string.Empty;

    public bool ShowDefaultBadge => !string.IsNullOrEmpty(DefaultBadge);

    public bool ShowCommunicationsBadge => !string.IsNullOrEmpty(CommunicationsBadge);

    [ObservableProperty]
    private double _peak;
}
