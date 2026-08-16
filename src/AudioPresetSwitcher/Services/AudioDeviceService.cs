using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using AudioPresetSwitcher.Models;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Services;

public sealed class AudioDeviceService : IAudioDeviceService
{
    private readonly MMDeviceEnumerator _enumerator = new();
    private readonly AudioNotificationClient _notificationClient = new();
    private readonly object _gate = new();
    private readonly List<MMDevice> _cached = [];
    private readonly DispatcherTimer _debounce = new() { Interval = AppTiming.DeviceChangeDebounce };
    private bool _disposed;

    public AudioDeviceService()
    {
        _notificationClient.Attach(OnNativeDevicesChanged);
        _enumerator.RegisterEndpointNotificationCallback(_notificationClient);
        _debounce.Tick += (_, _) =>
        {
            _debounce.Stop();
            RefreshCache();
            DevicesChanged?.Invoke(this, EventArgs.Empty);
        };
        RefreshCache();
    }

    public event EventHandler? DevicesChanged;

    public event EventHandler<PresetActivationResult>? PresetActivated;

    public IReadOnlyList<AudioDeviceInfo> GetPlaybackDevices() => GetDevices(DataFlow.Render);

    public IReadOnlyList<AudioDeviceInfo> GetRecordingDevices() => GetDevices(DataFlow.Capture);

    public IReadOnlyList<LiveDeviceInfo> GetLiveDevices()
    {
        lock (_gate)
        {
            var renderDefault = SafeDefault(DataFlow.Render, Role.Multimedia);
            var renderComms = SafeDefault(DataFlow.Render, Role.Communications);
            var captureDefault = SafeDefault(DataFlow.Capture, Role.Multimedia);
            var captureComms = SafeDefault(DataFlow.Capture, Role.Communications);

            return _cached.Select(device =>
            {
                var isRender = device.DataFlow == DataFlow.Render;
                TryReadVolume(device, out var volume, out var muted);
                TryReadPeak(device, out var peak);

                return new LiveDeviceInfo
                {
                    Id = device.ID,
                    FriendlyName = device.FriendlyName,
                    Flow = device.DataFlow,
                    IsDefaultMultimedia = device.ID == (isRender ? renderDefault : captureDefault),
                    IsDefaultCommunications = device.ID == (isRender ? renderComms : captureComms),
                    Volume = volume,
                    IsMuted = muted,
                    Peak = peak
                };
            }).ToList();
        }
    }

    public float GetPeak(string deviceId)
    {
        lock (_gate)
        {
            var device = _cached.FirstOrDefault(d => d.ID == deviceId);
            if (device is null)
            {
                return 0f;
            }

            TryReadPeak(device, out var peak);
            return peak;
        }
    }

    public AudioDeviceInfo? Resolve(string? keyword, DataFlow flow)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return null;
        }

        lock (_gate)
        {
            var match = MatchDevice(keyword, _cached.Where(d => d.DataFlow == flow), flow);
            return match is null ? null : ToInfo(match);
        }
    }

    public PresetActivationResult ActivatePreset(AudioPreset preset)
    {
        ArgumentNullException.ThrowIfNull(preset);

        MMDevice? playback;
        MMDevice? recording;
        lock (_gate)
        {
            playback = MatchDevice(preset.PlaybackKeyword, _cached.Where(d => d.DataFlow == DataFlow.Render), DataFlow.Render);
            recording = MatchDevice(preset.RecordingKeyword, _cached.Where(d => d.DataFlow == DataFlow.Capture), DataFlow.Capture);
        }

        TryActivateEndpoint(
            preset.PlaybackKeyword,
            playback,
            "playback",
            out var playbackSkipped,
            out var playbackOk,
            out var playbackError);

        TryActivateEndpoint(
            preset.RecordingKeyword,
            recording,
            "recording",
            out var recordingSkipped,
            out var recordingOk,
            out var recordingError);

        var result = new PresetActivationResult
        {
            Preset = preset,
            PlaybackSucceeded = playbackOk && !playbackSkipped,
            RecordingSucceeded = recordingOk && !recordingSkipped,
            PlaybackSkipped = playbackSkipped,
            RecordingSkipped = recordingSkipped,
            PlaybackError = playbackError,
            RecordingError = recordingError
        };

        PresetActivated?.Invoke(this, result);
        return result;
    }

    public static string GuessKeyword(string friendlyName)
    {
        if (string.IsNullOrWhiteSpace(friendlyName))
        {
            return string.Empty;
        }

        var match = Regex.Match(
            friendlyName.Trim(),
            @"^(?:Speakers|Headphones|Headset|Microphone|Mic|Line\s*In|Line\s*Out|Digital Audio(?: \(S/PDIF\))?)\s*\((.+)\)\s*$",
            RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : friendlyName.Trim();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _debounce.Stop();
        try
        {
            _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);
        }
        catch
        {
            // shutting down
        }

        lock (_gate)
        {
            foreach (var device in _cached)
            {
                device.Dispose();
            }

            _cached.Clear();
        }

        _enumerator.Dispose();
    }

    private IReadOnlyList<AudioDeviceInfo> GetDevices(DataFlow flow)
    {
        lock (_gate)
        {
            return _cached.Where(d => d.DataFlow == flow).Select(ToInfo).ToList();
        }
    }

    private void OnNativeDevicesChanged()
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return;
        }

        _ = dispatcher.BeginInvoke(() =>
        {
            if (_disposed)
            {
                return;
            }

            _debounce.Stop();
            _debounce.Start();
        }, DispatcherPriority.Background);
    }

    private void RefreshCache()
    {
        var next = new List<MMDevice>();
        try
        {
            next.AddRange(_enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active));
            next.AddRange(_enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active));
        }
        catch
        {
            foreach (var device in next)
            {
                device.Dispose();
            }

            return;
        }

        lock (_gate)
        {
            foreach (var device in _cached)
            {
                device.Dispose();
            }

            _cached.Clear();
            _cached.AddRange(next);
        }
    }

    private MMDevice? MatchDevice(string? keyword, IEnumerable<MMDevice> devices, DataFlow flow) =>
        DeviceKeywordMatcher.Match(keyword, devices, SafeDefault, flow);

    private string? SafeDefault(DataFlow flow, Role role)
    {
        try
        {
            using var device = _enumerator.GetDefaultAudioEndpoint(flow, role);
            return device.ID;
        }
        catch
        {
            return null;
        }
    }

    private static void TryActivateEndpoint(
        string? keyword,
        MMDevice? device,
        string kind,
        out bool skipped,
        out bool ok,
        out string? error)
    {
        skipped = string.IsNullOrWhiteSpace(keyword);
        ok = skipped;
        error = null;

        if (skipped)
        {
            return;
        }

        if (device is null)
        {
            error = $"{kind} device not found for \"{keyword}\"";
            return;
        }

        try
        {
            SetAllRoles(device.ID);
            ok = true;
        }
        catch (Exception ex)
        {
            error = $"could not set {kind} device: {ex.Message}";
        }
    }

    private static void TryReadVolume(MMDevice device, out float volume, out bool muted)
    {
        volume = 0f;
        muted = false;
        try
        {
            volume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            muted = device.AudioEndpointVolume.Mute;
        }
        catch
        {
            // Some endpoints do not expose volume.
        }
    }

    private static void TryReadPeak(MMDevice device, out float peak)
    {
        peak = 0f;
        try
        {
            peak = device.AudioMeterInformation.MasterPeakValue;
        }
        catch
        {
            // Some endpoints do not expose meters.
        }
    }

    private static void SetAllRoles(string deviceId)
    {
        PolicyConfigClient.SetDefaultEndpoint(deviceId, Role.Console);
        PolicyConfigClient.SetDefaultEndpoint(deviceId, Role.Multimedia);
        PolicyConfigClient.SetDefaultEndpoint(deviceId, Role.Communications);
    }

    private static AudioDeviceInfo ToInfo(MMDevice device) => new()
    {
        Id = device.ID,
        FriendlyName = device.FriendlyName,
        Flow = device.DataFlow
    };
}
