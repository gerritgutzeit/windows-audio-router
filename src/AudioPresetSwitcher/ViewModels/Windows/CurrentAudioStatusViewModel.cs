using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.Views.Pages;
using NAudio.CoreAudioApi;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Windows;

public partial class CurrentAudioStatusViewModel : ObservableObject, IDisposable
{
    private readonly AudioDeviceService _audio;
    private readonly INavigationService _navigation;
    private readonly DispatcherTimer _timer;

    public CurrentAudioStatusViewModel(AudioDeviceService audio, INavigationService navigation)
    {
        _audio = audio;
        _navigation = navigation;
        Playback = CurrentEndpointStatusViewModel.Missing(isPlayback: true);
        Recording = CurrentEndpointStatusViewModel.Missing(isPlayback: false);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _timer.Tick += (_, _) => RefreshPeaks();
        _audio.DevicesChanged += OnDevicesChanged;
        Reload();
        _timer.Start();
    }

    [ObservableProperty]
    private CurrentEndpointStatusViewModel _playback;

    [ObservableProperty]
    private CurrentEndpointStatusViewModel _recording;

    [RelayCommand]
    private void OpenLiveStatus() => _navigation.Navigate(typeof(DevicesPage));

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
        var playback = live.FirstOrDefault(d => d.Flow == DataFlow.Render && d.IsDefaultMultimedia);
        var recording = live.FirstOrDefault(d => d.Flow == DataFlow.Capture && d.IsDefaultMultimedia);
        Playback = FromDevice(playback, isPlayback: true);
        Recording = FromDevice(recording, isPlayback: false);
    }

    private void RefreshPeaks()
    {
        UpdatePeak(Playback);
        UpdatePeak(Recording);
    }

    private void UpdatePeak(CurrentEndpointStatusViewModel endpoint)
    {
        if (!endpoint.HasDevice || endpoint.Id is null)
        {
            endpoint.Peak = 0;
            return;
        }

        endpoint.Peak = _audio.GetPeak(endpoint.Id) * 100d;
    }

    private static CurrentEndpointStatusViewModel FromDevice(LiveDeviceInfo? info, bool isPlayback) =>
        info is null
            ? CurrentEndpointStatusViewModel.Missing(isPlayback)
            : CurrentEndpointStatusViewModel.From(info);
}

public partial class CurrentEndpointStatusViewModel : ObservableObject
{
    public static CurrentEndpointStatusViewModel Missing(bool isPlayback) => new()
    {
        Id = null,
        Name = isPlayback ? "No playback device" : "No microphone",
        HasDevice = false,
        IsMuted = false,
        Peak = 0,
        IconSymbol = isPlayback ? SymbolRegular.Speaker224 : SymbolRegular.Mic24
    };

    public static CurrentEndpointStatusViewModel From(LiveDeviceInfo info) => new()
    {
        Id = info.Id,
        Name = info.FriendlyName,
        HasDevice = true,
        IsMuted = info.IsMuted,
        Peak = info.Peak * 100d,
        IconSymbol = info.Flow == DataFlow.Render ? SymbolRegular.Speaker224 : SymbolRegular.Mic24
    };

    public string? Id { get; init; }

    public required string Name { get; init; }

    public bool HasDevice { get; init; }

    public bool IsMuted { get; init; }

    public SymbolRegular IconSymbol { get; init; }

    [ObservableProperty]
    private double _peak;
}
