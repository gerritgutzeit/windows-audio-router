import { useEffect, useState } from 'react'
import { STAGES } from '../lib/constants'

type StageId = (typeof STAGES)[number]['id']

export function StageRail() {
  const [active, setActive] = useState<StageId>(STAGES[0].id)

  useEffect(() => {
    const nodes = STAGES.map((s) => document.getElementById(s.id)).filter(
      Boolean,
    ) as HTMLElement[]

    if (!nodes.length) return

    const observer = new IntersectionObserver(
      (entries) => {
        const visible = entries
          .filter((e) => e.isIntersecting)
          .sort((a, b) => b.intersectionRatio - a.intersectionRatio)[0]
        const id = visible?.target?.id as StageId | undefined
        if (id) setActive(id)
      },
      { rootMargin: '-35% 0px -45% 0px', threshold: [0.15, 0.4, 0.7] },
    )

    nodes.forEach((n) => observer.observe(n))
    return () => observer.disconnect()
  }, [])

  return (
    <nav
      className="pointer-events-none fixed top-1/2 right-4 z-40 hidden -translate-y-1/2 flex-col gap-3 lg:flex"
      aria-label="Page stages"
    >
      {STAGES.map((stage) => {
        const isActive = active === stage.id
        return (
          <a
            key={stage.id}
            href={`#${stage.id}`}
            className="pointer-events-auto group flex items-center justify-end gap-2"
            aria-current={isActive ? 'true' : undefined}
          >
            <span
              className={`text-[10px] tracking-[0.16em] uppercase transition-all duration-300 ${
                isActive
                  ? 'translate-x-0 opacity-100 text-silver'
                  : 'translate-x-1 opacity-0 text-muted group-hover:translate-x-0 group-hover:opacity-70'
              }`}
            >
              {stage.label}
            </span>
            <span
              className={`block rounded-full transition-all duration-300 ${
                isActive
                  ? 'h-2.5 w-2.5 bg-accent-bright shadow-[0_0_12px_rgba(212,188,140,0.55)]'
                  : 'h-1.5 w-1.5 bg-white/25 group-hover:bg-white/50'
              }`}
            />
          </a>
        )
      })}
    </nav>
  )
}
