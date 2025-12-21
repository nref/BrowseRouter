using System.Text.Json;
using BrowseRouter.Config;
using BrowseRouter.Infrastructure;
using BrowseRouter.Model;

namespace BrowseRouter.Services;

internal class ConfigService(Config.Config config) : IConfigService
{
  public NotifyPreference GetNotifyPreference() => new()
  {
    IsEnabled = config.Notify.Enabled,
    IsSilent = config.Notify.Silent
  };

  public LogPreference GetLogPreference() => new()
  {
    IsEnabled = config.Log.Enabled,
    File = string.IsNullOrWhiteSpace(config.Log.File) switch
    {
      true => LogPreference.DefaultLogFile,
      false => Path.IsPathFullyQualified(config.Log.File) switch
      {
        true => config.Log.File,
        false => Path.Combine(App.ExePath, config.Log.File)
      }
    }
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

  public virtual async Task<List<FilterPreference>> GetFiltersAsync()
  {
    if (string.IsNullOrWhiteSpace(config.FiltersFile))
      return [];

    var dir = Path.GetDirectoryName(App.ExePath)!;
    string path = Path.IsPathFullyQualified(config.FiltersFile) switch
    {
      true => config.FiltersFile,
      false => Path.Combine(dir, config.FiltersFile)
    };

    var json = await File.ReadAllTextAsync(path);
    return JsonSerializer.Deserialize(json, SourceGenerationContext.Default.FilterPreferenceList) ?? [];
  }
}