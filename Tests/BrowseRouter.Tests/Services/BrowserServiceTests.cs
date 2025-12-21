using BrowseRouter.Config;
using BrowseRouter.Services;

namespace BrowseRouter.Tests.Services;

public class BrowserServiceTests : IAsyncLifetime
{
  private BrowseRouter.Config.Config _config = null!; // Set in InitializeAsync

  public Task InitializeAsync()
  {
    _config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = new Dictionary<string, string>
      {
        ["fake"] = "fake-browser.exe",
      },
    };

    CatchAllConfig.AddTo(_config);
    return Task.CompletedTask;
  }

  public Task DisposeAsync() => Task.CompletedTask;

  [Theory]
  [InlineData("https://example.com")]
  [InlineData("https://statics.teams.cdn.office.net/evergreen-assets/safelinks/1/atp-safelinks.html?url=https%3A%2F%2Fwww.example.org%2Fpath%2F")]
  public async Task HandlesUrl(string url)
  {
    var spy = new SpyProcessService();
    await new BrowserService(new ConfigService(_config), new EmptyNotifyService(), spy)
      .LaunchAsync(url, "Fake Window");

    spy.LastPath.Should().Be("fake-browser.exe");
  }

  [Fact]
  public async Task SubstitutesUrlTagInUnquotedBrowserArgs()
  {
    // Issue #94: User configures browser with args containing {url} tag
    // e.g., for Firefox containers: "firefox.exe ext+container:name=Work&url={url}"
    var config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = new Dictionary<string, string>
      {
        ["ff-work"] = "firefox.exe ext+container:name=Work&url={url}",
      },
    };
    CatchAllConfig.AddTo(config);

    var spy = new SpyProcessService();
    await new BrowserService(new ConfigService(config), new EmptyNotifyService(), spy)
      .LaunchAsync("https://example.com/path", "Fake Window");

    spy.LastPath.Should().Be("firefox.exe");
    spy.LastArgs.Should().Be("ext+container:name=Work&url=https://example.com/path");
  }

  [Fact]
  public async Task SubstitutesUrlTagInQuotedBrowserArgs()
  {
    // When browser path is quoted, args with {url} should still work
    var config = BrowseRouter.Config.Config.Empty with
    {
      Browsers = new Dictionary<string, string>
      {
        ["ff-work"] = "\"firefox.exe\" ext+container:name=Work&url={url}",
      },
    };
    CatchAllConfig.AddTo(config);

    var spy = new SpyProcessService();
    await new BrowserService(new ConfigService(config), new EmptyNotifyService(), spy)
      .LaunchAsync("https://example.com/path", "Fake Window");

    spy.LastPath.Should().Be("firefox.exe");
    spy.LastArgs.Should().Be("ext+container:name=Work&url=https://example.com/path");
  }
}