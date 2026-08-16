import { Menu, X } from 'lucide-react'
import { useEffect, useState } from 'react'
import appIcon from '../assets/images/app.png'
import { DOWNLOAD_URL, NAV_LINKS } from '../lib/constants'
import { useExportMode } from '../lib/exportMode'
import { Button } from './ui/Button'

export function Header() {
  const exportMode = useExportMode()
  const [open, setOpen] = useState(false)
  const [scrolled, setScrolled] = useState(false)

  useEffect(() => {
    if (exportMode.enabled) {
      setScrolled(true)
      return
    }
    const onScroll = () => setScrolled(window.scrollY > 24)
    onScroll()
    window.addEventListener('scroll', onScroll, { passive: true })
    return () => window.removeEventListener('scroll', onScroll)
  }, [exportMode.enabled])

  return (
    <header
      data-export-frame="header"
      data-export-section-root="header"
      className={`fixed inset-x-0 top-0 z-50 transition-all duration-500 ${
        scrolled ? 'py-2' : 'py-4'
      }`}
    >
      <div className="mx-auto max-w-6xl px-4 sm:px-6">
        <div
          data-export-layer="header-nav"
          className={`flex items-center justify-between gap-4 rounded-2xl px-3 py-2 transition-all duration-500 sm:px-4 ${
            scrolled
              ? 'border border-white/10 bg-void/75 shadow-[0_12px_40px_rgba(0,0,0,0.45)] backdrop-blur-xl'
              : 'border border-transparent bg-transparent'
          }`}
        >
          <a href="#top" className="flex items-center gap-3">
            <img
              src={appIcon}
              alt=""
              width={40}
              height={40}
              className={`app-icon app-icon-sm transition-[width,height] duration-500 ${
                scrolled ? 'h-8 w-8' : 'h-10 w-10'
              }`}
            />
            <span className="hidden text-sm font-semibold tracking-wide text-white sm:inline">
              AudioPresetSwitcher
            </span>
          </a>

          <nav
            className="hidden items-center gap-1 md:flex"
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
              className="hidden rounded-full px-4 py-2 text-xs sm:inline-flex"
            >
              Download
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
          <div className="mt-2 rounded-2xl border border-white/10 bg-void/95 p-3 backdrop-blur-xl md:hidden">
            <nav className="flex flex-col gap-1" aria-label="Mobile">
              {NAV_LINKS.map((link) => (
                <a
                  key={link.href}
                  href={link.href}
                  className="rounded-xl px-3 py-2.5 text-sm text-muted hover:bg-white/5 hover:text-white"
                  onClick={() => setOpen(false)}
                >
                  {link.label}
                </a>
              ))}
              <Button href={DOWNLOAD_URL} className="mt-2 w-full rounded-full">
                Download
              </Button>
            </nav>
          </div>
        )}
      </div>
    </header>
  )
}
