import { Footer } from './components/Footer'
import { Header } from './components/Header'
import { Hero } from './components/Hero'
import { LiveStage } from './components/LiveStage'
import { ProductStage } from './components/ProductStage'
import { StageRail } from './components/StageRail'
import { ExportModeProvider, useExportMode } from './lib/exportMode'

function AppShell() {
  const { enabled, showHeader, section } = useExportMode()
  const hideChrome = enabled && !showHeader
  const onlyHeader = enabled && section === 'header'

  return (
    <div className={`min-h-screen ${enabled ? 'bg-transparent' : 'bg-void'}`}>
      {!hideChrome && <Header />}
      {!enabled && <StageRail />}
      {!onlyHeader && (
        <main>
          <Hero />
          <ProductStage />
          <LiveStage />
        </main>
      )}
      {!onlyHeader && <Footer />}
    </div>
  )
}

export default function App() {
  return (
    <ExportModeProvider>
      <AppShell />
    </ExportModeProvider>
  )
}
