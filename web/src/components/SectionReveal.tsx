import { motion, useReducedMotion } from 'framer-motion'
import type { ReactNode } from 'react'

type SectionRevealProps = {
  children: ReactNode
  className?: string
  delay?: number
  id?: string
}

export function SectionReveal({
  children,
  className = '',
  delay = 0,
  id,
}: SectionRevealProps) {
  const reduceMotion = useReducedMotion()

  return (
    <motion.section
      id={id}
      className={className}
      initial={reduceMotion ? false : { opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, margin: '-80px' }}
      transition={{ duration: 0.55, ease: [0.22, 1, 0.36, 1], delay }}
    >
      {children}
    </motion.section>
  )
}
