# AudioPresetSwitcher landing page

Vite + React + TypeScript marketing site.

```bash
npm install
npm run dev
```

Production build (`base` is `/windows-audio-router/` for GitHub Pages):

```bash
npm run build
npm run preview
```

### Export layers for Photoshop

Builds the site and writes same-size transparent PNGs (one stack per section) to `exports/layers/`:

```bash
npx playwright install chromium
npm run export-layers
# optional header stack:
npm run export-layers:header
```

Hero / Product / Live / Header canvases are **1920×1080**. Footer uses the CTA panel bounds. Drop each folder’s PNGs into Photoshop as layers — they align 1:1. Video/void backgrounds are omitted on purpose.
