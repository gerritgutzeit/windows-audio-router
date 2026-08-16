using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.ViewModels;
using NAudio.CoreAudioApi;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class LiveDeviceCardViewModel : ObservableObject
{
    public LiveDeviceCardViewModel(LiveDeviceInfo info)
    {
        Id = info.Id;
        Name = info.FriendlyName;
        IsPlayback = info.Flow == DataFlow.Render;
        IsDefaultMultimedia = info.IsDefaultMultimedia;
        IsDefaultCommunications = info.IsDefaultCommunications;
        VolumePercent = info.IsMuted ? 0 : Math.Round(LiveAudioMeterHost.ToPercent(info.Volume));
        IsMuted = info.IsMuted;
        Peak = LiveAudioMeterHost.ToPercent(info.Peak);
        StatusText = info.IsMuted ? "Muted" : $"Volume {VolumePercent:0}%";
        IconSymbol = IsPlayback ? SymbolRegular.Speaker224 : SymbolRegular.Mic24;
    }

    public string Id { get; }

    public string Name { get; }

    public bool IsPlayback { get; }

    public bool IsDefaultMultimedia { get; }

    public bool IsDefaultCommunications { get; }

    public double VolumePercent { get; }

    public bool IsMuted { get; }

    public string StatusText { get; }

    public SymbolRegular IconSymbol { get; }

    public bool ShowDefaultBadge => IsDefaultMultimedia;

    public bool ShowCommunicationsBadge => IsDefaultCommunications;

    public bool ShowRoleChips => ShowDefaultBadge || ShowCommunicationsBadge;

    [ObservableProperty]
    private double _peak;
}
