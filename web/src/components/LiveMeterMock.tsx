import { motion, useReducedMotion } from 'framer-motion'
import { Headphones, Mic, type LucideIcon } from 'lucide-react'

type DeviceMeterRowProps = {
  icon: LucideIcon
  label: string
  device: string
  /** Base activity 0–1 for the animated fill */
  activity?: number
  className?: string
}

function AnimatedLevelBar({
  activity = 0.35,
  active = true,
}: {
  activity?: number
  active?: boolean
}) {
  const reduceMotion = useReducedMotion()
  const base = Math.max(0.06, Math.min(0.92, activity))

  return (
    <div
      className="mt-2 h-1 w-full overflow-hidden rounded-full bg-[#2e2e30]"
      role="img"
      aria-label="Simulated live audio level"
    >
      <motion.div
        className="h-full rounded-full bg-accent-bright"
        initial={{ width: `${base * 28}%` }}
        animate={
          active && !reduceMotion
            ? {
                width: [
                  `${base * 18}%`,
                  `${Math.min(95, base * 100 + 22)}%`,
                  `${base * 42}%`,
                  `${Math.min(88, base * 75 + 12)}%`,
                  `${base * 18}%`,
                ],
              }
            : { width: `${base * 40}%` }
        }
        transition={{
          duration: 1.55 + base * 0.4,
          repeat: Infinity,
          ease: 'easeInOut',
        }}
      />
    </div>
  )
}

export function DeviceMeterRow({
  icon: Icon,
  label,
  device,
  activity = 0.35,
  className = '',
}: DeviceMeterRowProps) {
  return (
    <div className={`flex items-start gap-3 ${className}`}>
      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-[#222224]">
        <Icon className="h-4 w-4 text-[#f0eeea]" strokeWidth={1.75} aria-hidden />
      </div>
      <div className="min-w-0 flex-1 pt-0.5">
        <p className="text-xs text-[#8a8680]">{label}</p>
        <p className="truncate text-sm font-semibold text-white">{device}</p>
        <AnimatedLevelBar activity={activity} />
      </div>
    </div>
  )
}

type LiveStatusCardProps = {
  className?: string
}

export function LiveStatusCard({ className = '' }: LiveStatusCardProps) {
  return (
    <div
      className={`rounded-xl bg-[#141416] p-4 ${className}`}
      aria-label="Live device status with meters"
    >
      <DeviceMeterRow
        icon={Headphones}
        label="Headphones"
        device="Kopfhörer (AirPods Max)"
        activity={0.28}
      />
      <DeviceMeterRow
        className="mt-4"
        icon={Mic}
        label="Microphone"
        device="Microphone (4- Shure MV7)"
        activity={0.55}
      />
    </div>
  )
}
