using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Windows;
using AudioPresetSwitcher.Models;
using Microsoft.Extensions.Hosting;

namespace AudioPresetSwitcher.Services;

public sealed class IpcService : IHostedService, IDisposable
{
    public const string PipeName = AppIdentity.PipeName;
    public const string MutexName = AppIdentity.MutexName;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IPresetActivationService _activation;
    private readonly IWindowService _windows;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;

    public IpcService(
        IPresetActivationService activation,
        IWindowService windows)
    {
        _activation = activation;
        _windows = windows;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _listenTask = Task.Run(() => ListenAsync(_cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts is null)
        {
            return;
        }

        await _cts.CancelAsync();
        if (_listenTask is not null)
        {
            try
            {
                await _listenTask.WaitAsync(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (TimeoutException)
            {
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    public void Dispose() => _cts?.Dispose();

    public static int Send(StartupOptions options)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
            client.Connect(2000);
            var request = new IpcRequest
            {
                Preset = options.PresetName,
                PresetIndex = options.PresetIndex,
                Show = options.ShowWindow
            };
            WriteMessage(client, request);
            var response = ReadMessage<IpcResponse>(client);
            return response?.ExitCode ?? 1;
        }
        catch
        {
            return 1;
        }
    }

    public IpcResponse Handle(IpcRequest request)
    {
        if (request.Show && !request.PresetIndex.HasValue && string.IsNullOrWhiteSpace(request.Preset))
        {
            _windows.ShowDashboard();
            return new IpcResponse { Ok = true, Message = "Opened dashboard", ExitCode = 0 };
        }

        var result = _activation.ActivateFromRequest(request.Preset, request.PresetIndex);
        if (result is null)
        {
            return new IpcResponse { Ok = false, Message = "Preset not found", ExitCode = 1 };
        }

        if (request.Show)
        {
            _windows.ShowDashboard();
        }

        return new IpcResponse
        {
            Ok = result.AnySuccess,
            Message = result.Summary,
            ExitCode = result.AnySuccess ? 0 : 1
        };
    }

    private async Task ListenAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(
                    PipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                await server.WaitForConnectionAsync(token);
                var request = ReadMessage<IpcRequest>(server) ?? new IpcRequest { Show = true };
                var response = Application.Current is null
                    ? new IpcResponse { Ok = false, Message = "Application is shutting down", ExitCode = 1 }
                    : await Application.Current.Dispatcher.InvokeAsync(() => Handle(request)).Task;
                WriteMessage(server, response);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                try
                {
                    await Task.Delay(200, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private static void WriteMessage<T>(Stream stream, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions) + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
    }

    private static T? ReadMessage<T>(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var line = reader.ReadLine();
        return string.IsNullOrWhiteSpace(line) ? default : JsonSerializer.Deserialize<T>(line, JsonOptions);
    }
}
