namespace BrowseRouter.Config;

internal record Config(
    NotifyConfig Notify,
    LogConfig Log,
    IReadOnlyList<Rule> Rules,
    string? FiltersFile
)
{
  public static Config Empty => new(NotifyConfig.Empty, LogConfig.Empty, [], null);
}
