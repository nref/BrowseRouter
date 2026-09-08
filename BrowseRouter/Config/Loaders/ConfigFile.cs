using System.Text.Json.Serialization;

namespace BrowseRouter.Config.Loaders;

internal record ConfigFile(
    NotifyConfig? Notify,
    LogConfig? Log,
    Dictionary<string, string>? Browsers,
    List<RuleConfig>? Rules,
    [property: JsonConverter(typeof(LegacySectionConverter))] List<PatternBrowser>? Sources,
    [property: JsonConverter(typeof(LegacySectionConverter))] List<PatternBrowser>? Urls,
    string? FiltersFile
)
{
  public static ConfigFile Empty => new(null, null, null, null, null, null, null);
}
