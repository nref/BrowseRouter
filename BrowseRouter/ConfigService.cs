namespace BrowseRouter;

public interface IConfigService
{
  IEnumerable<UrlPreference> GetUrlPreferences(string configType);
}

public class ConfigService : IConfigService
{
  /// <summary>
  /// Config lives in the same folder as the EXE, name "BrowserSelector.ini".
  /// </summary>
  public readonly string ConfigPath;

  public ConfigService()
  {
    // Fix for self-contained publishing
    ConfigPath = Path.Combine(Path.GetDirectoryName(App.ExePath)!, "config.ini");
  }

  public bool GetIsLogEnabled() => File.Exists(ConfigPath) 
    && GetConfig(ReadFile(), "log")
        .Select(SplitConfig)
        .Any(kvp => kvp.Key == "enabled" && kvp.Value == "true");

  public NotifyPreference GetNotifyPreference() => File.Exists(ConfigPath) switch
  {
    false => new NotifyPreference(),
    true => GetNotifyPreferenceCore(),
  };

  private NotifyPreference GetNotifyPreferenceCore()
  {
    Dictionary<string, string> notifyConfig = GetConfig(ReadFile(), "notify")
      .Select(SplitConfig)
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    return new NotifyPreference
    {
      IsEnabled = notifyConfig.TryGetValue("enabled", out string? enabled) switch
      {
        false => true,
        true => bool.TryParse(enabled, out bool isEnabled) && isEnabled,
      }
    };
  }

  public LogPreference GetLogPreference() => File.Exists(ConfigPath) switch
  {
    false => new LogPreference(),
    true => GetLogPreferenceCore(),
  };

  private LogPreference GetLogPreferenceCore()
  {
    Dictionary<string, string> logConfig = GetConfig(ReadFile(), "log")
      .Select(SplitConfig)
      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    return new LogPreference
    {
      IsEnabled = logConfig.TryGetValue("enabled", out string? enabled) && bool.TryParse(enabled, out bool isEnabled) && isEnabled,
      File = logConfig.TryGetValue("file", out string? path) ? path.Replace("\"", "") : LogPreference.DefaultLogFile,
    };
  }

  public IEnumerable<UrlPreference> GetUrlPreferences(string configType)
  {
    if (!File.Exists(ConfigPath))
      throw new InvalidOperationException($"The config file was not found: {ConfigPath}");

    // Poor mans INI file reading... Skip comment lines (TODO: support comments on other lines).
    IEnumerable<string> configLines = ReadFile();

    // Read the browsers section into a dictionary.
    Dictionary<string, Browser> browsers = GetConfig(configLines, "browsers")
      .Select(SplitConfig)
      .Select(kvp => new Browser { Name = kvp.Key, Location = kvp.Value })
      .ToDictionary(b => b.Name);

    if (browsers.Count == 0)
    {
      // There weren't any configured browsers
      return new List<UrlPreference>();
    }

    // Read the url preferences
    IEnumerable<UrlPreference> urls = GetConfig(configLines, configType)
      .Select(SplitConfig)
      .Select(kvp => new UrlPreference { UrlPattern = kvp.Key, Browser = browsers[kvp.Value] })
      .Where(up => up.Browser != null);

    if (configType == "urls")
      urls = urls.Union(new[] { new UrlPreference { UrlPattern = "*", Browser = browsers.FirstOrDefault().Value } }); // Add a catch-all that uses the first browser

    return urls;
  }

  private IEnumerable<string> ReadFile()
  {
    return File.ReadAllLines(ConfigPath)
      .Select(l => l.Trim())
      .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith(';') && !l.StartsWith('#'));
  }

  private IEnumerable<string> GetConfig(IEnumerable<string> configLines, string configName)
  {
    // Read everything from [configName] up to the next [section].
    return configLines
      .SkipWhile(l => !l.StartsWith($"[{configName}]", StringComparison.OrdinalIgnoreCase))
      .Skip(1)
      .TakeWhile(l => !l.StartsWith("[", StringComparison.OrdinalIgnoreCase))
      .Where(l => l.Contains('='));
  }

  /// <summary>
  /// Splits a line on the first '=' (poor INI parsing).
  /// </summary>
  private KeyValuePair<string, string> SplitConfig(string configLine)
  {
    string[] parts = configLine.Split('=', 2);
    return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
  }
}
