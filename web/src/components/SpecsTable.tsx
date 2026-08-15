import { GlowCard } from './GlowCard'
import { SectionReveal } from './SectionReveal'

const SPECS = [
  {
    label: 'Architecture',
    value: 'C# / .NET 8 / WPF (WPF-UI Mica material)',
  },
  {
    label: 'OS',
    value: 'Windows 10 (1809+) & Windows 11',
  },
  {
    label: 'Config path',
    value: '%AppData%\\AudioPresetSwitcher\\settings.json',
  },
  {
    label: 'Installer',
    value: 'Velopack — self-contained Setup.exe via GitHub Releases',
  },
  {
    label: 'Audio stack',
    value: 'NAudio WASAPI — enumerate, meters, device watch',
  },
] as const

export function SpecsTable() {
  return (
    <SectionReveal id="docs" className="mx-auto max-w-6xl px-4 py-20 sm:px-6">
      <p className="font-mono text-xs tracking-widest text-accent-bright">04</p>
      <h2 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
        Under the hood
      </h2>
      <p className="mt-3 max-w-2xl text-muted">
        Minimal surface area. Native Windows app — no Electron, no cloud account.
      </p>

      <GlowCard className="mt-10 overflow-hidden p-0">
        <table className="w-full text-left text-sm">
          <tbody>
            {SPECS.map((row, i) => (
              <tr
                key={row.label}
                className={i < SPECS.length - 1 ? 'border-b border-white/8' : ''}
              >
                <th
                  scope="row"
                  className="w-36 shrink-0 px-5 py-4 align-top font-medium text-white sm:w-48"
                >
                  {row.label}
                </th>
                <td className="px-5 py-4 text-muted sm:font-mono sm:text-xs">
                  {row.value}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </GlowCard>
    </SectionReveal>
  )
}
