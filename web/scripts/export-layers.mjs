import { spawn } from 'node:child_process'
import { mkdir, rm, writeFile } from 'node:fs/promises'
import { createServer } from 'node:net'
import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { chromium } from 'playwright'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const webRoot = path.resolve(__dirname, '..')
const outRoot = path.join(webRoot, 'exports', 'layers')
const VIEWPORT = { width: 1920, height: 1080 }
const BASE = '/windows-audio-router/'

const args = new Set(process.argv.slice(2))
const includeHeader = args.has('--header')

/** @type {{ folder: string, section: string, beat?: number, frame: string, layers: { id: string, file: string }[] }[]} */
const STACKS = [
  {
    folder: 'hero',
    section: 'hero',
    frame: 'hero',
    layers: [
      { id: 'hero-overlay-horizontal', file: '01-overlay-horizontal.png' },
      { id: 'hero-overlay-vertical', file: '02-overlay-vertical.png' },
      { id: 'hero-icon', file: '03-icon.png' },
      { id: 'hero-eyebrow', file: '04-eyebrow.png' },
      { id: 'hero-title', file: '05-title.png' },
      { id: 'hero-subtitle', file: '06-subtitle.png' },
      { id: 'hero-ctas', file: '07-ctas.png' },
      { id: 'hero-scroll-cue', file: '08-scroll-cue.png' },
    ],
  },
  {
    folder: 'product-01-dashboard',
    section: 'product',
    beat: 0,
    frame: 'product',
    layers: [
      { id: 'product-radial', file: '01-radial.png' },
      { id: 'product-ambient-glow', file: '02-ambient-glow.png' },
      { id: 'product-copy', file: '03-copy.png' },
      { id: 'product-visual', file: '04-visual.png' },
      { id: 'product-beat-dots', file: '05-beat-dots.png' },
    ],
  },
  {
    folder: 'product-02-tray',
    section: 'product',
    beat: 1,
    frame: 'product',
    layers: [
      { id: 'product-radial', file: '01-radial.png' },
      { id: 'product-copy', file: '02-copy.png' },
      { id: 'product-visual', file: '03-visual.png' },
      { id: 'product-beat-dots', file: '04-beat-dots.png' },
    ],
  },
  {
    folder: 'product-03-automate',
    section: 'product',
    beat: 2,
    frame: 'product',
    layers: [
      { id: 'product-radial', file: '01-radial.png' },
      { id: 'product-copy', file: '02-copy.png' },
      { id: 'product-visual', file: '03-visual.png' },
      { id: 'product-beat-dots', file: '04-beat-dots.png' },
    ],
  },
  {
    folder: 'live',
    section: 'live',
    frame: 'live',
    layers: [
      { id: 'live-radial', file: '01-radial.png' },
      { id: 'live-copy', file: '02-copy.png' },
      { id: 'live-glow', file: '03-glow.png' },
      { id: 'live-card', file: '04-card.png' },
    ],
  },
  {
    folder: 'footer',
    section: 'footer',
    frame: 'footer',
    layers: [
      { id: 'footer-mask-gradient', file: '01-mask-gradient.png' },
      { id: 'footer-gold-radial', file: '02-gold-radial.png' },
      { id: 'footer-ring', file: '03-ring.png' },
      { id: 'footer-icon', file: '04-icon.png' },
      { id: 'footer-title', file: '05-title.png' },
      { id: 'footer-body', file: '06-body.png' },
      { id: 'footer-ctas', file: '07-ctas.png' },
      { id: 'footer-meta', file: '08-meta.png' },
    ],
  },
]

if (includeHeader) {
  STACKS.push({
    folder: 'header',
    section: 'header',
    frame: 'header',
    layers: [{ id: 'header-nav', file: '01-nav.png' }],
  })
}

function getFreePort() {
  return new Promise((resolve, reject) => {
    const server = createServer()
    server.listen(0, '127.0.0.1', () => {
      const address = server.address()
      if (!address || typeof address === 'string') {
        server.close()
        reject(new Error('Could not allocate a free port'))
        return
      }
      const { port } = address
      server.close((err) => (err ? reject(err) : resolve(port)))
    })
    server.on('error', reject)
  })
}

function run(command, commandArgs, options = {}) {
  const isWin = process.platform === 'win32'
  return spawn(command, commandArgs, {
    cwd: webRoot,
    stdio: ['ignore', 'pipe', 'pipe'],
    shell: isWin,
    windowsHide: true,
    ...options,
  })
}

function stopProcess(child) {
  if (!child.pid || child.killed) return
  if (process.platform === 'win32') {
    spawn('taskkill', ['/pid', String(child.pid), '/T', '/F'], {
      stdio: 'ignore',
      windowsHide: true,
    })
    return
  }
  child.kill('SIGTERM')
}

async function waitForServer(url, timeoutMs = 60_000) {
  const start = Date.now()
  while (Date.now() - start < timeoutMs) {
    try {
      const res = await fetch(url)
      if (res.ok || res.status === 404) return
    } catch {
      // retry
    }
    await new Promise((r) => setTimeout(r, 250))
  }
  throw new Error(`Timed out waiting for ${url}`)
}

function sleep(ms) {
  return new Promise((r) => setTimeout(r, ms))
}

async function ensureBuild() {
  console.log('Building site…')
  await new Promise((resolve, reject) => {
    const child = run('npm', ['run', 'build'])
    let stderr = ''
    child.stderr.on('data', (chunk) => {
      stderr += chunk
      process.stderr.write(chunk)
    })
    child.stdout.on('data', (chunk) => process.stdout.write(chunk))
    child.on('exit', (code) => {
      if (code === 0) resolve()
      else reject(new Error(stderr || `build failed with code ${code}`))
    })
  })
}

/**
 * @param {import('playwright').Page} page
 * @param {string} layerId
 */
async function soloLayer(page, layerId) {
  await page.evaluate((id) => {
    document.documentElement.dataset.exportSolo = id
    document.querySelectorAll('[data-export-layer]').forEach((el) => {
      el.classList.toggle(
        'export-solo-active',
        el.getAttribute('data-export-layer') === id,
      )
    })
  }, layerId)
}

/**
 * @param {import('playwright').Page} page
 * @param {string} frame
 */
async function frameClip(page, frame) {
  const handle = page.locator(`[data-export-frame="${frame}"]`).first()
  await handle.waitFor({ state: 'visible', timeout: 15_000 })
  const box = await handle.boundingBox()
  if (!box) throw new Error(`No bounding box for frame "${frame}"`)

  // Sticky stages + header: full fixed canvas. Footer: panel bounds.
  if (frame !== 'footer') {
    return {
      x: 0,
      y: 0,
      width: VIEWPORT.width,
      height: VIEWPORT.height,
    }
  }

  const x = Math.max(0, Math.floor(box.x))
  const y = Math.max(0, Math.floor(box.y))
  const width = Math.min(Math.ceil(box.width), VIEWPORT.width - x)
  const height = Math.min(Math.ceil(box.height), VIEWPORT.height - y)
  return { x, y, width, height }
}

async function main() {
  await ensureBuild()

  const port = await getFreePort()
  const origin = `http://127.0.0.1:${port}`
  const preview = run('npx', [
    'vite',
    'preview',
    '--host',
    '127.0.0.1',
    '--port',
    String(port),
    '--strictPort',
  ])

  let previewLog = ''
  preview.stdout.on('data', (c) => {
    previewLog += c
  })
  preview.stderr.on('data', (c) => {
    previewLog += c
  })

  try {
    await waitForServer(`${origin}${BASE}`)
    await rm(outRoot, { recursive: true, force: true })
    await mkdir(outRoot, { recursive: true })

    const browser = await chromium.launch()
    const context = await browser.newContext({
      viewport: VIEWPORT,
      deviceScaleFactor: 1,
      reducedMotion: 'reduce',
    })
    const page = await context.newPage()

    let total = 0

    for (const stack of STACKS) {
      const params = new URLSearchParams({
        export: '1',
        section: stack.section,
      })
      if (typeof stack.beat === 'number') params.set('beat', String(stack.beat))
      if (stack.section === 'header') params.set('header', '1')

      const url = `${origin}${BASE}?${params.toString()}`
      console.log(`\n→ ${stack.folder}`)
      await page.goto(url, { waitUntil: 'networkidle' })
      await page.waitForFunction(
        () => document.documentElement.dataset.export === '1',
      )
      await sleep(300)

      const dir = path.join(outRoot, stack.folder)
      await mkdir(dir, { recursive: true })

      const clip = await frameClip(page, stack.frame)
      const sizes = new Set()

      for (const layer of stack.layers) {
        await soloLayer(page, layer.id)
        await sleep(40)
        const filePath = path.join(dir, layer.file)
        await page.screenshot({
          path: filePath,
          type: 'png',
          omitBackground: true,
          clip,
        })
        sizes.add(`${clip.width}x${clip.height}`)
        total += 1
        console.log(`  ${layer.file} (${clip.width}×${clip.height})`)
      }

      if (sizes.size !== 1) {
        throw new Error(
          `Inconsistent canvas sizes in ${stack.folder}: ${[...sizes].join(', ')}`,
        )
      }

      await writeFile(
        path.join(dir, 'README.txt'),
        [
          `AudioPresetSwitcher layer stack: ${stack.folder}`,
          `Canvas: ${clip.width}x${clip.height}`,
          'Import all PNGs into Photoshop as layers — they align 1:1.',
          'Background video/void intentionally omitted.',
          '',
        ].join('\n'),
        'utf8',
      )
    }

    await browser.close()
    console.log(`\nDone. ${total} PNGs → ${outRoot}`)
  } catch (err) {
    console.error(previewLog)
    throw err
  } finally {
    stopProcess(preview)
  }
}

main().catch((err) => {
  console.error(err)
  process.exit(1)
})
