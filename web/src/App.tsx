import { CliTerminal } from './components/CliTerminal'
import { FeatureBento } from './components/FeatureBento'
import { Footer } from './components/Footer'
import { Header } from './components/Header'
import { Hero } from './components/Hero'
import { SpecsTable } from './components/SpecsTable'
import { ValueProps } from './components/ValueProps'

export default function App() {
  return (
    <div id="top" className="min-h-screen">
      <Header />
      <main>
        <Hero />
        <ValueProps />
        <FeatureBento />
        <CliTerminal />
        <SpecsTable />
      </main>
      <Footer />
    </div>
  )
}
