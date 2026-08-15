import { Download } from 'lucide-react'
import { DOWNLOAD_URL, REPO_URL } from '../lib/constants'
import { Button } from './ui/Button'

export function Footer() {
  return (
    <footer className="border-t border-white/8 px-4 pb-16 pt-20 sm:px-6">
      <div className="mx-auto max-w-3xl text-center">
        <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
          Ready to streamline your audio setups?
        </h2>
        <p className="mt-3 text-muted">
          Download the Velopack installer and switch presets from the tray or
          CLI.
        </p>
        <div className="mt-8 flex justify-center">
          <Button href={DOWNLOAD_URL} className="px-6 py-3">
            <Download className="h-4 w-4" />
            Download for Windows
          </Button>
        </div>
        <div className="mt-6 flex flex-wrap items-center justify-center gap-2">
          {['MIT License', 'Open Source', '.NET 8 Native'].map((badge) => (
            <span
              key={badge}
              className="rounded-full border border-white/10 bg-white/[0.03] px-3 py-1 text-xs text-muted"
            >
              {badge}
            </span>
          ))}
        </div>
      </div>

      <div className="mx-auto mt-16 flex max-w-6xl flex-col items-center justify-between gap-4 border-t border-white/8 pt-8 text-xs text-muted sm:flex-row">
        <p>AudioPresetSwitcher</p>
        <div className="flex gap-4">
          <a href={REPO_URL} className="hover:text-white">
            GitHub
          </a>
          <a href={`${REPO_URL}/releases/latest`} className="hover:text-white">
            Releases
          </a>
          <a href={`${REPO_URL}/blob/main/README.md`} className="hover:text-white">
            Documentation
          </a>
        </div>
      </div>
    </footer>
  )
}
