import {
  motion,
  useReducedMotion,
  useScroll,
  useTransform,
} from 'framer-motion'
import { ChevronDown, Download } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import appIcon from '../assets/images/app.png'
import { DOWNLOAD_URL, REPO_URL } from '../lib/constants'
import { Button } from './ui/Button'

const VIDEO_WEBM = `${import.meta.env.BASE_URL}background_header_loop_hd.webm`
const VIDEO_MP4 = `${import.meta.env.BASE_URL}background_header_loop_hd.mp4`
const FALLBACK_BG = '#080809'

export function Hero() {
  const ref = useRef<HTMLElement>(null)
  const videoRef = useRef<HTMLVideoElement>(null)
  const reduceMotion = useReducedMotion()
  const [videoFailed, setVideoFailed] = useState(false)
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start start', 'end start'],
  })

  const mediaY = useTransform(scrollYProgress, [0, 1], ['0%', '22%'])
  const mediaScale = useTransform(scrollYProgress, [0, 1], [1.02, 1.12])
  const contentY = useTransform(scrollYProgress, [0, 1], ['0%', '14%'])
  const contentOpacity = useTransform(scrollYProgress, [0, 0.55, 0.85], [1, 0.75, 0])
  const fadeOut = useTransform(scrollYProgress, [0.6, 1], [0, 1])

  useEffect(() => {
    const el = videoRef.current
    if (!el || videoFailed) return

    el.muted = true
    el.defaultMuted = true
    el.playsInline = true
    el.loop = true

    const tryPlay = () => {
      if (reduceMotion) {
        el.pause()
        return
      }
      void el.play().catch(() => undefined)
    }

    // Some MP4s don't loop cleanly — restart manually
    const onEnded = () => {
      el.currentTime = 0
      tryPlay()
    }

    // Resume if the browser pauses while the hero is still on screen
    const onVisibility = () => {
      if (document.visibilityState === 'visible') tryPlay()
    }

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (!entry) return
        if (entry.isIntersecting) tryPlay()
        else el.pause()
      },
      { threshold: 0.2 },
    )

    el.addEventListener('ended', onEnded)
    document.addEventListener('visibilitychange', onVisibility)
    observer.observe(el)

    if (el.readyState >= 2) tryPlay()
    else {
      el.addEventListener('canplay', tryPlay, { once: true })
      el.addEventListener('loadeddata', tryPlay, { once: true })
    }

    return () => {
      el.removeEventListener('ended', onEnded)
      el.removeEventListener('canplay', tryPlay)
      el.removeEventListener('loadeddata', tryPlay)
      document.removeEventListener('visibilitychange', onVisibility)
      observer.disconnect()
    }
  }, [reduceMotion, videoFailed])

  return (
    <section
      id="top"
      ref={ref}
      className="relative h-[165svh]"
      aria-label="Hero"
    >
      <div className="stage-pin">
        <div className="absolute inset-0 overflow-hidden" aria-hidden>
          <motion.div
            className="absolute inset-0"
            style={
              reduceMotion
                ? { backgroundColor: FALLBACK_BG }
                : { y: mediaY, scale: mediaScale, backgroundColor: FALLBACK_BG }
            }
          >
            {!videoFailed && (
              <video
                ref={videoRef}
                className="absolute inset-0 h-full w-full object-cover object-center"
                autoPlay
                muted
                loop
                playsInline
                preload="auto"
                disablePictureInPicture
                onError={() => setVideoFailed(true)}
              >
                <source src={VIDEO_WEBM} type="video/webm" />
                <source src={VIDEO_MP4} type="video/mp4" />
              </video>
            )}
            {/* Keep left open for video; shade the copy side */}
            <div className="absolute inset-0 bg-gradient-to-r from-transparent via-void/35 to-void/85" />
            <div className="absolute inset-0 bg-gradient-to-b from-void/30 via-transparent to-void" />
          </motion.div>
        </div>

        <motion.div
          className="pointer-events-none absolute inset-0 bg-void"
          style={reduceMotion ? undefined : { opacity: fadeOut }}
          aria-hidden
        />

        <motion.div
          className="relative z-10 mx-auto flex min-h-[100svh] w-full max-w-6xl items-center justify-end px-4 py-28 sm:px-6 lg:px-8"
          style={
            reduceMotion
              ? undefined
              : { y: contentY, opacity: contentOpacity }
          }
        >
          <div className="flex w-full max-w-xl flex-col items-end text-right">
            <motion.img
              src={appIcon}
              alt="AudioPresetSwitcher"
              width={112}
              height={112}
              className="app-icon h-20 w-20 sm:h-24 sm:w-24 lg:h-28 lg:w-28"
              initial={reduceMotion ? false : { opacity: 0, x: 28, scale: 0.92 }}
              animate={{ opacity: 1, x: 0, scale: 1 }}
              transition={{ duration: 0.8, ease: [0.22, 1, 0.36, 1] }}
            />

            <motion.p
              className="mt-7 text-[11px] font-medium tracking-[0.14em] text-white/55 uppercase sm:text-xs"
              initial={reduceMotion ? false : { opacity: 0, y: 12 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.1, duration: 0.55 }}
            >
              For streamers &amp; desk setups
            </motion.p>

            <motion.h1
              className="title-display mt-3 w-full text-[2.45rem] leading-[1.05] sm:text-5xl md:text-[3.75rem] md:leading-[1.02]"
              initial={reduceMotion ? false : { opacity: 0, y: 18 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.18, duration: 0.65 }}
            >
              switching sources,
              <br />
              finally streamlined
            </motion.h1>

            <motion.p
              className="mt-5 max-w-sm text-base leading-relaxed text-muted sm:text-lg"
              initial={reduceMotion ? false : { opacity: 0, y: 14 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.28, duration: 0.55 }}
            >
              Flip Audio defaults in one click. Hassle free.
            </motion.p>

            <motion.div
              className="pointer-events-auto mt-8 flex flex-col items-end gap-3 sm:flex-row sm:items-center sm:justify-end"
              initial={reduceMotion ? false : { opacity: 0, y: 14 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.36, duration: 0.55 }}
            >
              <a
                href={REPO_URL}
                className="order-2 rounded-full px-2 py-2 text-sm text-muted transition-colors hover:text-white sm:order-1"
              >
                View source →
              </a>
              <Button
                href={DOWNLOAD_URL}
                className="order-1 rounded-full px-7 py-3.5 text-base sm:order-2"
              >
                <Download className="h-4 w-4" />
                Download for Windows
              </Button>
            </motion.div>
          </div>
        </motion.div>

        <a
          href="#product"
          className="absolute bottom-8 left-1/2 z-10 flex -translate-x-1/2 flex-col items-center gap-1 text-[10px] tracking-[0.2em] text-muted uppercase transition-colors hover:text-white"
        >
          Scroll
          <ChevronDown className="h-4 w-4 animate-bounce" />
        </a>
      </div>
    </section>
  )
}
