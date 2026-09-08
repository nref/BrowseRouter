using BrowseRouter.Config.Loaders;
using BrowseRouter.Services;

namespace BrowseRouter.Tests.Services.BrowserServiceTests;

public class LaunchAsyncMethod
{
  private static readonly Dictionary<string, string> Browsers = new()
  {
    ["fake"] = "fake-browser.exe",
    ["chrome"] = "chrome.exe",
    ["ff-work"] = "\"Mozilla Firefox\\firefox.exe\" ext+container:name=Work&url={url}",
  };

  [Theory]
  [InlineData("https://example.com")]
  [InlineData("https://teams.public.onecdn.static.microsoft/evergreen-assets/safelinks/1/atp-safelinks.html?url=https%3A%2F%2Fwww.example.org%2Fpath%2F")]
  public async Task CatchAllOnly_UsesTheFirstBrowser(string url)
  {
    SpyProcessService spy = await Launch(ConfigFile.Empty with { Browsers = Browsers }, url, "Fake Window");

    spy.LastPath.Should().Be("fake-browser.exe");
  }

  [Fact]
  public async Task FirefoxContainerBrowser_ReceivesTheUrlAsAnArgument()
  {
    ConfigFile file = ConfigFile.Empty with
    {
      Browsers = Browsers,
      Rules = [new RuleConfig("*work.test*", null, "ff-work")],
    };

    SpyProcessService spy = await Launch(file, "https://www.work.test/foobar", "Fake Window");

    spy.LastPath.Should().Be("Mozilla Firefox\\firefox.exe");
    spy.LastArgs.Should().Be("ext+container:name=Work&url=https://www.work.test/foobar");
  }

  [Fact]
  public async Task UrlRuleAboveASourceRule_Wins()
  {
    ConfigFile file = ConfigFile.Empty with
    {
      Browsers = Browsers,
      Rules =
      [
        new RuleConfig("*.youtube.com", null, "chrome"),
        new RuleConfig(null, "*Discord*", "ff-work"),
      ],
    };

    SpyProcessService spy = await Launch(file, "https://www.youtube.com/watch?v=x", "Chat - Discord");

    spy.LastPath.Should().Be("chrome.exe");
  }

  [Fact]
  public async Task UrlRuleBelowASourceRule_Loses()
  {
    ConfigFile file = ConfigFile.Empty with
    {
      Browsers = Browsers,
      Rules =
      [
        new RuleConfig(null, "*Discord*", "ff-work"),
        new RuleConfig("*.youtube.com", null, "chrome"),
      ],
    };

    SpyProcessService spy = await Launch(file, "https://www.youtube.com/watch?v=x", "Chat - Discord");

    spy.LastPath.Should().Be("Mozilla Firefox\\firefox.exe");
  }

  [Fact]
  public async Task LegacySections_KeepSourcesAheadOfUrls()
  {
    ConfigFile file = ConfigFile.Empty with
    {
      Browsers = Browsers,
      Sources = [new PatternBrowser("*Discord*", "ff-work")],
      Urls = [new PatternBrowser("*.youtube.com", "chrome")],
    };

    SpyProcessService spy = await Launch(file, "https://www.youtube.com/watch?v=x", "Chat - Discord");

    spy.LastPath.Should().Be("Mozilla Firefox\\firefox.exe");
  }

  private static async Task<SpyProcessService> Launch(ConfigFile file, string url, string windowTitle)
  {
    (BrowseRouter.Config.Config config, _) = ConfigTranslator.ToConfig(file);
    SpyProcessService spy = new();

    await new BrowserService(new ConfigService(config), new EmptyNotifyService(), spy)
      .LaunchAsync(url, windowTitle);

    return spy;
  }
}
