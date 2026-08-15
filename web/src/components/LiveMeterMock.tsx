import { motion, useReducedMotion } from 'framer-motion'
import { useMemo } from 'react'

type LiveMeterMockProps = {
  bars?: number
  className?: string
  active?: boolean
}

export function LiveMeterMock({
  bars = 24,
  className = '',
  active = true,
}: LiveMeterMockProps) {
  const reduceMotion = useReducedMotion()
  const seeds = useMemo(
    () => Array.from({ length: bars }, (_, i) => 0.25 + ((i * 17) % 60) / 100),
    [bars],
  )

  return (
    <div
      className={`flex h-16 items-end gap-0.5 sm:gap-1 ${className}`}
      role="img"
      aria-label="Simulated live audio peak meters"
    >
      {seeds.map((seed, i) => (
        <motion.div
          key={i}
          className="w-1 flex-1 rounded-t-sm bg-gradient-to-t from-accent to-accent-bright sm:w-1.5"
          initial={{ height: `${seed * 40}%` }}
          animate={
            active && !reduceMotion
              ? {
                  height: [
                    `${seed * 35}%`,
                    `${Math.min(100, seed * 100 + 40)}%`,
                    `${seed * 55}%`,
                    `${Math.min(95, seed * 80 + 20)}%`,
                    `${seed * 35}%`,
                  ],
                }
              : { height: `${seed * 50}%` }
          }
          transition={{
            duration: 1.4 + (i % 5) * 0.15,
            repeat: Infinity,
            ease: 'easeInOut',
            delay: (i % 8) * 0.05,
          }}
        />
      ))}
    </div>
  )
}
