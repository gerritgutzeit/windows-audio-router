import {
  createContext,
  createElement,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

export type ExportSection =
  | 'hero'
  | 'product'
  | 'live'
  | 'footer'
  | 'header'
  | null

export type ExportModeState = {
  enabled: boolean
  beat: 0 | 1 | 2
  section: ExportSection
  showHeader: boolean
}

const DEFAULT: ExportModeState = {
  enabled: false,
  beat: 0,
  section: null,
  showHeader: false,
}

export function parseExportMode(): ExportModeState {
  if (typeof window === 'undefined') return DEFAULT

  const params = new URLSearchParams(window.location.search)
  const enabled = params.get('export') === '1'
  if (!enabled) return DEFAULT

  const beatRaw = Number(params.get('beat') ?? '0')
  const beat = (beatRaw === 1 || beatRaw === 2 ? beatRaw : 0) as 0 | 1 | 2
  const sectionParam = params.get('section')
  const section: ExportSection =
    sectionParam === 'hero' ||
    sectionParam === 'product' ||
    sectionParam === 'live' ||
    sectionParam === 'footer' ||
    sectionParam === 'header'
      ? sectionParam
      : null

  return {
    enabled: true,
    beat,
    section,
    showHeader: params.get('header') === '1' || section === 'header',
  }
}

function applyExportDom(state: ExportModeState) {
  if (typeof document === 'undefined') return
  const root = document.documentElement
  if (!state.enabled) {
    delete root.dataset.export
    delete root.dataset.exportSection
    return
  }
  root.dataset.export = '1'
  if (state.section) root.dataset.exportSection = state.section
  else delete root.dataset.exportSection
}

const ExportModeContext = createContext<ExportModeState>(DEFAULT)

export function ExportModeProvider({ children }: { children: ReactNode }) {
  const [state] = useState(() => {
    const next = parseExportMode()
    applyExportDom(next)
    return next
  })

  useEffect(() => {
    applyExportDom(state)
  }, [state])

  const value = useMemo(() => state, [state])

  return createElement(ExportModeContext.Provider, { value }, children)
}

export function useExportMode(): ExportModeState {
  return useContext(ExportModeContext)
}

/** Resting transform for Framer Motion while exporting (no scroll fades). */
export const EXPORT_REST = { opacity: 1, x: 0, y: 0, scale: 1 } as const
