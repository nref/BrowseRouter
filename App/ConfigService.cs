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
    this.ConfigPath = ComputeConfigPath();
    Log.Write($"Using config file: {ConfigPath}");
  }

  /// <summary>
  /// Returns the best choice from a list of candidate configuration file paths.
  /// The candidates are, in descending order of priority:
  /// <list type="number">
  ///   <item><c>config.ini</c> in the current working directory</item>
  ///   <item><c>%UserProfile%\.config\BrowseRouter\config.ini</c> or <c>$HOME/.config/BrowseRouter/config.ini</c></item>
  ///   <item><c>config.ini</c> in the applicaton's directory</item>
  /// </list>
  /// The last item in the above list is the default if none of the other files exist.
  /// </summary>
  public string ComputeConfigPath()
  {
    const string ConfigFilename = "config.ini";
    string[] BasePaths = {
      Directory.GetCurrentDirectory(),
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", App.FriendlyName),
      App.BaseDir,
    };
    Log.Write($"Candidate config base paths: {string.Join(", ", BasePaths)}");
    var configPath = from basePath in BasePaths
                     let candidateConfigPath = Path.Combine(basePath, ConfigFilename)
                     where File.Exists(candidateConfigPath)
                     select candidateConfigPath;
    return configPath.FirstOrDefault(BasePaths.Last());
  }

  public IEnumerable<UrlPreference> GetUrlPreferences(string configType)
  {
    if (!File.Exists(ConfigPath))
      throw new InvalidOperationException($"The config file was not found: {ConfigPath}");

    // Poor mans INI file reading... Skip comment lines (TODO: support comments on other lines).
    var configLines =
      File.ReadAllLines(ConfigPath)
      .Select(l => l.Trim())
      .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith(";") && !l.StartsWith("#"));

    // Read the browsers section into a dictionary.
    var browsers = GetConfig(configLines, "browsers")
      .Select(SplitConfig)
      .Select(kvp => new Browser(name: kvp.Key, location: kvp.Value))
      .ToDictionary(b => b.Name);

    if (!browsers.Any())
    {
      // There weren't any configured browsers
      return new List<UrlPreference>();
    }

    // Read the url preferences
    var urls = GetConfig(configLines, configType)
      .Select(SplitConfig)
      .Select(kvp => new UrlPreference(urlPattern: kvp.Key, browser: browsers[kvp.Value]))
      .Where(up => up.Browser != null);

    if (configType == "urls")
      urls = urls.Union(new[] { new UrlPreference(urlPattern: "*", browser: browsers.FirstOrDefault().Value) }); // Add a catch-all that uses the first browser

    return urls;
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
    var parts = configLine.Split(new[] { '=' }, 2);
    return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
  }
}
