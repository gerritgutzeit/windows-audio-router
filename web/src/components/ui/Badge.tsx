import type { ReactNode } from 'react'

type BadgeProps = {
  children: ReactNode
  className?: string
  glow?: boolean
}

export function Badge({ children, className = '', glow = false }: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center gap-2 rounded-full border border-white/10 bg-white/[0.04] px-3 py-1 text-xs font-medium text-muted backdrop-blur-md ${
        glow ? 'shadow-[0_0_20px_rgba(0,136,255,0.35)] border-accent/40 text-accent-bright' : ''
      } ${className}`}
    >
      {glow && (
        <span className="relative flex h-1.5 w-1.5">
          <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-accent-bright opacity-60" />
          <span className="relative inline-flex h-1.5 w-1.5 rounded-full bg-accent-bright" />
        </span>
      )}
      {children}
    </span>
  )
}
