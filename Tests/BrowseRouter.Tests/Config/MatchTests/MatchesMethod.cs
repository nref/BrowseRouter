using BrowseRouter.Config;
using BrowseRouter.Model;

namespace BrowseRouter.Tests.Config.MatchTests;

public class MatchesMethod
{
  [Theory]
  [InlineData("*.microsoft.com", "https://docs.microsoft.com/en-us/")]
  [InlineData("microsoft.com", "https://microsoft.com/de-de/search?q=qwerty")]
  [InlineData("*visualstudio.com", "https://my.visualstudio.com/?auth_redirect=true")]
  [InlineData("*", "https://anything.test/a/b?c=d")]
  public void DomainWildcard_MatchesTheAuthorityOnly(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainWildcard)
      .Matches(Request(url)).Should().BeTrue();

  [Theory]
  [InlineData("*.microsoft.com", "https://microsoft.com/en-us/")]
  [InlineData("microsoft.com", "https://www.microsoft.com/de-de/")]
  [InlineData("microsoft.com", "https://microsoftxcom/")]
  [InlineData("*visualstudio.com", "https://visualstudio.net/")]
  public void DomainWildcard_DoesNotMatchADifferentAuthority(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainWildcard)
      .Matches(Request(url)).Should().BeFalse();

  [Theory]
  [InlineData(@"(?:[0-9]{1,3}\.){3}[0-9]{1,3}", "https://192.168.69.420/proxy?q=base")]
  [InlineData(@"sites\.google\.com/a/myproject", "https://sites.google.com/a/myproject/home")]
  public void DomainAndPathRegex_MatchesTheAuthorityAndPath(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainAndPathRegex)
      .Matches(Request(url)).Should().BeTrue();

  [Theory]
  [InlineData(@"sites\.google\.com/a/myproject", "https://sites.google.com/a/other/home")]
  public void DomainAndPathRegex_DoesNotMatchADifferentPath(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainAndPathRegex)
      .Matches(Request(url)).Should().BeFalse();

  [Theory]
  [InlineData("www.youtube.com/redirect?*reddit.com*",
    "https://www.youtube.com/redirect?event=video_description&q=https%3A%2F%2Fwww.reddit.com%2Fdata")]
  public void DomainAndQueryWildcard_MatchesTheAuthorityPathAndQuery(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainAndQueryWildcard)
      .Matches(Request(url)).Should().BeTrue();

  [Theory]
  [InlineData("www.youtube.com/redirect?*reddit.com*",
    "https://www.youtube.com/redirect?event=video_description&q=https%3A%2F%2Fexample.com")]
  public void DomainAndQueryWildcard_DoesNotMatchADifferentQuery(string pattern, string url)
    => new Match.Url(pattern, Match.UrlSyntax.DomainAndQueryWildcard)
      .Matches(Request(url)).Should().BeFalse();

  [Theory]
  [InlineData("*Discord -> Discord", "Jerrk's Server - Discord -> Discord")]
  [InlineData(" -> AutoHotkey64", " -> AutoHotkey64")]
  [InlineData("*Outlook*", "Inbox - Outlook - Work")]
  public void SourceWildcard_MatchesTheWindowTitle(string pattern, string windowTitle)
    => new Match.Source(pattern, Match.SourceSyntax.Wildcard)
      .Matches(Request("https://example.test/", windowTitle)).Should().BeTrue();

  [Theory]
  [InlineData("*Discord -> Discord", "Inbox - Outlook")]
  public void SourceWildcard_DoesNotMatchADifferentWindowTitle(string pattern, string windowTitle)
    => new Match.Source(pattern, Match.SourceSyntax.Wildcard)
      .Matches(Request("https://example.test/", windowTitle)).Should().BeFalse();

  [Theory]
  [InlineData("^Slack \\| .*", "Slack | Test Channel")]
  public void SourceRegex_MatchesTheWindowTitle(string pattern, string windowTitle)
    => new Match.Source(pattern, Match.SourceSyntax.Regex)
      .Matches(Request("https://example.test/", windowTitle)).Should().BeTrue();

  [Theory]
  [InlineData("^Slack \\| .*", "Not Slack | Test Channel")]
  public void SourceRegex_DoesNotMatchADifferentWindowTitle(string pattern, string windowTitle)
    => new Match.Source(pattern, Match.SourceSyntax.Regex)
      .Matches(Request("https://example.test/", windowTitle)).Should().BeFalse();

  [Fact]
  public void UrlCase_IgnoresTheWindowTitle()
    => new Match.Url("example.test", Match.UrlSyntax.DomainWildcard)
      .Matches(Request("https://example.test/", "anything at all")).Should().BeTrue();

  [Fact]
  public void SourceCase_IgnoresTheUrl()
    => new Match.Source("*Outlook*", Match.SourceSyntax.Wildcard)
      .Matches(Request("https://example.test/", "Inbox - Outlook")).Should().BeTrue();

  private static LaunchRequest Request(string url, string windowTitle = "Fake Window")
    => new(new Uri(url), windowTitle);
}
