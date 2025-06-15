namespace BrowseRouter.Config;

internal static class CatchAllConfig
{
  public static void AddTo(Config config)
  {
    // Add a catch-all that uses the first browser
    if (!config.Urls.ContainsKey("*"))
      config.Urls["*"] = config.Browsers.FirstOrDefault().Key;
  }
}