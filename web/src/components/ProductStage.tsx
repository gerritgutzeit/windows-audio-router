import {
  AnimatePresence,
  motion,
  useMotionValueEvent,
  useReducedMotion,
  useScroll,
  useTransform,
  type MotionValue,
} from 'framer-motion'
import { Check, Search, Terminal } from 'lucide-react'
import { useRef, useState } from 'react'
import screenshot from '../assets/images/Screenshot.png'
import { useExportMode, EXPORT_REST } from '../lib/exportMode'

const BEATS = [
  {
    id: 'dashboard',
    kicker: '01 · Software',
    title: 'Audio dashboard',
    body: 'Presets as cards — activate in one click.',
  },
  {
    id: 'tray',
    kicker: '02 · Tray',
    title: 'Always one click away.',
    body: 'Hide to the tray. Right-click any preset live.',
  },
  {
    id: 'automate',
    kicker: '03 · Automate',
    title: 'Automation ready.',
    body: 'CLI flags + named pipes for scripts and decks.',
  },
] as const

function useBeatIndex(progress: MotionValue<number>, reduceMotion: boolean) {
  const [beat, setBeat] = useState(0)

  useMotionValueEvent(progress, 'change', (v) => {
    if (reduceMotion) return
    if (v < 0.45) setBeat(0)
    else if (v < 0.72) setBeat(1)
    else setBeat(2)
  })

  return beat
}

function DashboardShowcase({ reduceMotion }: { reduceMotion: boolean }) {
  return (
    <div className="relative mx-auto w-full max-w-[1200px] px-2 sm:px-4">
      <motion.div
        data-export-layer="product-ambient-glow"
        className="pointer-events-none absolute -inset-[8%] rounded-[3rem] bg-accent/20 blur-[80px]"
        aria-hidden
        animate={
          reduceMotion
            ? undefined
            : {
                opacity: [0.35, 0.55, 0.35],
                scale: [0.96, 1.02, 0.96],
              }
        }
        transition={{ duration: 6, repeat: Infinity, ease: 'easeInOut' }}
      />

      <motion.div
        data-export-layer="product-visual"
        className="relative origin-center"
        style={{ perspective: 1600 }}
        initial={
          reduceMotion
            ? false
            : { opacity: 0, y: 80, rotateX: 14, scale: 0.88 }
        }
        animate={{ opacity: 1, y: 0, rotateX: 0, scale: 1 }}
        transition={{ duration: 0.9, ease: [0.16, 1, 0.3, 1] }}
      >
        <motion.div
          className="relative overflow-hidden rounded-2xl border border-white/10 shadow-[0_40px_120px_rgba(0,0,0,0.65)] sm:rounded-3xl"
          animate={
            reduceMotion
              ? undefined
              : { y: [0, -10, 0], rotateX: [0, 1.2, 0], rotateY: [0, -1.5, 0] }
          }
          transition={{ duration: 7, repeat: Infinity, ease: 'easeInOut' }}
          style={{ transformStyle: 'preserve-3d' }}
        >
          <div className="relative">
            <img
              src={screenshot}
              alt="AudioPresetSwitcher audio dashboard"
              className="block h-auto w-full"
              width={1166}
              height={774}
            />

            <div
              className="pointer-events-none absolute inset-0 bg-gradient-to-t from-void/40 via-transparent to-transparent"
              aria-hidden
            />

            {!reduceMotion && (
              <motion.div
                className="pointer-events-none absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
                aria-hidden
                initial={{ x: '-120%', opacity: 0 }}
                animate={{ x: ['-120%', '140%'], opacity: [0, 1, 0] }}
                transition={{
                  duration: 2.4,
                  delay: 0.8,
                  repeat: Infinity,
                  repeatDelay: 4.5,
                  ease: 'easeInOut',
                }}
              />
            )}
          </div>
        </motion.div>
      </motion.div>
    </div>
  )
}

function TrayPreview() {
  return (
    <motion.div
      data-export-layer="product-visual"
      className="mx-auto w-full max-w-md rounded-2xl border border-white/10 bg-[#141416]/95 p-5 shadow-2xl"
      initial={{ opacity: 0, scale: 0.94, y: 24 }}
      animate={{ opacity: 1, scale: 1, y: 0 }}
      transition={{ duration: 0.5, ease: [0.22, 1, 0.36, 1] }}
    >
      <p className="mb-4 text-[10px] tracking-[0.18em] text-muted uppercase">
        System tray
      </p>
      {['Desk Studio', 'Headset', 'Speakers'].map((name, i) => (
        <motion.div
          key={name}
          className="flex items-center justify-between rounded-lg px-2 py-2.5 text-sm text-white/90"
          initial={{ opacity: 0, x: -12 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ delay: 0.12 + i * 0.08 }}
        >
          <span>{name}</span>
          {i === 0 && <Check className="h-4 w-4 text-accent-bright" />}
        </motion.div>
      ))}
    </motion.div>
  )
}

function AutomatePreview() {
  return (
    <div
      data-export-layer="product-visual"
      className="mx-auto flex w-full max-w-lg flex-col gap-4"
    >
      <motion.div
        className="rounded-2xl metal-panel p-6"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.45 }}
      >
        <Terminal className="mb-3 h-5 w-5 text-accent-bright" />
        <code className="block font-mono text-sm leading-relaxed text-silver">
          AudioPresetSwitcher.exe --preset &quot;Desk Studio&quot;
        </code>
      </motion.div>
      <motion.div
        className="rounded-2xl metal-panel p-6"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.45, delay: 0.1 }}
      >
        <Search className="mb-3 h-5 w-5 text-accent-bright" />
        <p className="text-sm text-muted">Match by keyword — not USB GUIDs.</p>
        <p className="mt-2 font-mono text-sm text-accent-bright">
          keyword: Arctis Nova
        </p>
      </motion.div>
    </div>
  )
}

export function ProductStage() {
  const ref = useRef<HTMLElement>(null)
  const exportMode = useExportMode()
  const reduceMotion = !!useReducedMotion() || exportMode.enabled
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start start', 'end end'],
  })
  const scrollBeat = useBeatIndex(scrollYProgress, reduceMotion)
  const beat = exportMode.enabled ? exportMode.beat : scrollBeat
  const copy = BEATS[beat]

  const showcaseScale = useTransform(scrollYProgress, [0, 0.35], [1, 1.03])
  const overlayOpacity = useTransform(scrollYProgress, [0, 0.12, 0.4], [0, 1, 1])

  return (
    <section
      id="product"
      ref={ref}
      className={`relative ${exportMode.enabled ? 'h-[100svh]' : 'h-[320svh]'} ${exportMode.enabled ? 'bg-transparent' : 'bg-void'}`}
      aria-label="Product stages"
      data-export-section-root="product"
    >
      <div className="stage-pin overflow-hidden" data-export-frame="product">
        <div
          data-export-layer="product-radial"
          className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_50%_30%,rgba(181,154,109,0.12),transparent_55%)]"
        />

        <AnimatePresence mode="wait">
          {beat === 0 ? (
            <motion.div
              key="showcase"
              className="relative z-10 flex h-full min-h-[100svh] w-full flex-col justify-center py-20"
              initial={exportMode.enabled ? false : { opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0, y: -24 }}
              transition={{ duration: 0.4 }}
            >
              <motion.div
                data-export-layer="product-copy"
                className="mx-auto mb-6 max-w-3xl px-4 text-center sm:mb-8 sm:px-6"
                style={
                  exportMode.enabled
                    ? EXPORT_REST
                    : reduceMotion
                      ? undefined
                      : { opacity: overlayOpacity }
                }
              >
                <p className="text-[11px] font-medium tracking-[0.14em] text-accent-bright uppercase">
                  {copy.kicker}
                </p>
                <h2 className="title-display mt-2 text-2xl sm:text-3xl">
                  {copy.title}
                </h2>
              </motion.div>

              <motion.div
                className="w-full"
                style={
                  exportMode.enabled
                    ? EXPORT_REST
                    : reduceMotion
                      ? undefined
                      : { scale: showcaseScale }
                }
              >
                <DashboardShowcase reduceMotion={reduceMotion} />
              </motion.div>
            </motion.div>
          ) : (
            <motion.div
              key={`beat-${beat}`}
              className="relative z-10 mx-auto grid w-full max-w-6xl items-center gap-10 px-4 py-24 sm:px-6 lg:grid-cols-2 lg:gap-16"
              initial={exportMode.enabled ? false : { opacity: 0, y: 28 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -20 }}
              transition={{ duration: 0.45, ease: [0.22, 1, 0.36, 1] }}
            >
              <div data-export-layer="product-copy">
                <p className="text-xs font-medium tracking-[0.14em] text-accent-bright uppercase">
                  {copy.kicker}
                </p>
                <h2 className="title-display mt-3 text-4xl sm:text-5xl">
                  {copy.title}
                </h2>
                <p className="mt-4 max-w-md text-lg text-muted">{copy.body}</p>
              </div>
              <div className="relative min-h-[240px]">
                {beat === 1 ? <TrayPreview /> : <AutomatePreview />}
              </div>
            </motion.div>
          )}
        </AnimatePresence>

        <div
          data-export-layer="product-beat-dots"
          className="absolute inset-x-0 bottom-8 z-20 mx-auto flex max-w-xs gap-2 px-4"
        >
          {BEATS.map((b, i) => (
            <span
              key={b.id}
              className={`h-1 flex-1 rounded-full transition-colors duration-300 ${
                i === beat ? 'bg-accent-bright' : 'bg-white/15'
              }`}
            />
          ))}
        </div>
      </div>
    </section>
  )
}
