using BrowseRouter.Infrastructure;

namespace BrowseRouter.Config;

public static class ConfigServiceFactory
{
  public static async Task<IConfigService> CreateAsync()
  {
    var dir = Path.GetDirectoryName(App.ExePath)!;
    var jsonPath = Path.Combine(dir, "config.json");

    IConfigLoader loader = File.Exists(jsonPath)
        ? new JsonConfigLoader(jsonPath)
        : new IniConfigLoader(Path.Combine(dir, "config.ini"));

    var config = await loader.LoadAsync();

    CatchAllConfig.AddTo(config);

    return new ConfigService(config);
  }
}
