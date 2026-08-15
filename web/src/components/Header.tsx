import { Menu, ToggleRight, X } from 'lucide-react'
import { useState } from 'react'
import { DOWNLOAD_URL, NAV_LINKS } from '../lib/constants'
import { Button } from './ui/Button'

export function Header() {
  const [open, setOpen] = useState(false)

  return (
    <header className="sticky top-0 z-50 border-b border-transparent bg-void/70 backdrop-blur-xl">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-3 sm:px-6">
        <a
          href="#top"
          className="flex items-center gap-2 text-sm font-semibold tracking-tight text-white"
        >
          <span className="flex h-8 w-8 items-center justify-center rounded-full border border-white/10 bg-white/[0.04]">
            <ToggleRight className="h-4 w-4 text-accent-bright" aria-hidden />
          </span>
          <span className="hidden sm:inline">AudioPresetSwitcher</span>
          <span className="sm:hidden">APS</span>
        </a>

        <nav
          className="hidden items-center rounded-full border border-white/10 bg-white/[0.03] p-1 backdrop-blur-md md:flex"
          aria-label="Primary"
        >
          {NAV_LINKS.map((link) => (
            <a
              key={link.href}
              href={link.href}
              className="rounded-full px-3.5 py-1.5 text-xs font-medium text-muted transition-colors hover:bg-white/5 hover:text-white"
            >
              {link.label}
            </a>
          ))}
        </nav>

        <div className="flex items-center gap-2">
          <Button
            href={DOWNLOAD_URL}
            variant="primary"
            className="hidden px-4 py-2 text-xs sm:inline-flex"
          >
            Get Windows App
            <span aria-hidden>↗</span>
          </Button>
          <button
            type="button"
            className="inline-flex h-9 w-9 items-center justify-center rounded-full border border-white/10 bg-white/[0.04] text-white md:hidden"
            aria-expanded={open}
            aria-label={open ? 'Close menu' : 'Open menu'}
            onClick={() => setOpen((v) => !v)}
          >
            {open ? <X className="h-4 w-4" /> : <Menu className="h-4 w-4" />}
          </button>
        </div>
      </div>

      {open && (
        <div className="border-t border-white/10 bg-void/95 px-4 py-4 backdrop-blur-xl md:hidden">
          <nav className="flex flex-col gap-1" aria-label="Mobile">
            {NAV_LINKS.map((link) => (
              <a
                key={link.href}
                href={link.href}
                className="rounded-lg px-3 py-2.5 text-sm text-muted hover:bg-white/5 hover:text-white"
                onClick={() => setOpen(false)}
              >
                {link.label}
              </a>
            ))}
            <Button href={DOWNLOAD_URL} className="mt-2 w-full">
              Get Windows App ↗
            </Button>
          </nav>
        </div>
      )}
    </header>
  )
}
