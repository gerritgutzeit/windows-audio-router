import { Footer } from './components/Footer'
import { Header } from './components/Header'
import { Hero } from './components/Hero'
import { LiveStage } from './components/LiveStage'
import { ProductStage } from './components/ProductStage'
import { StageRail } from './components/StageRail'

export default function App() {
  return (
    <div className="min-h-screen bg-void">
      <Header />
      <StageRail />
      <main>
        <Hero />
        <ProductStage />
        <LiveStage />
      </main>
      <Footer />
    </div>
  )
}
