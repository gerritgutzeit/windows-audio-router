# AudioPresetSwitcher

Windows 11 system-tray app that switches default playback and recording devices with one click. Presets are edited in a Fluent dashboard — never in a JSON file.

Closing the window hides the app to the tray. Exit is only available from the sidebar footer or the tray menu.

![AudioPresetSwitcher dashboard](docs/images/Screenshot.png)

## Features

- Card-based preset manager (create, edit, duplicate, delete, activate)
- Live device status with volume and peak meters
- Dark / light / system theme, Mica backdrop
- Tray menu with a checkmark on the active preset
- CLI for Stream Deck and other automation
- Optional Windows toast when a preset is applied
- Optional start with Windows

Devices are matched by a **keyword** against the current Windows `FriendlyName` (for example `Arctis Nova` or `Shure MV7`). That survives USB/Bluetooth reconnects, which change the internal device GUID.

Applying a preset sets multimedia **and** communications defaults (Teams, Discord, and similar) to the same playback and recording devices.

## Requirements

- Windows 10 1809 or later (Windows 11 recommended)
- For running a Release: nothing else — the published EXE is self-contained
- For building from source: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Install from GitHub Releases

1. Open the [Releases](../../releases) page.
2. Download `AudioPresetSwitcher-win-x64.zip` or `AudioPresetSwitcher.exe`.
3. Put the EXE somewhere stable (for example `%LOCALAPPDATA%\Programs\AudioPresetSwitcher`) and run it.

Windows SmartScreen may warn on the first launch because the binary is not code-signed. Choose **More info** → **Run anyway** if you built or downloaded it from this repository.

Settings live in:

```text
%AppData%\AudioPresetSwitcher\settings.json
```

That file is created automatically. Do not edit it by hand unless you know what you are doing.

## Usage

### Dashboard

| Page | What it does |
| --- | --- |
| **Presets** | List of presets. **Activate** applies devices immediately. Chevron → Edit / Duplicate / Delete. |
| **Live status** | Connected playback and recording devices, volume, live levels, default-role badges. |
| **Settings** | Run at startup, theme, toast notifications. |

In the preset editor, pick devices from the dropdowns, then optionally shorten the **match keyword** so it still matches after Windows renames the endpoint slightly.

### System tray

- Left-click: open the dashboard
- Right-click: activate a preset, open the dashboard, or exit

### Command line

If the tray app is already running, a second process forwards the command over a local named pipe and exits. If it is not running, the first instance applies the preset and stays in the tray (no window).

```text
AudioPresetSwitcher.exe --preset "Desk"
AudioPresetSwitcher.exe -p "Desk"
AudioPresetSwitcher.exe --preset-index 0
```

| Flag | Meaning | Exit code |
| --- | --- | --- |
| `--preset` / `-p` | Preset name (case-insensitive) | `0` on success, `1` if missing or switch failed |
| `--preset-index` | Zero-based index in the saved list | same |

Stream Deck: add a **System → Open** action pointing at the EXE, with arguments `--preset "Headset"`.

## Build from source

```powershell
dotnet restore AudioPresetSwitcher.sln
dotnet build AudioPresetSwitcher.sln -c Release
```

Self-contained single-file EXE (same output GitHub Actions attaches to a Release):

```powershell
dotnet publish src/AudioPresetSwitcher/AudioPresetSwitcher.csproj -c Release -p:PublishProfile=win-x64
```

Output: `publish/win-x64/AudioPresetSwitcher.exe`

## How matching works

Each preset stores keywords, not GUIDs.

1. Enumerate active WASAPI endpoints.
2. Keep names that contain the keyword (case-insensitive substring).
3. Prefer an exact name match, then the tightest substring ratio, then the current default if several still tie.
4. Set Console, Multimedia, and Communications roles for the matched playback and recording devices.

If a side has an empty keyword, that side is left unchanged. If a keyword matches nothing, that side is skipped and a toast explains which keyword failed.

## Tech stack

- C# / .NET 8 / WPF
- [WPF-UI](https://github.com/lepoco/wpfui) (Fluent, Mica)
- [H.NotifyIcon.Wpf](https://github.com/HavenDV/H.NotifyIcon)
- [NAudio](https://github.com/naudio/NAudio) (WASAPI enumerate, meters, device watch)
