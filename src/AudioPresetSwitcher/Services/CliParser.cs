using AudioPresetSwitcher.Models;

namespace AudioPresetSwitcher.Services;

public static class CliParser
{
    public static StartupOptions Parse(IReadOnlyList<string> args)
    {
        var options = new StartupOptions();
        for (var i = 0; i < args.Count; i++)
        {
            var arg = args[i];
            if ((arg is "--preset" or "-p") && i + 1 < args.Count)
            {
                options.PresetName = args[++i];
            }
            else if (arg is "--preset-index" && i + 1 < args.Count && int.TryParse(args[i + 1], out var index))
            {
                options.PresetIndex = index;
                i++;
            }
        }

        options.ShowWindow = !options.HasPresetRequest;
        return options;
    }
}
