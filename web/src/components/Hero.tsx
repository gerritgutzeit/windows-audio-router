import { ArrowUpRight, Download } from 'lucide-react'
import { DOWNLOAD_URL, REPO_URL } from '../lib/constants'
import { HeroPreview } from './HeroPreview'
import { Badge } from './ui/Badge'
import { Button } from './ui/Button'

export function Hero() {
  return (
    <section className="relative overflow-hidden px-4 pb-20 pt-16 sm:px-6 sm:pt-24">
      <div
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_top,rgba(0,136,255,0.12),transparent_55%)]"
        aria-hidden
      />
      <div className="relative mx-auto grid max-w-6xl items-center gap-12 lg:grid-cols-2 lg:gap-16">
        <div>
          <Badge glow className="mb-6">
            Windows 11 Audio Management
          </Badge>
          <h1 className="max-w-xl text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
            Switch audio sources instantly. No config files.
          </h1>
          <p className="mt-5 max-w-lg text-base leading-relaxed text-muted sm:text-lg">
            A high-precision system-tray app for Windows. Switch playback and
            recording devices in one click — visually or via CLI.
          </p>
          <div className="mt-8 flex flex-wrap items-center gap-3">
            <Button href={DOWNLOAD_URL} className="px-6 py-3">
              <Download className="h-4 w-4" />
              Download for Windows (.exe)
            </Button>
            <Button href={REPO_URL} variant="secondary" className="px-6 py-3">
              View on GitHub
              <ArrowUpRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
        <HeroPreview />
      </div>
    </section>
  )
}
