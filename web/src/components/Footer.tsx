import { motion, useReducedMotion } from 'framer-motion'
import { Download } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import appIcon from '../assets/images/app.png'
import { DOWNLOAD_URL, REPO_URL } from '../lib/constants'
import { useExportMode } from '../lib/exportMode'
import { Button } from './ui/Button'

const VIDEO_WEBM = `${import.meta.env.BASE_URL}background.webm`
const VIDEO_MP4 = `${import.meta.env.BASE_URL}background.mp4`
const PANEL_BG = '#101012'

function useCanHover() {
  const [canHover, setCanHover] = useState(false)

  useEffect(() => {
    const mq = window.matchMedia('(hover: hover) and (pointer: fine)')
    const update = () => setCanHover(mq.matches)
    update()
    mq.addEventListener('change', update)
    return () => mq.removeEventListener('change', update)
  }, [])

  return canHover
}

export function Footer() {
  const exportMode = useExportMode()
  const reduceMotion = !!useReducedMotion() || exportMode.enabled
  const canHover = useCanHover()
  const videoRef = useRef<HTMLVideoElement>(null)
  const hoveringRef = useRef(false)
  const [videoFailed, setVideoFailed] = useState(false)

  useEffect(() => {
    if (exportMode.enabled) return
    const el = videoRef.current
    if (!el || videoFailed) return

    if (reduceMotion) {
      el.pause()
      return
    }

    if (!canHover) {
      el.loop = true
      const play = () => {
        void el.play().catch(() => undefined)
      }
      if (el.readyState >= 2) play()
      else el.addEventListener('canplay', play, { once: true })
      return () => el.removeEventListener('canplay', play)
    }

    hoveringRef.current = false
    el.loop = false
    el.pause()
    el.currentTime = 0
  }, [reduceMotion, videoFailed, canHover, exportMode.enabled])

  const playWhileHovered = () => {
    const el = videoRef.current
    if (!el || videoFailed || reduceMotion || !canHover) return
    hoveringRef.current = true
    el.loop = true
    void el.play().catch(() => undefined)
  }

  const finishCycleOnLeave = () => {
    const el = videoRef.current
    if (!el || !canHover) return
    hoveringRef.current = false
    el.loop = false
  }

  const onVideoEnded = () => {
    const el = videoRef.current
    if (!el || !canHover) return
    if (hoveringRef.current) {
      el.loop = true
      void el.play().catch(() => undefined)
      return
    }
    el.pause()
    el.currentTime = 0
  }

  return (
    <footer
      id="get"
      className="relative overflow-hidden px-4 pb-10 pt-8 sm:px-6"
      data-export-section-root="footer"
    >
      <div
        data-export-hide
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_30%_0%,rgba(181,154,109,0.1),transparent_55%)]"
      />

      <motion.div
        data-export-frame="footer"
        className={`relative mx-auto max-w-6xl overflow-hidden rounded-[2rem] ${
          exportMode.enabled
            ? ''
            : 'border border-accent/25 shadow-[0_30px_80px_rgba(0,0,0,0.55)]'
        }`}
        initial={reduceMotion ? false : { opacity: 0, y: 40 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true, margin: '-10%' }}
        transition={{ duration: 0.7, ease: [0.22, 1, 0.36, 1] }}
      >
        <div
          className="relative overflow-hidden"
          style={{
            backgroundColor: exportMode.enabled ? 'transparent' : PANEL_BG,
          }}
          onMouseEnter={playWhileHovered}
          onMouseLeave={finishCycleOnLeave}
        >
          <div className="absolute inset-0 overflow-hidden" aria-hidden>
            {!videoFailed && !exportMode.enabled && (
              <video
                ref={videoRef}
                className="absolute inset-y-0 left-0 h-full w-[72%] max-w-none scale-105 object-cover object-center sm:w-[64%]"
                style={{
                  WebkitMaskImage:
                    'linear-gradient(90deg, #000 0%, #000 42%, rgba(0,0,0,0.75) 62%, rgba(0,0,0,0.35) 78%, transparent 100%)',
                  maskImage:
                    'linear-gradient(90deg, #000 0%, #000 42%, rgba(0,0,0,0.75) 62%, rgba(0,0,0,0.35) 78%, transparent 100%)',
                }}
                muted
                playsInline
                preload="auto"
                autoPlay={!canHover && !reduceMotion}
                loop={!canHover}
                onEnded={onVideoEnded}
                onError={() => setVideoFailed(true)}
              >
                <source src={VIDEO_WEBM} type="video/webm" />
                <source src={VIDEO_MP4} type="video/mp4" />
              </video>
            )}

            <div
              data-export-layer="footer-mask-gradient"
              className="absolute inset-0"
              style={{
                background: `linear-gradient(
                  90deg,
                  transparent 0%,
                  transparent 36%,
                  color-mix(in srgb, ${PANEL_BG} 20%, transparent) 52%,
                  color-mix(in srgb, ${PANEL_BG} 55%, transparent) 68%,
                  color-mix(in srgb, ${PANEL_BG} 88%, transparent) 82%,
                  ${PANEL_BG} 94%,
                  ${PANEL_BG} 100%
                )`,
              }}
            />

            <div
              data-export-layer="footer-gold-radial"
              className="absolute inset-0 bg-[radial-gradient(ellipse_at_58%_45%,rgba(181,154,109,0.1),transparent_55%)]"
            />
          </div>

          <div
            data-export-layer="footer-ring"
            className="pointer-events-none absolute inset-0 ring-1 ring-inset ring-accent/15"
            aria-hidden
          />

          <div className="relative flex min-h-[22rem] items-center justify-end px-6 py-14 sm:min-h-[26rem] sm:px-10 sm:py-16 lg:px-14">
            <div className="flex w-full max-w-md flex-col items-end text-right lg:max-w-lg">
              <img
                src={appIcon}
                alt=""
                width={64}
                height={64}
                data-export-layer="footer-icon"
                className="app-icon h-14 w-14 sm:h-16 sm:w-16"
              />
              <h2
                data-export-layer="footer-title"
                className="title-display mt-6 text-4xl sm:text-5xl"
              >
                Ready to Switch?
              </h2>
              <p
                data-export-layer="footer-body"
                className="mt-4 max-w-sm text-muted"
              >
                Native Windows app. Tray, dashboard, CLI — one installer.
              </p>
              <div
                data-export-layer="footer-ctas"
                className="mt-8 flex flex-col items-end gap-3 sm:flex-row sm:items-center sm:justify-end"
              >
                <a
                  href={`${REPO_URL}/blob/main/README.md`}
                  className="order-2 text-sm text-muted transition-colors hover:text-white sm:order-1"
                >
                  Installation guide →
                </a>
                <Button
                  href={DOWNLOAD_URL}
                  className="order-1 rounded-full px-7 py-3.5 text-base sm:order-2"
                >
                  <Download className="h-4 w-4" />
                  Get AudioPresetSwitcher
                </Button>
              </div>
            </div>
          </div>
        </div>

        <div
          data-export-layer="footer-meta"
          className="relative z-10 flex flex-col items-center justify-between gap-3 border-t border-white/10 bg-[#0c0c0e] px-6 py-5 text-xs text-muted sm:flex-row sm:px-10"
        >
          <p>C# · .NET 8 · WPF-UI · Velopack</p>
          <div className="flex gap-4">
            <a href={REPO_URL} className="hover:text-white">
              GitHub
            </a>
            <a href={`${REPO_URL}/releases/latest`} className="hover:text-white">
              Releases
            </a>
            <a href="#top" className="hover:text-white">
              Top
            </a>
          </div>
        </div>
      </motion.div>
    </footer>
  )
}
