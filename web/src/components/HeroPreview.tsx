import { AnimatePresence, motion, useReducedMotion } from 'framer-motion'
import { Check, Headphones, Mic } from 'lucide-react'
import { useEffect, useState } from 'react'

const PRESETS = [
  { name: 'Desk Studio', playback: 'Audio Interface XYZ', recording: 'USB Mic Pro' },
  { name: 'Headset', playback: 'Arctis Nova', recording: 'Arctis Nova' },
] as const

export function HeroPreview() {
  const [index, setIndex] = useState(0)
  const [toggling, setToggling] = useState(false)
  const reduceMotion = useReducedMotion()
  const active = PRESETS[index]

  useEffect(() => {
    if (reduceMotion) return
    const id = window.setInterval(() => {
      setToggling(true)
      window.setTimeout(() => {
        setIndex((i) => (i + 1) % PRESETS.length)
        setToggling(false)
      }, 280)
    }, 3200)
    return () => window.clearInterval(id)
  }, [reduceMotion])

  const onToggle = () => {
    setToggling(true)
    window.setTimeout(() => {
      setIndex((i) => (i + 1) % PRESETS.length)
      setToggling(false)
    }, 280)
  }

  return (
    <div className="relative mx-auto w-full max-w-lg">
      <div className="absolute -inset-8 rounded-3xl bg-accent/10 blur-3xl" aria-hidden />
      <div className="relative overflow-hidden rounded-2xl border border-white/10 bg-[#0c0e12]/90 shadow-2xl backdrop-blur-xl">
        <div className="flex items-center gap-2 border-b border-white/8 px-4 py-3">
          <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
          <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
          <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
          <span className="ml-2 text-xs text-muted">Presets</span>
        </div>

        <div className="space-y-3 p-4">
          {PRESETS.map((preset, i) => {
            const isActive = i === index
            return (
              <motion.div
                key={preset.name}
                layout
                className={`flex items-center justify-between rounded-xl border px-3 py-3 transition-colors ${
                  isActive
                    ? 'border-accent/40 bg-accent/10'
                    : 'border-white/8 bg-white/[0.02]'
                }`}
              >
                <div>
                  <p className="text-sm font-medium text-white">{preset.name}</p>
                  <p className="mt-0.5 text-xs text-muted">
                    {preset.playback} · {preset.recording}
                  </p>
                </div>
                <AnimatePresence mode="wait">
                  {isActive ? (
                    <motion.span
                      key="check"
                      initial={{ scale: 0.6, opacity: 0 }}
                      animate={{ scale: 1, opacity: 1 }}
                      exit={{ scale: 0.6, opacity: 0 }}
                      className="flex h-7 w-7 items-center justify-center rounded-full bg-accent text-white"
                    >
                      <Check className="h-3.5 w-3.5" />
                    </motion.span>
                  ) : (
                    <motion.button
                      key="idle"
                      type="button"
                      onClick={onToggle}
                      className="rounded-full border border-white/10 px-3 py-1 text-[11px] text-muted hover:text-white"
                    >
                      Activate
                    </motion.button>
                  )}
                </AnimatePresence>
              </motion.div>
            )
          })}

          <div className="flex items-center justify-between gap-4 rounded-xl border border-white/8 bg-white/[0.02] px-3 py-3">
            <div className="flex items-center gap-3 text-xs text-muted">
              <span className="inline-flex items-center gap-1.5">
                <Headphones className="h-3.5 w-3.5 text-accent-bright" />
                <AnimatePresence mode="wait">
                  <motion.span
                    key={active.playback}
                    initial={{ opacity: 0, y: 4 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -4 }}
                    transition={{ duration: 0.25 }}
                  >
                    {active.playback}
                  </motion.span>
                </AnimatePresence>
              </span>
              <span className="inline-flex items-center gap-1.5">
                <Mic className="h-3.5 w-3.5 text-accent-bright" />
                <AnimatePresence mode="wait">
                  <motion.span
                    key={active.recording}
                    initial={{ opacity: 0, y: 4 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -4 }}
                    transition={{ duration: 0.25 }}
                  >
                    {active.recording}
                  </motion.span>
                </AnimatePresence>
              </span>
            </div>

            <button
              type="button"
              onClick={onToggle}
              aria-label="Toggle active preset"
              className={`relative h-8 w-14 shrink-0 rounded-full border transition-colors ${
                toggling ? 'border-accent-bright/60 bg-accent/30' : 'border-white/15 bg-white/10'
              }`}
            >
              <motion.span
                className="absolute top-0.5 left-0.5 flex h-6 w-6 items-center justify-center rounded-full bg-white shadow"
                animate={{ x: index === 0 ? 0 : 22 }}
                transition={{ type: 'spring', stiffness: 420, damping: 28 }}
              />
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
