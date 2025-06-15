namespace BrowseRouter.Config;

internal class IniConfigLoader(string path) : IConfigLoader
{
  public async Task<Config> LoadAsync()
  {
    var lines = (await File.ReadAllLinesAsync(path))
      .Select(l => l.Trim())
      .Where(l => !string.IsNullOrWhiteSpace(l) && !";#".Contains(l[0]))
      .ToList();

    var config = Config.Empty;

    // helper to grab k=v under [section]
    IEnumerable<KeyValuePair<string, string>> Section(string name)
        => lines
           .SkipWhile(l => !l.StartsWith($"[{name}]", StringComparison.OrdinalIgnoreCase))
           .Skip(1)
           .TakeWhile(l => !l.StartsWith("["))
           .Where(l => l.Contains('='))
           .Select(l =>
           {
             var p = l.Split('=', 2);
             return KeyValuePair.Create(p[0].Trim(), p[1].Trim());
           });

    // populate
    foreach (var kv in Section("notify"))
    {
      if (kv.Key.Equals("enabled", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out var e))
        config = config with { Notify = config.Notify with { Enabled = e } };

      if (kv.Key.Equals("silent", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out var s))
        config = config with { Notify = config.Notify with { Silent = s } };
    }

    foreach (var kv in Section("log"))
    {
      if (kv.Key.Equals("enabled", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out var e)) 
        config = config with { Log = config.Log with { Enabled = e } };
      if (kv.Key.Equals("file", StringComparison.OrdinalIgnoreCase))
        config = config with { Log = config.Log with { File = kv.Value.Replace("\"", "") } };
    }

    foreach (var kv in Section("browsers"))
      config.Browsers[kv.Key] = kv.Value;

    foreach (var kv in Section("sources"))
      config.Sources[kv.Key] = kv.Value;

    config = config with { FiltersFile = Section("filters").FirstOrDefault().Value };

    return config;
  }
}
