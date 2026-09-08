using System.Text.RegularExpressions;
using BrowseRouter.Model;

namespace BrowseRouter.Config;

public abstract record Match
{
  private Match() { }

  public abstract bool Matches(LaunchRequest request);

  public enum UrlSyntax { DomainWildcard, DomainAndPathRegex, DomainAndQueryWildcard }

  public enum SourceSyntax { Wildcard, Regex }

  public record Url(string Pattern, UrlSyntax Syntax) : Match
  {
    public override bool Matches(LaunchRequest request)
      => Regex.IsMatch(Input(request.Url), RegexPattern());

    public override string ToString() => Syntax switch
    {
      UrlSyntax.DomainAndPathRegex => $"url /{Pattern}/",
      UrlSyntax.DomainAndQueryWildcard => $"url ?{Pattern}?",
      _ => $"url \"{Pattern}\""
    };

    private string Input(Uri url) => Syntax switch
    {
      UrlSyntax.DomainAndPathRegex => url.Authority + url.AbsolutePath,
      UrlSyntax.DomainAndQueryWildcard => url.Authority + url.PathAndQuery,
      _ => url.Authority
    };

    private string RegexPattern() => Syntax switch
    {
      UrlSyntax.DomainAndPathRegex => Pattern,
      _ => PatternSyntax.ToRegexPattern(Pattern)
    };
  }

  public record Source(string Pattern, SourceSyntax Syntax) : Match
  {
    public override bool Matches(LaunchRequest request)
      => Regex.IsMatch(request.WindowTitle, RegexPattern());

    public override string ToString() => Syntax switch
    {
      SourceSyntax.Regex => $"source /{Pattern}/",
      _ => $"source \"{Pattern}\""
    };

    private string RegexPattern() => Syntax switch
    {
      SourceSyntax.Regex => Pattern,
      _ => PatternSyntax.ToRegexPattern(Pattern)
    };
  }
}
