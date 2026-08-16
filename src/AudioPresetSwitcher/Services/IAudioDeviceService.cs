using AudioPresetSwitcher.Models;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Services;

public interface IAudioDeviceService : IDisposable
{
    event EventHandler? DevicesChanged;

    event EventHandler<PresetActivationResult>? PresetActivated;

    IReadOnlyList<AudioDeviceInfo> GetPlaybackDevices();

    IReadOnlyList<AudioDeviceInfo> GetRecordingDevices();

    IReadOnlyList<LiveDeviceInfo> GetLiveDevices();

    float GetPeak(string deviceId);

    AudioDeviceInfo? Resolve(string? keyword, DataFlow flow);

    PresetActivationResult ActivatePreset(AudioPreset preset);
}
