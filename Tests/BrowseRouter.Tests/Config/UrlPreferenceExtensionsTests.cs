using BrowseRouter.Config;
using BrowseRouter.Model;

namespace BrowseRouter.Tests.Config;

public class UrlPreferenceExtensionsTests
{
  private readonly Browser _edge;

  public UrlPreferenceExtensionsTests()
  {
    _edge = new Browser
    {
      Name = "edge",
      Location = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
    };
  }

  [Fact]
  public void GetDomainAndPatternRegular()
  {
    var preferences = new List<UrlPreference>();

    {
      var domainPref = new UrlPreference
      {
        UrlPattern = "*.microsoft.com",
        Browser = _edge
      };

      var (domain, pattern) =
          domainPref.GetDomainAndPattern(new Uri("https://docs.microsoft.com/en-us/"));

      domain.Should().Be("docs.microsoft.com");
      pattern.Should().Be("^.*\\.microsoft\\.com$");
      preferences.Add(domainPref);
    }

    {
      var domainPref = new UrlPreference
      {
        UrlPattern = "microsoft.com",
        Browser = _edge
      };

      var (domain, pattern) =
          domainPref.GetDomainAndPattern(new Uri("https://www.microsoft.com/de-de/search/explore?q=qwerty"));

      domain.Should().Be("www.microsoft.com");
      pattern.Should().Be("^microsoft\\.com$");
      preferences.Add(domainPref);
    }

    {
      var domainPref = new UrlPreference
      {
        UrlPattern = "*visualstudio.com",
        Browser = _edge
      };

      var (domain, pattern) =
          domainPref.GetDomainAndPattern(new Uri("https://my.visualstudio.com/?auth_redirect=true"));

      domain.Should().Be("my.visualstudio.com");
      pattern.Should().Be("^.*visualstudio\\.com$");
      preferences.Add(domainPref);
    }

    preferences.TryGetPreference(new Uri("https://docs.microsoft.com/en-us/"), out var pref1).Should().BeTrue();
    preferences.TryGetPreference(new Uri("https://www.microsoft.com/de-de/search/explore?q=qwerty"), out var pref2).Should().BeTrue();
    preferences.TryGetPreference(new Uri("https://my.visualstudio.com/?auth_redirect=true"), out var pref3).Should().BeTrue();
  }

  [Fact]
  public void GetDomainAndPatternRegex()
  {
    var preferences = new List<UrlPreference>();

    {
      var domainPref = new UrlPreference
      {
        UrlPattern = "/(?:[0-9]{1,3}\\.){3}[0-9]{1,3}/",
        Browser = _edge
      };

      var (domain, pattern) =
          domainPref.GetDomainAndPattern(new Uri("https://192.168.69.420/proxy"));

      domain.Should().Be("192.168.69.420/proxy");
      pattern.Should().Be("(?:[0-9]{1,3}\\.){3}[0-9]{1,3}");
      preferences.Add(domainPref);
    }

    preferences.TryGetPreference(new Uri("https://192.168.69.420/proxy?q=base"), out var pref1).Should().BeTrue();
  }

  [Fact]
  public void GetDomainAndPatternQuery()
  {
    var preferences = new List<UrlPreference>();

    {
      var domainPref = new UrlPreference
      {
        UrlPattern = "?www.youtube.com/redirect?*reddit.com*?",
        Browser = _edge
      };

      var (domain, pattern) =
          domainPref.GetDomainAndPattern(new Uri("https://www.youtube.com/redirect" +
          "?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine"));

      domain.Should().Be("www.youtube.com/redirect?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine");
      pattern.Should().Be("^www\\.youtube\\.com/redirect\\?.*reddit\\.com.*$");
      preferences.Add(domainPref);
    }

    preferences.TryGetPreference(new Uri("https://www.youtube.com/redirect" +
      "?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine"), out var pref1)
      .Should()
      .BeTrue();
  }
}