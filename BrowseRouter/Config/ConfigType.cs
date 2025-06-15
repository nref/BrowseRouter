namespace BrowseRouter.Config;

public record ConfigType(string Name)
{
  public static ConfigType Browsers { get; } = new("browsers");
  public static ConfigType Filters { get; } = new("filters");
  public static ConfigType Urls { get; } = new("urls");
  public static ConfigType Sources { get; } = new("sources");
  public static ConfigType Log { get; } = new("log");
  public static ConfigType Notify { get; } = new("Notify");
}
