using System.Text.Json;
using BrowseRouter.Config;
using BrowseRouter.Infrastructure;

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

  public IReadOnlyList<Rule> GetRules() => config.Rules;

  public async Task<List<FilterPreference>> GetFiltersAsync()
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