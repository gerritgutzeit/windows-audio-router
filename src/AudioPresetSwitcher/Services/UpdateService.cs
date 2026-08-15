using System.Reflection;
using System.Windows;
using Velopack;
using Velopack.Sources;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.Services;

public sealed class UpdateService
{
    private const string RepoUrl = "https://github.com/gerritgutzeit/windows-audio-router";

    private readonly ISnackbarService _snackbar;
    private readonly UpdateManager _manager;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private UpdateInfo? _pending;

    public UpdateService(ISnackbarService snackbar)
    {
        _snackbar = snackbar;
        _manager = new UpdateManager(new GithubSource(RepoUrl, null, false));
    }

    public bool IsInstalled => _manager.IsInstalled;

    public bool UpdateReady => _pending is not null || _manager.UpdatePendingRestart is not null;

    public string CurrentVersionText
    {
        get
        {
            if (_manager.CurrentVersion is not null)
            {
                return _manager.CurrentVersion.ToString();
            }

            return Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
        }
    }

    public async Task CheckAndDownloadInBackgroundAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInstalled)
        {
            return;
        }

        try
        {
            var ready = await CheckAndDownloadAsync(showStatus: false, cancellationToken);
            if (ready)
            {
                await ShowSnackbarAsync(
                    "Update ready",
                    "Restart from Settings to install the new version.",
                    ControlAppearance.Info);
            }
        }
        catch
        {
            // Background checks should stay quiet.
        }
    }

    public async Task<bool> CheckAndDownloadAsync(bool showStatus, CancellationToken cancellationToken = default)
    {
        if (!IsInstalled)
        {
            if (showStatus)
            {
                await ShowSnackbarAsync(
                    "Updates unavailable",
                    "Install via Setup.exe from GitHub Releases to enable auto-updates.",
                    ControlAppearance.Secondary);
            }

            return false;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_manager.UpdatePendingRestart is not null || _pending is not null)
            {
                if (showStatus)
                {
                    await ShowSnackbarAsync(
                        "Update ready",
                        "Restart from Settings to install the new version.",
                        ControlAppearance.Info);
                }

                return true;
            }

            var update = await _manager.CheckForUpdatesAsync();
            if (update is null)
            {
                if (showStatus)
                {
                    await ShowSnackbarAsync(
                        "You're up to date",
                        $"Version {CurrentVersionText} is the latest.",
                        ControlAppearance.Success);
                }

                return false;
            }

            if (showStatus)
            {
                await ShowSnackbarAsync(
                    "Downloading update",
                    $"Version {update.TargetFullRelease.Version}…",
                    ControlAppearance.Info);
            }

            await _manager.DownloadUpdatesAsync(update, cancelToken: cancellationToken);
            _pending = update;

            if (showStatus)
            {
                await ShowSnackbarAsync(
                    "Update ready",
                    "Restart from Settings to install the new version.",
                    ControlAppearance.Info);
            }

            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void ApplyAndRestart()
    {
        if (!IsInstalled)
        {
            return;
        }

        var asset = _pending?.TargetFullRelease ?? _manager.UpdatePendingRestart;
        _manager.ApplyUpdatesAndRestart(asset);
    }

    private Task ShowSnackbarAsync(string title, string message, ControlAppearance appearance)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return Task.CompletedTask;
        }

        return dispatcher.InvokeAsync(() =>
            _snackbar.Show(title, message, appearance, null, TimeSpan.FromSeconds(5))).Task;
    }
}
