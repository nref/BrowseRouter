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
    this.ConfigPath = Path.Combine(Path.GetDirectoryName(App.ExePath)!, "config.ini");
  }
  
  private IEnumerable<string>? _configLines = null;
  private IEnumerable<string> ConfigLines => _configLines ??= ReadFile();

  public bool GetIsLogEnabled() => File.Exists(ConfigPath) 
    && GetConfig("log")
        .Select(SplitConfig)
        .Any(kvp => kvp.Key == "enabled" && kvp.Value == "true");

  public NotifyPreference GetNotifyPreference() => File.Exists(ConfigPath) switch
  {
    false => new NotifyPreference(),
    true => GetNotifyPreferenceCore(),
  };

  private NotifyPreference GetNotifyPreferenceCore()
  {
    var notifyConfig = GetConfig("notify")
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
    var logConfig = GetConfig("log")
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

    // Read the browsers section into a dictionary.
    var browsers = GetConfig("browsers")
      .Select(SplitConfig)
      .Select(kvp => new Browser { Name = kvp.Key, Location = kvp.Value })
      .ToDictionary(b => b.Name);

    if (!browsers.Any())
    {
      // There weren't any configured browsers
      return new List<UrlPreference>();
    }

    // Read the url preferences
    var urls = GetConfig(configType)
      .Select(SplitConfig)
      .Select(kvp => new UrlPreference { UrlPattern = kvp.Key, Browser = browsers[kvp.Value] })
      .Where(up => up.Browser != null);

    if (configType == "urls")
      urls = urls.Union(new[] { new UrlPreference { UrlPattern = "*", Browser = browsers.FirstOrDefault().Value } }); // Add a catch-all that uses the first browser

    return urls;
  }

  private IEnumerable<string> ReadFile()
  {
    if (!File.Exists(ConfigPath))
      throw new InvalidOperationException($"The config file was not found at \"{ConfigPath}\"");

    // Poor mans INI file reading... Skip comment lines (TODO: support comments on other lines).
    return File.ReadAllLines(ConfigPath)
          .Select(l => l.Trim())
          .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith(';') && !l.StartsWith('#'));
  }

  private IEnumerable<string> GetConfig(string configName)
  {
    // Read everything from [configName] up to the next [section].
    return ConfigLines
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
    var parts = configLine.Split(new[] { '=' }, 2);
    return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
  }
}
