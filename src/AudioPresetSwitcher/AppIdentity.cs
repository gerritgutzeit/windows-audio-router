namespace AudioPresetSwitcher;

/// <summary>
/// Central product identity strings used for IPC, tray, settings, and shell integration.
/// </summary>
public static class AppIdentity
{
    public const string Name = "AudioPresetSwitcher";

    public const string ExecutableFileName = "AudioPresetSwitcher.exe";

    public const string AppUserModelId = "AudioPresetSwitcher.Desktop";

    public const string PipeName = "AudioPresetSwitcher.ipc";

    public const string MutexName = @"Local\AudioPresetSwitcher";
}
