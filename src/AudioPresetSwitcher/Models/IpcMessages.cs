namespace AudioPresetSwitcher.Models;

public sealed class IpcRequest
{
    public string? Preset { get; set; }

    public int? PresetIndex { get; set; }

    public bool Show { get; set; }
}

public sealed class IpcResponse
{
    public bool Ok { get; set; }

    public string Message { get; set; } = string.Empty;

    public int ExitCode { get; set; }
}
