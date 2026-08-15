import { Check, Copy } from 'lucide-react'
import { useState } from 'react'
import { CLI_SNIPPET } from '../lib/constants'
import { GlowCard } from './GlowCard'
import { SectionReveal } from './SectionReveal'

function highlightLine(line: string) {
  if (line.startsWith('#')) {
    return <span className="text-muted">{line}</span>
  }

  const parts = line.split(/(\s+)/)
  return parts.map((part, i) => {
    if (part.startsWith('--') || part === '-p') {
      return (
        <span key={i} className="text-accent-bright">
          {part}
        </span>
      )
    }
    if (part.startsWith('"') && part.endsWith('"')) {
      return (
        <span key={i} className="text-emerald-300/90">
          {part}
        </span>
      )
    }
    if (/^\d+$/.test(part)) {
      return (
        <span key={i} className="text-amber-200/90">
          {part}
        </span>
      )
    }
    if (part.includes('.exe')) {
      return (
        <span key={i} className="text-white">
          {part}
        </span>
      )
    }
    return (
      <span key={i} className="text-white/80">
        {part}
      </span>
    )
  })
}

export function CliTerminal() {
  const [copied, setCopied] = useState(false)

  const copy = async () => {
    try {
      await navigator.clipboard.writeText(CLI_SNIPPET)
      setCopied(true)
      window.setTimeout(() => setCopied(false), 2000)
    } catch {
      setCopied(false)
    }
  }

  return (
    <SectionReveal id="automation" className="mx-auto max-w-6xl px-4 py-20 sm:px-6">
      <p className="font-mono text-xs tracking-widest text-accent-bright">03</p>
      <h2 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
        Automation made simple
      </h2>
      <p className="mt-3 max-w-2xl text-muted">
        If the tray app is already running, a second process forwards the command
        over a local named pipe and exits.
      </p>

      <GlowCard className="mt-10 overflow-hidden p-0">
        <div className="flex items-center justify-between border-b border-white/8 px-4 py-3">
          <div className="flex items-center gap-2">
            <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
            <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
            <span className="h-2.5 w-2.5 rounded-full bg-white/15" />
            <span className="ml-2 font-mono text-xs text-muted">powershell</span>
          </div>
          <button
            type="button"
            onClick={copy}
            className="inline-flex items-center gap-1.5 rounded-full border border-white/10 bg-white/[0.04] px-3 py-1.5 text-xs text-muted transition-colors hover:text-white"
          >
            {copied ? (
              <>
                <Check className="h-3.5 w-3.5 text-accent-bright" />
                Copied
              </>
            ) : (
              <>
                <Copy className="h-3.5 w-3.5" />
                Copy
              </>
            )}
          </button>
        </div>
        <pre className="overflow-x-auto p-5 font-mono text-xs leading-relaxed sm:text-sm">
          <code>
            {CLI_SNIPPET.split('\n').map((line, i) => (
              <div key={i} className="min-h-[1.4em]">
                {highlightLine(line)}
              </div>
            ))}
          </code>
        </pre>
      </GlowCard>

      {copied && (
        <div
          role="status"
          className="fixed bottom-6 left-1/2 z-50 -translate-x-1/2 rounded-full border border-accent/40 bg-void/95 px-4 py-2 text-sm text-white shadow-lg backdrop-blur-xl glow-accent"
        >
          CLI snippet copied to clipboard
        </div>
      )}
    </SectionReveal>
  )
}
