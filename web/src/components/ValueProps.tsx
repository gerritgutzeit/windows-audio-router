import { Check, Search, ToggleLeft } from 'lucide-react'
import { useState } from 'react'
import { GlowCard } from './GlowCard'
import { SectionReveal } from './SectionReveal'

export function ValueProps() {
  const [presetOn, setPresetOn] = useState(true)

  return (
    <SectionReveal id="presets" className="mx-auto max-w-6xl px-4 py-20 sm:px-6">
      <p className="font-mono text-xs tracking-widest text-accent-bright">01</p>
      <h2 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
        Hardware feel, digital speed
      </h2>
      <p className="mt-3 max-w-2xl text-muted">
        Tactile preset switching inspired by physical gear — with matching that
        survives USB and Bluetooth reconnects.
      </p>

      <div className="mt-10 grid gap-4 md:grid-cols-3">
        <GlowCard className="p-5">
          <div className="mb-4 flex items-center gap-2 text-sm font-medium text-white">
            <ToggleLeft className="h-4 w-4 text-accent-bright" />
            Preset cards
          </div>
          <p className="mb-5 text-sm text-muted">
            Create, edit, duplicate, and activate from a Fluent dashboard — never
            by editing JSON.
          </p>
          <button
            type="button"
            onClick={() => setPresetOn((v) => !v)}
            className={`flex w-full items-center justify-between rounded-xl border px-3 py-3 text-left transition-colors ${
              presetOn
                ? 'border-accent/40 bg-accent/10'
                : 'border-white/10 bg-white/[0.02]'
            }`}
          >
            <div>
              <p className="text-sm font-medium">Desk Studio</p>
              <p className="text-xs text-muted">Click to toggle</p>
            </div>
            <span
              className={`relative h-7 w-12 rounded-full border transition-colors ${
                presetOn ? 'border-accent bg-accent' : 'border-white/15 bg-white/10'
              }`}
            >
              <span
                className={`absolute top-0.5 h-5 w-5 rounded-full bg-white transition-transform ${
                  presetOn ? 'translate-x-6' : 'translate-x-0.5'
                }`}
              />
            </span>
          </button>
        </GlowCard>

        <GlowCard className="p-5">
          <div className="mb-4 flex items-center gap-2 text-sm font-medium text-white">
            <Check className="h-4 w-4 text-accent-bright" />
            System tray
          </div>
          <p className="mb-5 text-sm text-muted">
            Close the window to hide to the tray. Right-click activates a preset
            with a checkmark on the active one.
          </p>
          <div className="rounded-xl border border-white/10 bg-[#0a0b0e] p-3">
            <div className="mb-2 text-[10px] uppercase tracking-wider text-muted">
              Taskbar menu
            </div>
            {['Desk Studio', 'Headset', 'Speakers'].map((name, i) => (
              <div
                key={name}
                className="flex items-center justify-between rounded-lg px-2 py-1.5 text-xs text-white/90 hover:bg-white/5"
              >
                <span>{name}</span>
                {i === 0 && <Check className="h-3.5 w-3.5 text-accent-bright" />}
              </div>
            ))}
            <div className="mt-1 border-t border-white/8 pt-1 text-xs text-muted">
              Exit
            </div>
          </div>
        </GlowCard>

        <GlowCard className="p-5">
          <div className="mb-4 flex items-center gap-2 text-sm font-medium text-white">
            <Search className="h-4 w-4 text-accent-bright" />
            Keyword matching
          </div>
          <p className="mb-5 text-sm text-muted">
            Devices match by FriendlyName keyword — not unstable USB/Bluetooth
            GUIDs.
          </p>
          <div className="space-y-2 font-mono text-[11px]">
            <div className="rounded-lg border border-white/10 bg-white/[0.02] px-3 py-2 text-muted">
              GUID{' '}
              <span className="line-through opacity-50">
                {'{A1B2...F9}'}
              </span>
            </div>
            <div className="rounded-lg border border-accent/30 bg-accent/10 px-3 py-2 text-accent-bright">
              keyword: <span className="text-white">Arctis Nova</span>
            </div>
            <p className="pt-1 text-xs font-sans text-muted">
              Survives reconnects when Windows renames the endpoint slightly.
            </p>
          </div>
        </GlowCard>
      </div>
    </SectionReveal>
  )
}
