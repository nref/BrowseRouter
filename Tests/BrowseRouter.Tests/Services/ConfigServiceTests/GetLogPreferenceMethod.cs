using BrowseRouter.Config;
using BrowseRouter.Services;

namespace BrowseRouter.Tests.Services.ConfigServiceTests;

public class GetLogPreferenceMethod
{
  [Fact]
  public void NoFileConfigured_UsesDefaultLogFile()
  {
    ConfigService service = new(BrowseRouter.Config.Config.Empty);

    service.GetLogPreference().File.Should().Be(LogPreference.DefaultLogFile);
  }

  [Fact]
  public void RelativeFileConfigured_IsResolvedAgainstTheExecutable()
  {
    BrowseRouter.Config.Config config = BrowseRouter.Config.Config.Empty with
    {
      Log = new LogConfig(Enabled: true, File: "browserouter.log")
    };

    ConfigService service = new(config);

    service.GetLogPreference().File.Should().EndWith("browserouter.log");
    service.GetLogPreference().IsEnabled.Should().BeTrue();
  }
}
