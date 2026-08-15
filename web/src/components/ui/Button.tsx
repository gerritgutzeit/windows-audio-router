import type { AnchorHTMLAttributes, ButtonHTMLAttributes, ReactNode } from 'react'

type Variant = 'primary' | 'secondary' | 'ghost'

type Shared = {
  variant?: Variant
  children: ReactNode
  className?: string
}

type ButtonAsButton = Shared &
  ButtonHTMLAttributes<HTMLButtonElement> & { href?: undefined }

type ButtonAsLink = Shared &
  AnchorHTMLAttributes<HTMLAnchorElement> & { href: string }

type ButtonProps = ButtonAsButton | ButtonAsLink

const variants: Record<Variant, string> = {
  primary:
    'bg-accent text-white glow-accent hover:bg-accent-bright hover:shadow-[0_0_32px_rgba(0,210,255,0.45)]',
  secondary: 'glass text-white hover:bg-white/5 hover:border-white/20',
  ghost: 'text-muted hover:text-white hover:bg-white/5',
}

export function Button({
  variant = 'primary',
  children,
  className = '',
  ...props
}: ButtonProps) {
  const classes = `inline-flex items-center justify-center gap-2 rounded-full px-5 py-2.5 text-sm font-medium transition-all duration-300 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent-bright disabled:opacity-50 ${variants[variant]} ${className}`

  if ('href' in props && props.href) {
    const { href, ...rest } = props
    return (
      <a href={href} className={classes} {...rest}>
        {children}
      </a>
    )
  }

  const buttonProps = props as ButtonAsButton
  return (
    <button type="button" className={classes} {...buttonProps}>
      {children}
    </button>
  )
}
