using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace AudioPresetSwitcher.Services;

internal sealed class AudioNotificationClient : IMMNotificationClient
{
    private Action? _onChanged;

    public void Attach(Action onChanged) => _onChanged = onChanged;

    public void OnDeviceStateChanged(string deviceId, DeviceState newState) => Notify();

    public void OnDeviceAdded(string pwstrDeviceId) => Notify();

    public void OnDeviceRemoved(string deviceId) => Notify();

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) => Notify();

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {
        // Volume/peak property spam; ignore.
    }

    private void Notify()
    {
        var handler = _onChanged;
        if (handler is null)
        {
            return;
        }

        ThreadPool.QueueUserWorkItem(_ => handler());
    }
}
