using System.Text.RegularExpressions;
using BrowseRouter.Model;

namespace BrowseRouter.Config.Loaders;

internal class ConfigTranslator
{
  private readonly List<string> _warnings = [];

  private ConfigTranslator() { }

  public static (Config Config, IReadOnlyList<string> Warnings) ToConfig(ConfigFile file)
    => new ConfigTranslator().Translate(file);

  private (Config, IReadOnlyList<string>) Translate(ConfigFile file)
  {
    Config config = new(
      file.Notify ?? new NotifyConfig(),
      file.Log ?? LogConfig.Empty,
      ToRules(file),
      file.FiltersFile);

    return (config, _warnings);
  }

  private IReadOnlyList<Rule> ToRules(ConfigFile file)
  {
    if (file.Browsers is null || file.Browsers.Count == 0)
    {
      _warnings.Add("No browsers are configured, so no URL can be routed. Add a \"browsers\" section.");
      return [];
    }

    List<Rule> rules = file.Rules is null
      ? ToLegacyRules(file, file.Browsers)
      : ToDeclaredRules(file, file.Browsers);

    Match catchAll = new Match.Url("*", Match.UrlSyntax.DomainWildcard);

    if (!rules.Any(r => r.Condition == catchAll))
    {
      KeyValuePair<string, string> first = file.Browsers.First();
      rules.Add(new Rule(catchAll, new Browser(first.Key, first.Value)));
    }

    return rules;
  }

  private List<Rule> ToDeclaredRules(ConfigFile file, Dictionary<string, string> browsers)
  {
    if (file.Sources?.Count > 0 || file.Urls?.Count > 0)
      _warnings.Add("Both \"rules\" and the older \"urls\" or \"sources\" sections are present. Only \"rules\" is used.");

    return file.Rules!
      .Select(rule => ToRule(ToCondition(rule), rule.Browser, browsers))
      .OfType<Rule>()
      .ToList();
  }

  private List<Rule> ToLegacyRules(ConfigFile file, Dictionary<string, string> browsers)
  {
    IEnumerable<Rule?> sources = (file.Sources ?? [])
      .Select(entry => ToRule(ToSourceMatch(entry.Pattern), entry.Browser, browsers));

    IEnumerable<Rule?> urls = (file.Urls ?? [])
      .Select(entry => ToRule(ToUrlMatch(entry.Pattern), entry.Browser, browsers));

    return sources.Concat(urls).OfType<Rule>().ToList();
  }

  private Rule? ToRule(Match? condition, string? browserName, Dictionary<string, string> browsers)
  {
    if (condition is null)
      return null;

    if (string.IsNullOrWhiteSpace(browserName))
    {
      _warnings.Add($"Rule {condition} names no browser.");
      return null;
    }

    if (!browsers.TryGetValue(browserName, out string? location))
    {
      _warnings.Add($"Rule {condition} names browser \"{browserName}\", which is not in \"browsers\".");
      return null;
    }

    return new Rule(condition, new Browser(browserName, location));
  }

  private Match? ToCondition(RuleConfig rule)
  {
    if (rule.Url is not null && rule.Source is not null)
    {
      _warnings.Add($"Rule \"{rule.Url}\" sets both \"url\" and \"source\". Set exactly one.");
      return null;
    }

    if (rule.Url is not null)
      return ToUrlMatch(rule.Url);

    if (rule.Source is not null)
      return ToSourceMatch(rule.Source);

    _warnings.Add("A rule sets neither \"url\" nor \"source\". Set exactly one.");
    return null;
  }

  private Match? ToUrlMatch(string pattern)
  {
    if (PatternSyntax.TryGetDelimited(pattern, '/', out string regex))
    {
      string? error = RegexError(regex);

      if (error is null)
        return new Match.Url(regex, Match.UrlSyntax.DomainAndPathRegex);

      _warnings.Add($"URL pattern \"{pattern}\" is not a valid regular expression: {error}");
      return null;
    }

    if (PatternSyntax.TryGetDelimited(pattern, '?', out string query))
      return new Match.Url(query, Match.UrlSyntax.DomainAndQueryWildcard);

    return new Match.Url(pattern, Match.UrlSyntax.DomainWildcard);
  }

  private Match? ToSourceMatch(string pattern)
  {
    if (PatternSyntax.TryGetDelimited(pattern, '/', out string regex))
    {
      string? error = RegexError(regex);

      if (error is null)
        return new Match.Source(regex, Match.SourceSyntax.Regex);

      _warnings.Add($"Source pattern \"{pattern}\" is not a valid regular expression: {error}");
      return null;
    }

    if (PatternSyntax.TryGetDelimited(pattern, '?', out _))
    {
      _warnings.Add($"Source pattern \"{pattern}\" uses the query syntax, which applies only to URLs.");
      return null;
    }

    return new Match.Source(pattern, Match.SourceSyntax.Wildcard);
  }

  private static string? RegexError(string regex)
  {
    try
    {
      _ = Regex.IsMatch(string.Empty, regex);
      return null;
    }
    catch (RegexParseException e)
    {
      return e.Message;
    }
  }
}
