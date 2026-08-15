export const REPO_URL = 'https://github.com/gerritgutzeit/windows-audio-router'
export const RELEASES_URL = `${REPO_URL}/releases/latest`
export const DOWNLOAD_URL = `${REPO_URL}/releases/latest/download/AudioPresetSwitcher-win-Setup.exe`
export const SITE_URL = 'https://gerritgutzeit.github.io/windows-audio-router/'

export const CLI_SNIPPET = `# Activate preset by name
AudioPresetSwitcher.exe --preset "Desk Studio"

# Short flag
AudioPresetSwitcher.exe -p "Desk Studio"

# Activate by zero-based index
AudioPresetSwitcher.exe --preset-index 0`

export const NAV_LINKS = [
  { label: 'Presets', href: '#presets' },
  { label: 'Live Monitor', href: '#live-monitor' },
  { label: 'Automation', href: '#automation' },
  { label: 'Docs', href: '#docs' },
] as const
