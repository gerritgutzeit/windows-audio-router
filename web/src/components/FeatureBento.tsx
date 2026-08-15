import { Layers, Radio, RefreshCw, Terminal } from 'lucide-react'
import { GlowCard } from './GlowCard'
import { LiveMeterMock } from './LiveMeterMock'
import { SectionReveal } from './SectionReveal'

export function FeatureBento() {
  return (
    <SectionReveal id="live-monitor" className="mx-auto max-w-6xl px-4 py-20 sm:px-6">
      <p className="font-mono text-xs tracking-widest text-accent-bright">02</p>
      <h2 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
        Built for power users & streamers
      </h2>
      <p className="mt-3 max-w-2xl text-muted">
        Dual-role defaults, live meters, automation hooks, and silent updates —
        without leaving the tray.
      </p>

      <div className="mt-10 grid gap-4 sm:grid-cols-2 lg:grid-cols-4 lg:grid-rows-2">
        <GlowCard className="p-5 sm:col-span-2 lg:col-span-2 lg:row-span-2">
          <div className="mb-3 flex items-center gap-2 text-sm font-medium">
            <Radio className="h-4 w-4 text-accent-bright" />
            Live audio meters
          </div>
          <p className="mb-6 text-sm text-muted">
            Watch volume and peak levels for connected playback and recording
            devices in the Live status page.
          </p>
          <div className="rounded-xl border border-white/10 bg-[#0a0b0e] p-4">
            <div className="mb-2 flex justify-between text-xs text-muted">
              <span>Playback · Speakers</span>
              <span className="text-accent-bright">Peak</span>
            </div>
            <LiveMeterMock />
            <div className="mb-2 mt-5 flex justify-between text-xs text-muted">
              <span>Recording · Microphone</span>
              <span className="text-accent-bright">Peak</span>
            </div>
            <LiveMeterMock bars={20} />
          </div>
        </GlowCard>

        <GlowCard className="p-5 lg:col-span-2">
          <div className="mb-3 flex items-center gap-2 text-sm font-medium">
            <Layers className="h-4 w-4 text-accent-bright" />
            Dual-role switching
          </div>
          <p className="text-sm text-muted">
            Applying a preset sets multimedia and communications defaults to the
            same playback and recording devices — Teams, Discord, and similar
            stay in sync.
          </p>
          <div className="mt-4 flex flex-wrap gap-2">
            {['Console', 'Multimedia', 'Communications'].map((role) => (
              <span
                key={role}
                className="rounded-full border border-accent/30 bg-accent/10 px-3 py-1 text-xs text-accent-bright"
              >
                {role}
              </span>
            ))}
          </div>
        </GlowCard>

        <GlowCard className="p-5">
          <div className="mb-3 flex items-center gap-2 text-sm font-medium">
            <Terminal className="h-4 w-4 text-accent-bright" />
            Automation ready
          </div>
          <p className="text-sm text-muted">
            Stream Deck and scripts call the installed EXE with{' '}
            <code className="font-mono text-accent-bright">--preset</code> flags.
          </p>
        </GlowCard>

        <GlowCard className="p-5">
          <div className="mb-3 flex items-center gap-2 text-sm font-medium">
            <RefreshCw className="h-4 w-4 text-accent-bright" />
            Background updates
          </div>
          <p className="text-sm text-muted">
            Velopack installer architecture checks GitHub Releases and updates in
            the background.
          </p>
        </GlowCard>
      </div>
    </SectionReveal>
  )
}
