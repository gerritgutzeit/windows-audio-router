export const REPO_URL = 'https://github.com/gerritgutzeit/windows-audio-router'
export const RELEASES_URL = `${REPO_URL}/releases/latest`
export const DOWNLOAD_URL = `${REPO_URL}/releases/latest/download/AudioPresetSwitcher-win-Setup.exe`
export const SITE_URL = 'https://gerritgutzeit.github.io/windows-audio-router/'

export const NAV_LINKS = [
  { label: 'Product', href: '#product' },
  { label: 'Live', href: '#live' },
  { label: 'Get it', href: '#get' },
] as const

export const STAGES = [
  { id: 'top', label: 'Start' },
  { id: 'product', label: 'Product' },
  { id: 'live', label: 'Live' },
  { id: 'get', label: 'Get it' },
] as const
