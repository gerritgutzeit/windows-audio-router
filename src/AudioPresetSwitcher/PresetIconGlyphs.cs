using AudioPresetSwitcher.Models;
using Wpf.Ui.Controls;

namespace AudioPresetSwitcher;

public static class PresetIconGlyphs
{
    public static SymbolRegular ToSymbol(PresetIcon icon) =>
        icon == PresetIcon.Speaker
            ? SymbolRegular.Speaker224
            : SymbolRegular.HeadphonesSoundWave24;
}
