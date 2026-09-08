using BrowseRouter.Config;
using BrowseRouter.Config.Loaders;

namespace BrowseRouter.Tests.Config.Loaders.JsonConfigLoaderTests;

public class LoadAsyncMethod
{
  [Fact]
  public async Task ShippedConfig_TranslatesToRulesEndingInACatchAll()
  {
    ConfigFile file = await new JsonConfigLoader("config.json").LoadAsync();

    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = ConfigTranslator.ToConfig(file);

    warnings.Should().BeEmpty();
    config.Rules.Should().HaveCountGreaterThan(1);
    config.Rules.Last().Condition.Should().Be(new Match.Url("*", Match.UrlSyntax.DomainWildcard));
    config.FiltersFile.Should().Be("filters.json");
  }

  [Fact]
  public async Task ShippedConfig_NamesOnlyBrowsersItDefines()
  {
    ConfigFile file = await new JsonConfigLoader("config.json").LoadAsync();

    (BrowseRouter.Config.Config config, _) = ConfigTranslator.ToConfig(file);

    config.Rules.Select(r => r.Browser.Name).Should().BeSubsetOf(file.Browsers!.Keys);
  }
}
