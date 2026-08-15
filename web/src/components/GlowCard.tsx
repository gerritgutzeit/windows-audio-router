import {
  useCallback,
  useRef,
  type CSSProperties,
  type MouseEvent,
  type ReactNode,
} from 'react'

type GlowCardProps = {
  children: ReactNode
  className?: string
}

export function GlowCard({ children, className = '' }: GlowCardProps) {
  const ref = useRef<HTMLDivElement>(null)

  const onMove = useCallback((e: MouseEvent<HTMLDivElement>) => {
    const el = ref.current
    if (!el) return
    const rect = el.getBoundingClientRect()
    const x = e.clientX - rect.left
    const y = e.clientY - rect.top
    el.style.setProperty('--glow-x', `${x}px`)
    el.style.setProperty('--glow-y', `${y}px`)
  }, [])

  return (
    <div
      ref={ref}
      onMouseMove={onMove}
      className={`group relative overflow-hidden rounded-2xl glass ${className}`}
      style={
        {
          '--glow-x': '50%',
          '--glow-y': '50%',
        } as CSSProperties
      }
    >
      <div
        className="pointer-events-none absolute inset-0 opacity-0 transition-opacity duration-300 group-hover:opacity-100"
        style={{
          background:
            'radial-gradient(480px circle at var(--glow-x) var(--glow-y), rgba(0,136,255,0.12), transparent 55%)',
        }}
      />
      <div className="relative z-10">{children}</div>
    </div>
  )
}
