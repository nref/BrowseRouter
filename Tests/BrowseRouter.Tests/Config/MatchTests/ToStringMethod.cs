using BrowseRouter.Config;
using BrowseRouter.Model;

namespace BrowseRouter.Tests.Config.MatchTests;

public class ToStringMethod
{
  [Fact]
  public void DomainWildcard_ShowsThePatternAsWritten()
    => new Match.Url("*.youtube.com", Match.UrlSyntax.DomainWildcard)
      .ToString().Should().Be("url \"*.youtube.com\"");

  [Fact]
  public void DomainAndPathRegex_RestoresTheSlashDelimiters()
    => new Match.Url("^a$", Match.UrlSyntax.DomainAndPathRegex)
      .ToString().Should().Be("url /^a$/");

  [Fact]
  public void DomainAndQueryWildcard_RestoresTheQuestionMarkDelimiters()
    => new Match.Url("a?b", Match.UrlSyntax.DomainAndQueryWildcard)
      .ToString().Should().Be("url ?a?b?");

  [Fact]
  public void SourceWildcard_ShowsThePatternAsWritten()
    => new Match.Source("*Outlook*", Match.SourceSyntax.Wildcard)
      .ToString().Should().Be("source \"*Outlook*\"");

  [Fact]
  public void SourceRegex_RestoresTheSlashDelimiters()
    => new Match.Source("^a$", Match.SourceSyntax.Regex)
      .ToString().Should().Be("source /^a$/");

  [Fact]
  public void Rule_ShowsItsConditionAndBrowser()
    => new Rule(new Match.Url("*", Match.UrlSyntax.DomainWildcard), new Browser("chrome", "c.exe"))
      .ToString().Should().Be("url \"*\" => \"chrome\" (\"c.exe\")");
}
