using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Services;

internal static class DeviceKeywordMatcher
{
    public static MMDevice? Match(
        string? keyword,
        IEnumerable<MMDevice> devices,
        Func<DataFlow, Role, string?> getDefaultId,
        DataFlow flow)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return null;
        }

        var term = keyword.Trim();
        var candidates = devices
            .Where(d => d.FriendlyName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (candidates.Count == 0)
        {
            return null;
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        var ranked = candidates
            .Select(device => new
            {
                Device = device,
                Exact = device.FriendlyName.Equals(term, StringComparison.OrdinalIgnoreCase),
                Score = term.Length / (double)Math.Max(device.FriendlyName.Length, 1)
            })
            .OrderByDescending(x => x.Exact)
            .ThenByDescending(x => x.Score)
            .ThenBy(x => x.Device.FriendlyName.Length)
            .ToList();

        var top = ranked[0];
        var tied = ranked
            .Where(x => x.Exact == top.Exact && Math.Abs(x.Score - top.Score) < 0.0001)
            .ToList();
        if (tied.Count > 1)
        {
            var defaultId = getDefaultId(flow, Role.Multimedia);
            var preferred = tied.FirstOrDefault(x => x.Device.ID == defaultId);
            if (preferred is not null)
            {
                return preferred.Device;
            }
        }

        return top.Device;
    }
}
