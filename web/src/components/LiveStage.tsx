import {
  motion,
  useReducedMotion,
  useScroll,
  useTransform,
} from 'framer-motion'
import { useRef } from 'react'
import { useExportMode, EXPORT_REST } from '../lib/exportMode'
import { LiveStatusCard } from './LiveMeterMock'

export function LiveStage() {
  const ref = useRef<HTMLElement>(null)
  const exportMode = useExportMode()
  const reduceMotion = !!useReducedMotion() || exportMode.enabled
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start end', 'end start'],
  })

  const cardY = useTransform(scrollYProgress, [0.15, 0.5, 0.85], [60, 0, -40])
  const cardScale = useTransform(scrollYProgress, [0.2, 0.5], [0.92, 1])
  const cardOpacity = useTransform(scrollYProgress, [0.1, 0.35, 0.8, 0.95], [0, 1, 1, 0.4])
  const textY = useTransform(scrollYProgress, [0.1, 0.4], [40, 0])
  const textOpacity = useTransform(scrollYProgress, [0.1, 0.35], [0, 1])

  return (
    <section
      id="live"
      ref={ref}
      className={`relative ${exportMode.enabled ? 'h-[100svh] bg-transparent' : 'min-h-[160svh] bg-void'}`}
      aria-label="Live meters"
      data-export-section-root="live"
    >
      <div className="stage-pin" data-export-frame="live">
        <div
          data-export-layer="live-radial"
          className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_70%_40%,rgba(181,154,109,0.1),transparent_55%)]"
        />

        <div className="relative z-10 mx-auto flex w-full max-w-5xl flex-col items-center gap-12 px-4 py-24 sm:px-6 lg:flex-row lg:items-center lg:justify-between lg:gap-16">
          <motion.div
            data-export-layer="live-copy"
            className="max-w-md text-center lg:text-left"
            style={
              exportMode.enabled
                ? EXPORT_REST
                : reduceMotion
                  ? undefined
                  : { y: textY, opacity: textOpacity }
            }
          >
            <p className="text-xs font-medium tracking-[0.14em] text-accent-bright uppercase">
              Live status
            </p>
            <h2 className="title-display mt-3 text-4xl sm:text-5xl">
              See it. Switch it.
            </h2>
            <p className="mt-4 text-lg text-muted">
              Real device names, live levels — the same card you see in the app.
            </p>
          </motion.div>

          <motion.div
            className="relative w-full max-w-md"
            style={
              exportMode.enabled
                ? EXPORT_REST
                : reduceMotion
                  ? undefined
                  : { y: cardY, scale: cardScale, opacity: cardOpacity }
            }
          >
            <div
              data-export-layer="live-glow"
              className="pointer-events-none absolute -inset-8 rounded-[2rem] bg-accent/15 blur-3xl"
              aria-hidden
            />
            <div data-export-layer="live-card">
              <LiveStatusCard className="relative shadow-[0_30px_80px_rgba(0,0,0,0.5)]" />
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  )
}
