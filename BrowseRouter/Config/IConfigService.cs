namespace BrowseRouter.Config;

public interface IConfigService
{
  NotifyPreference GetNotifyPreference();
  LogPreference GetLogPreference();
  IReadOnlyList<Rule> GetRules();
  Task<List<FilterPreference>> GetFiltersAsync();
}
