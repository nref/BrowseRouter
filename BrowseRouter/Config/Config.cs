namespace BrowseRouter.Config;

internal record Config(
    NotifyConfig Notify,
    LogConfig Log,
    Dictionary<string, string> Browsers,
    Dictionary<string, string> Sources,
    Dictionary<string, string> Urls,
    string? FiltersFile
)
{
  public static Config Empty => new(NotifyConfig.Empty, LogConfig.Empty, [], [], [], string.Empty);


}
