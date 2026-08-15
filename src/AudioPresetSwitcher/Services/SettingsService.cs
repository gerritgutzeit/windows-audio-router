using System.Text.Json;
using System.Text.Json.Serialization;
using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly object _gate = new();
    private readonly string _filePath;
    private CancellationTokenSource? _debounce;

    public SettingsService()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AudioPresetSwitcher");
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, "settings.json");
        Current = Load();
    }

    public AppSettings Current { get; private set; }

    public event EventHandler? Changed;

    public void Update(Action<AppSettings> mutate)
    {
        lock (_gate)
        {
            mutate(Current);
        }

        Changed?.Invoke(this, EventArgs.Empty);
        ScheduleSave();
    }

    public void SaveNow()
    {
        AppSettings snapshot;
        lock (_gate)
        {
            snapshot = Clone(Current);
        }

        var json = JsonSerializer.Serialize(snapshot, JsonOptions);
        var temp = _filePath + ".tmp";
        File.WriteAllText(temp, json);
        File.Copy(temp, _filePath, overwrite: true);
        File.Delete(temp);
    }

    private void ScheduleSave()
    {
        _debounce?.Cancel();
        _debounce?.Dispose();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, token);
                SaveNow();
            }
            catch (OperationCanceledException)
            {
                // superseded by a newer change
            }
        }, token);
    }

    private AppSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    private static AppSettings Clone(AppSettings source)
    {
        var json = JsonSerializer.Serialize(source, JsonOptions);
        return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
    }
}
