namespace BrowseRouter.Config;

public interface IConfigService
{
  NotifyPreference GetNotifyPreference();
  LogPreference GetLogPreference();
  IEnumerable<UrlPreference> GetUrlPreferences(ConfigType configType);
  Task<List<FilterPreference>> GetFiltersAsync();
}
