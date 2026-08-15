using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.ViewModels.Dialogs;

public partial class PresetEditorViewModel : ObservableObject
{
    private readonly AudioDeviceService _audio;
    private bool _suppressKeywordSync;

    public PresetEditorViewModel(AudioDeviceService audio)
    {
        _audio = audio;
    }

    [ObservableProperty]
    private string _name = "New preset";

    [ObservableProperty]
    private string _icon = "Headphones";

    [ObservableProperty]
    private string _playbackKeyword = string.Empty;

    [ObservableProperty]
    private string _recordingKeyword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<AudioDeviceInfo> _playbackDevices = [];

    [ObservableProperty]
    private ObservableCollection<AudioDeviceInfo> _recordingDevices = [];

    [ObservableProperty]
    private AudioDeviceInfo? _selectedPlaybackDevice;

    [ObservableProperty]
    private AudioDeviceInfo? _selectedRecordingDevice;

    public string[] IconOptions { get; } = ["Headphones", "Speaker"];

    public void LoadDevices(AudioPreset? existing)
    {
        _suppressKeywordSync = true;
        PlaybackDevices = new ObservableCollection<AudioDeviceInfo>(_audio.GetPlaybackDevices());
        RecordingDevices = new ObservableCollection<AudioDeviceInfo>(_audio.GetRecordingDevices());
        SelectedPlaybackDevice = Match(PlaybackDevices, existing?.PlaybackKeyword, DataFlow.Render);
        SelectedRecordingDevice = Match(RecordingDevices, existing?.RecordingKeyword, DataFlow.Capture);
        _suppressKeywordSync = false;
    }

    partial void OnSelectedPlaybackDeviceChanged(AudioDeviceInfo? value)
    {
        if (_suppressKeywordSync || value is null)
        {
            return;
        }

        PlaybackKeyword = AudioDeviceService.GuessKeyword(value.FriendlyName);
    }

    partial void OnSelectedRecordingDeviceChanged(AudioDeviceInfo? value)
    {
        if (_suppressKeywordSync || value is null)
        {
            return;
        }

        RecordingKeyword = AudioDeviceService.GuessKeyword(value.FriendlyName);
    }

    public bool TryBuild(Guid id, out AudioPreset preset, out string error)
    {
        preset = new AudioPreset();
        if (string.IsNullOrWhiteSpace(Name))
        {
            error = "Give this preset a name.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(PlaybackKeyword) && string.IsNullOrWhiteSpace(RecordingKeyword))
        {
            error = "Choose at least a playback or recording device.";
            return false;
        }

        preset = new AudioPreset
        {
            Id = id,
            Name = Name.Trim(),
            Icon = Icon,
            PlaybackKeyword = PlaybackKeyword.Trim(),
            RecordingKeyword = RecordingKeyword.Trim()
        };
        error = string.Empty;
        return true;
    }

    private AudioDeviceInfo? Match(IEnumerable<AudioDeviceInfo> devices, string? keyword, DataFlow flow)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return null;
        }

        return _audio.Resolve(keyword, flow)
               ?? devices.FirstOrDefault(d => d.FriendlyName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}
