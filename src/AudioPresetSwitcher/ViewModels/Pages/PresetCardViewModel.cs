using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using NAudio.CoreAudioApi;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class PresetCardViewModel : ObservableObject
{
    private readonly IAudioDeviceService _audio;
    private readonly ISettingsService _settings;
    private readonly IPresetActivationService _activation;

    public PresetCardViewModel(
        AudioPreset preset,
        IAudioDeviceService audio,
        ISettingsService settings,
        IPresetActivationService activation)
    {
        Preset = preset;
        _audio = audio;
        _settings = settings;
        _activation = activation;
        Refresh();
    }

    public AudioPreset Preset { get; }

    public Guid Id => Preset.Id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private bool _playbackFound = true;

    [ObservableProperty]
    private bool _recordingFound = true;

    public SymbolRegular IconSymbol => PresetIconGlyphs.ToSymbol(Preset.Icon);

    public void Refresh()
    {
        Name = Preset.Name;
        IsActive = _settings.Current.LastActivePresetId == Preset.Id;

        var playback = _audio.Resolve(Preset.PlaybackKeyword, DataFlow.Render);
        var recording = _audio.Resolve(Preset.RecordingKeyword, DataFlow.Capture);
        PlaybackFound = string.IsNullOrWhiteSpace(Preset.PlaybackKeyword) || playback is not null;
        RecordingFound = string.IsNullOrWhiteSpace(Preset.RecordingKeyword) || recording is not null;

        var playbackText = string.IsNullOrWhiteSpace(Preset.PlaybackKeyword)
            ? "No playback device"
            : playback?.FriendlyName ?? "Playback device not found";
        var recordingText = string.IsNullOrWhiteSpace(Preset.RecordingKeyword)
            ? "No recording device"
            : recording?.FriendlyName ?? "Recording device not found";
        Subtitle = $"{playbackText}  ·  {recordingText}";
    }

    [RelayCommand]
    private void Activate()
    {
        _activation.ActivateAndRemember(Preset);
        Refresh();
    }
}
