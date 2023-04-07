namespace BrowserRouter.Tests
{
  public class UrlPreferenceExtensionsTests
  {
    private Browser edge;

    [SetUp]
    public void Setup()
    {
      edge = new Browser
      {
        Name = "edge",
        Location = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
      };
    }

    [Test]
    public void GetDomainAndPatternRegular()
    {
      var preferences = new List<UrlPreference>();

      {
        var domainPref = new UrlPreference
        {
          UrlPattern = "*.microsoft.com",
          Browser = edge
        };

        var (domain, pattern) =
            domainPref.GetDomainAndPattern(new Uri("https://docs.microsoft.com/en-us/"));

        Assert.That(domain, Is.EqualTo("docs.microsoft.com"));
        Assert.That(pattern, Is.EqualTo("^.*\\.microsoft\\.com$"));
        preferences.Add(domainPref);
      }

      {
        var domainPref = new UrlPreference
        {
          UrlPattern = "microsoft.com",
          Browser = edge
        };

        var (domain, pattern) =
            domainPref.GetDomainAndPattern(new Uri("https://www.microsoft.com/de-de/search/explore?q=qwerty"));

        Assert.That(domain, Is.EqualTo("www.microsoft.com"));
        Assert.That(pattern, Is.EqualTo("^microsoft\\.com$"));
        preferences.Add(domainPref);
      }

      {
        var domainPref = new UrlPreference
        {
          UrlPattern = "*visualstudio.com",
          Browser = edge
        };

        var (domain, pattern) =
            domainPref.GetDomainAndPattern(new Uri("https://my.visualstudio.com/?auth_redirect=true"));

        Assert.That(domain, Is.EqualTo("my.visualstudio.com"));
        Assert.That(pattern, Is.EqualTo("^.*visualstudio\\.com$"));
        preferences.Add(domainPref);
      }

      Assert.That(preferences.TryGetPreference(new Uri("https://docs.microsoft.com/en-us/"), out var pref1), Is.True);
      Assert.That(preferences.TryGetPreference(new Uri("https://www.microsoft.com/de-de/search/explore?q=qwerty"), out var pref2), Is.True);
      Assert.That(preferences.TryGetPreference(new Uri("https://my.visualstudio.com/?auth_redirect=true"), out var pref3), Is.True);
    }

    [Test]
    public void GetDomainAndPatternRegex()
    {
      var preferences = new List<UrlPreference>();

      {
        var domainPref = new UrlPreference
        {
          UrlPattern = "/(?:[0-9]{1,3}\\.){3}[0-9]{1,3}/",
          Browser = edge
        };

        var (domain, pattern) =
            domainPref.GetDomainAndPattern(new Uri("https://192.168.69.420/proxy"));

        Assert.That(domain, Is.EqualTo("192.168.69.420/proxy"));
        Assert.That(pattern, Is.EqualTo("(?:[0-9]{1,3}\\.){3}[0-9]{1,3}"));
        preferences.Add(domainPref);
      }

      Assert.That(preferences.TryGetPreference(new Uri("https://192.168.69.420/proxy?q=base"), out var pref1), Is.True);
    }

    [Test]
    public void GetDomainAndPatternQuery()
    {
      var preferences = new List<UrlPreference>();

      {
        var domainPref = new UrlPreference
        {
          UrlPattern = "?www.youtube.com/redirect?*reddit.com*?",
          Browser = edge
        };

        var (domain, pattern) =
            domainPref.GetDomainAndPattern(new Uri("https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine"));

        Assert.That(domain, Is.EqualTo("www.youtube.com/redirect?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine"));
        Assert.That(pattern, Is.EqualTo("^www\\.youtube\\.com/redirect\\?.*reddit\\.com.*$"));
        preferences.Add(domainPref);
      }

      Assert.That(preferences.TryGetPreference(new Uri("https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLU&q=https%3A%2F%2Fwww.reddit.com%2Fdata%20mine"), out var pref1), Is.True);


    }
  }
}