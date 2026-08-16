using System.Windows;
using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Dialogs;
using AudioPresetSwitcher.Views.Dialogs;
using Microsoft.Win32;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class PresetsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IAudioDeviceService _audio;
    private readonly IPresetActivationService _activation;
    private readonly ShortcutService _shortcuts;
    private readonly IContentDialogService _dialogs;
    private readonly ISnackbarService _snackbar;

    public PresetsViewModel(
        ISettingsService settings,
        IAudioDeviceService audio,
        IPresetActivationService activation,
        ShortcutService shortcuts,
        IContentDialogService dialogs,
        ISnackbarService snackbar)
    {
        _settings = settings;
        _audio = audio;
        _activation = activation;
        _shortcuts = shortcuts;
        _dialogs = dialogs;
        _snackbar = snackbar;

        Reload();
        _settings.Changed += (_, _) => Application.Current?.Dispatcher.BeginInvoke(Reload);
        _audio.DevicesChanged += (_, _) => Application.Current?.Dispatcher.BeginInvoke(Reload);
        _audio.PresetActivated += (_, _) => Application.Current?.Dispatcher.BeginInvoke(Reload);
    }

    [ObservableProperty]
    private ObservableCollection<PresetCardViewModel> _presets = [];

    [ObservableProperty]
    private bool _hasPresets;

    [RelayCommand]
    private async Task Create() => await EditAsync(null);

    [RelayCommand]
    private async Task Edit(PresetCardViewModel? card)
    {
        if (card is null)
        {
            await EditAsync(null);
            return;
        }

        await EditAsync(card.Preset);
    }

    [RelayCommand]
    private void Duplicate(PresetCardViewModel? card)
    {
        if (card is null)
        {
            return;
        }

        var copy = card.Preset.Clone();
        copy.Name = $"{card.Preset.Name} copy";
        _settings.Update(s => s.Presets.Add(copy));
    }

    [RelayCommand]
    private async Task Delete(PresetCardViewModel? card)
    {
        if (card is null)
        {
            return;
        }

        var confirm = new ContentDialog
        {
            Title = "Delete preset",
            Content = $"Delete \"{card.Name}\"? This cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await _dialogs.ShowAsync(confirm, CancellationToken.None);
        if (result != ContentDialogResult.Primary)
        {
            return;
        }

        _settings.Update(s =>
        {
            s.Presets.RemoveAll(p => p.Id == card.Id);
            if (s.LastActivePresetId == card.Id)
            {
                s.LastActivePresetId = null;
            }
        });
    }

    [RelayCommand]
    private void CreateShortcut(PresetCardViewModel? card)
    {
        if (card is null)
        {
            return;
        }

        var dialog = CreateShortcutSaveDialog(card.Name);
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            WriteShortcutFile(dialog.FileName, card.Preset);
        }
        catch (Exception ex)
        {
            ShowCaution("Could not create shortcut", ex.Message);
        }
    }

    private async Task EditAsync(AudioPreset? existing)
    {
        var editor = CreateEditor(existing);
        var dialog = new ContentDialog
        {
            Title = existing is null ? "New preset" : "Edit preset",
            Content = new PresetEditorView { DataContext = editor },
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await _dialogs.ShowAsync(dialog, CancellationToken.None);
        if (result != ContentDialogResult.Primary)
        {
            return;
        }

        if (!editor.TryBuild(existing?.Id ?? Guid.NewGuid(), out var preset, out var error))
        {
            ShowCaution("Could not save preset", error);
            return;
        }

        SavePreset(existing, preset);
    }

    private PresetEditorViewModel CreateEditor(AudioPreset? existing)
    {
        var editor = new PresetEditorViewModel(_audio)
        {
            Name = existing?.Name ?? "New preset",
            PlaybackKeyword = existing?.PlaybackKeyword ?? string.Empty,
            RecordingKeyword = existing?.RecordingKeyword ?? string.Empty,
            Icon = existing?.Icon ?? PresetIcon.Headphones
        };
        editor.LoadDevices(existing);
        return editor;
    }

    private void SavePreset(AudioPreset? existing, AudioPreset preset)
    {
        _settings.Update(s =>
        {
            if (existing is null)
            {
                s.Presets.Add(preset);
                return;
            }

            var index = s.Presets.FindIndex(p => p.Id == existing.Id);
            if (index >= 0)
            {
                s.Presets[index] = preset;
            }
            else
            {
                s.Presets.Add(preset);
            }
        });
    }

    private static SaveFileDialog CreateShortcutSaveDialog(string presetName) =>
        new()
        {
            Title = "Create shortcut",
            Filter = "Shortcut (*.lnk)|*.lnk",
            DefaultExt = ".lnk",
            AddExtension = true,
            FileName = ShortcutService.SanitizeFileName(presetName),
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            OverwritePrompt = true
        };

    private void WriteShortcutFile(string selectedPath, AudioPreset preset)
    {
        var exe = _shortcuts.ResolveExecutablePath();
        if (string.IsNullOrWhiteSpace(exe) || !File.Exists(exe))
        {
            ShowCaution("Could not create shortcut", "Application executable was not found.");
            return;
        }

        var path = selectedPath;
        if (!path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
        {
            path += ".lnk";
        }

        _shortcuts.CreateShortcut(
            path,
            exe,
            ShortcutService.FormatPresetArguments(preset.Name),
            $"Activate preset \"{preset.Name}\"");

        _snackbar.Show(
            "Shortcut created",
            Path.GetFileName(path),
            ControlAppearance.Success,
            null,
            TimeSpan.FromSeconds(3));
    }

    private void ShowCaution(string title, string message) =>
        _snackbar.Show(title, message, ControlAppearance.Caution, null, TimeSpan.FromSeconds(4));

    private void Reload()
    {
        Presets = new ObservableCollection<PresetCardViewModel>(
            _settings.Current.Presets.Select(preset =>
                new PresetCardViewModel(preset, _audio, _settings, _activation)));
        HasPresets = Presets.Count > 0;
    }
}
