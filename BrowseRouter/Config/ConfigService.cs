using System.Text.Json;
using BrowseRouter.Model;

namespace BrowseRouter.Config;

internal class ConfigService(Config config) : IConfigService
{
  public NotifyPreference GetNotifyPreference() => new()
  {
    IsEnabled = config.Notify.Enabled,
    IsSilent = config.Notify.Silent
  };

  public LogPreference GetLogPreference() => new()
  {
    IsEnabled = config.Log.Enabled,
    File = string.IsNullOrWhiteSpace(config.Log.File)
                      ? LogPreference.DefaultLogFile
                      : config.Log.File!
  };

  public IEnumerable<UrlPreference> GetUrlPreferences(ConfigType configType)
  {
    Dictionary<string, string> section = GetSection(configType);

    if (section.Count == 0 || config.Browsers.Count == 0)
      return [];

    List<UrlPreference> prefs = section
        .Select(kv => new UrlPreference
        {
          UrlPattern = kv.Key,
          Browser = config.Browsers.TryGetValue(kv.Value, out var loc)
                         ? new Browser { Name = kv.Value, Location = loc }
                         : null!
        })
        .Where(u => u.Browser != null)
        .ToList();

    return prefs;
  }

  public Dictionary<string, string> GetSection(ConfigType configType) => configType switch
  {
    _ when configType == ConfigType.Sources => config.Sources,
    _ when configType == ConfigType.Browsers => config.Browsers,
    _ when configType == ConfigType.Urls => config.Urls,
    _ => [],
  };

  public async Task<List<FilterPreference>> GetFiltersAsync()
  {
    if (string.IsNullOrWhiteSpace(config.FiltersFile))
      return [];

    var json = await File.ReadAllTextAsync(config.FiltersFile);
    return JsonSerializer.Deserialize(json, SourceGenerationContext.Default.FilterPreferenceList) ?? [];
  }
}