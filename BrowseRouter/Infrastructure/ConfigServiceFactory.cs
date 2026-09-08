using BrowseRouter.Config;
using BrowseRouter.Config.Loaders;
using BrowseRouter.Services;

namespace BrowseRouter.Infrastructure;

public static class ConfigServiceFactory
{
  public static async Task<IConfigService> CreateAsync()
  {
    var dir = Path.GetDirectoryName(App.ExePath)!;
    var jsonPath = Path.Combine(dir, "config.json");

    IConfigLoader loader = File.Exists(jsonPath)
        ? new JsonConfigLoader(jsonPath)
        : new IniConfigLoader(Path.Combine(dir, "config.ini"));

    (Config.Config config, IReadOnlyList<string> warnings) = ConfigTranslator.ToConfig(await loader.LoadAsync());

    ConfigService service = new(config);
    Log.Preference = service.GetLogPreference();

    foreach (string warning in warnings)
      Log.Write(warning);

    return service;
  }
}
