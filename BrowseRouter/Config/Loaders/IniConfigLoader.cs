namespace BrowseRouter.Config.Loaders;

internal class IniConfigLoader(string path) : IConfigLoader
{
  public async Task<ConfigFile> LoadAsync()
  {
    List<string> lines = (await File.ReadAllLinesAsync(path))
      .Select(l => l.Trim())
      .Where(l => !string.IsNullOrWhiteSpace(l) && !";#".Contains(l[0]))
      .ToList();

    IEnumerable<KeyValuePair<string, string>> Section(string name)
        => lines
           .SkipWhile(l => !l.StartsWith($"[{name}]", StringComparison.OrdinalIgnoreCase))
           .Skip(1)
           .TakeWhile(l => !l.StartsWith('['))
           .Where(l => l.Contains('='))
           .Select(l =>
           {
             string[] p = l.Split('=', 2);
             return KeyValuePair.Create(p[0].Trim(), p[1].Trim());
           });

    NotifyConfig notify = NotifyConfig.Empty;

    foreach (KeyValuePair<string, string> kv in Section("notify"))
    {
      if (kv.Key.Equals("enabled", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out bool enabled))
        notify = notify with { Enabled = enabled };

      if (kv.Key.Equals("silent", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out bool silent))
        notify = notify with { Silent = silent };
    }

    LogConfig log = LogConfig.Empty;

    foreach (KeyValuePair<string, string> kv in Section("log"))
    {
      if (kv.Key.Equals("enabled", StringComparison.OrdinalIgnoreCase) && bool.TryParse(kv.Value, out bool enabled))
        log = log with { Enabled = enabled };

      if (kv.Key.Equals("file", StringComparison.OrdinalIgnoreCase))
        log = log with { File = kv.Value.Replace("\"", "") };
    }

    Dictionary<string, string> browsers = [];

    foreach (KeyValuePair<string, string> kv in Section("browsers"))
      browsers[kv.Key] = kv.Value;

    return new ConfigFile(
      notify,
      log,
      browsers,
      null,
      Section("sources").Select(kv => new PatternBrowser(kv.Key, kv.Value)).ToList(),
      null,
      Section("filters").FirstOrDefault().Value);
  }
}
