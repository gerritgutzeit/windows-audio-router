using System.Windows;
using AudioPresetSwitcher.Models;
using AudioPresetSwitcher.Services;
using AudioPresetSwitcher.ViewModels.Dialogs;
using AudioPresetSwitcher.Views.Dialogs;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher.ViewModels.Pages;

public partial class PresetsViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private readonly AudioDeviceService _audio;
    private readonly NotificationService _notifications;
    private readonly IContentDialogService _dialogs;
    private readonly ISnackbarService _snackbar;

    public PresetsViewModel(
        SettingsService settings,
        AudioDeviceService audio,
        NotificationService notifications,
        IContentDialogService dialogs,
        ISnackbarService snackbar)
    {
        _settings = settings;
        _audio = audio;
        _notifications = notifications;
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

    public async Task CreateAsync() => await EditAsync(null);

    [RelayCommand]
    private async Task Create() => await CreateAsync();

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

    private async Task EditAsync(AudioPreset? existing)
    {
        var editor = new PresetEditorViewModel(_audio)
        {
            Name = existing?.Name ?? "New preset",
            PlaybackKeyword = existing?.PlaybackKeyword ?? string.Empty,
            RecordingKeyword = existing?.RecordingKeyword ?? string.Empty,
            Icon = existing?.Icon ?? "Headphones"
        };
        editor.LoadDevices(existing);

        var view = new PresetEditorView { DataContext = editor };
        var dialog = new ContentDialog
        {
            Title = existing is null ? "New preset" : "Edit preset",
            Content = view,
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
            _snackbar.Show("Could not save preset", error, ControlAppearance.Caution, null, TimeSpan.FromSeconds(4));
            return;
        }

        _settings.Update(s =>
        {
            if (existing is null)
            {
                s.Presets.Add(preset);
            }
            else
            {
                var index = s.Presets.FindIndex(p => p.Id == existing.Id);
                if (index >= 0)
                {
                    s.Presets[index] = preset;
                }
                else
                {
                    s.Presets.Add(preset);
                }
            }
        });
    }

    private void Reload()
    {
        Presets = new ObservableCollection<PresetCardViewModel>(
            _settings.Current.Presets.Select(preset =>
                new PresetCardViewModel(preset, _audio, _settings, _notifications)));
        HasPresets = Presets.Count > 0;
    }
}
