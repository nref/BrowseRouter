using BrowseRouter.Config;
using BrowseRouter.Services;

namespace BrowseRouter.Tests.Services;

public class ConfigServiceTests
{
  [Fact]
  public void GetUrlPreferences_NoBrowsers_ReturnsEmpty()
  {
    var config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = []
    };

    var service = new ConfigService(config);
    var result = service.GetUrlPreferences(new ConfigType("anything"));
    result.Should().BeEmpty();
  }

  [Fact]
  public void GetUrlPreferences_LoadsUrls()
  {
    var config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = new Dictionary<string, string>
      {
        ["chrome"] = "C:\\chrome.exe",
        ["firefox"] = "C:\\firefox.exe",
      },
      Urls = new Dictionary<string, string>
      {
        ["site1"] = "chrome",
        ["site2"] = "firefox",
      },
    };

    var service = new ConfigService(config);

    var prefs = service.GetUrlPreferences(ConfigType.Urls).ToList();

    prefs.Should().HaveCount(2);
    prefs.Should().Contain(up =>
        up.UrlPattern == "site1" &&
        up.Browser.Name == "chrome" &&
        up.Browser.Location == @"C:\chrome.exe"
    );
    prefs.Should().Contain(up =>
        up.UrlPattern == "site2" &&
        up.Browser.Name == "firefox" &&
        up.Browser.Location == @"C:\firefox.exe"
    );
  }

  [Fact]
  public void GetUrlPreferences_ConfigTypeUrls_AppendsCatchAll()
  {
    var config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = new Dictionary<string, string>
      {
        ["edge"] = "C:\\edge.exe",
      },
      Urls = new Dictionary<string, string>
      {
        ["*.test.com"] = "edge",
      },
    };
    CatchAllConfig.AddTo(config);

    var service = new ConfigService(config);

    var prefs = service.GetUrlPreferences(ConfigType.Urls).ToList();

    // should have the one from config + the "*" catch-all
    prefs.Should().HaveCount(2);
    prefs.Should().Contain(up => up.UrlPattern == "*.test.com" && up.Browser.Name == "edge");
    prefs.Should().Contain(up => up.UrlPattern == "*" && up.Browser.Name == "edge");
  }
}
