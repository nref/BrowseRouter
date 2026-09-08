using BrowseRouter.Config;
using BrowseRouter.Config.Loaders;
using BrowseRouter.Model;

namespace BrowseRouter.Tests.Config.Loaders.ConfigTranslatorTests;

public class ToConfigMethod
{
  private static readonly Dictionary<string, string> Browsers = new()
  {
    ["chrome"] = @"C:\chrome.exe",
    ["ff"] = @"C:\firefox.exe",
  };

  [Fact]
  public void LegacySectionsOnly_PutsSourcesBeforeUrls()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Sources = [new PatternBrowser("*Outlook*", "ff")],
      Urls = [new PatternBrowser("*.youtube.com", "chrome")],
    });

    config.Rules.Select(r => r.Condition.ToString())
      .Should().Equal("source \"*Outlook*\"", "url \"*.youtube.com\"", "url \"*\"");
  }

  [Fact]
  public void LegacyUrls_KeepTheirListOrder()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Urls =
      [
        new PatternBrowser("z.test", "chrome"),
        new PatternBrowser("a.test", "ff"),
        new PatternBrowser("m.test", "chrome"),
      ],
    });

    config.Rules.Take(3).Select(r => r.Condition.ToString())
      .Should().Equal("url \"z.test\"", "url \"a.test\"", "url \"m.test\"");
  }

  [Fact]
  public void RulesPresent_AreUsedInOrder()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules =
      [
        new RuleConfig("*.youtube.com", null, "chrome"),
        new RuleConfig(null, "*Discord*", "ff"),
      ],
    });

    config.Rules.Select(r => r.Condition.ToString())
      .Should().Equal("url \"*.youtube.com\"", "source \"*Discord*\"", "url \"*\"");
    warnings.Should().BeEmpty();
  }

  [Fact]
  public void RulesAndLegacySectionsBothPresent_UsesRulesAndWarns()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig("*.youtube.com", null, "chrome")],
      Sources = [new PatternBrowser("*Outlook*", "ff")],
    });

    config.Rules.Should().NotContain(r => r.Condition is Match.Source);
    warnings.Should().ContainSingle(w => w.Contains("rules"));
  }

  [Fact]
  public void EmptyRulesArrayWithLegacySections_StillUsesRules()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Rules = [],
      Urls = [new PatternBrowser("*.youtube.com", "chrome")],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
  }

  [Fact]
  public void UnknownBrowser_DropsTheRuleAndWarns()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig("*.youtube.com", null, "opera")],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
    warnings.Should().ContainSingle(w => w.Contains("opera"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void MissingOrBlankBrowser_DropsTheRuleAndWarnsWithoutThrowing(string? browser)
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig("*.youtube.com", null, browser)],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void NoBrowsers_YieldsNoRulesAndWarns()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Browsers = null,
      Urls = [new PatternBrowser("*.youtube.com", "chrome")],
    });

    config.Rules.Should().BeEmpty();
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void EmptyBrowsers_YieldsNoRulesAndWarns()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Browsers = [],
    });

    config.Rules.Should().BeEmpty();
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void AbsentNotifyAndLog_AreSubstitutedWithoutThrowing()
  {
    (BrowseRouter.Config.Config config, _) = Translate(ConfigFile.Empty with { Browsers = Browsers });

    config.Notify.Enabled.Should().Be(NotifyPreference.IsEnabledDefaultValue);
    config.Log.Should().Be(LogConfig.Empty);
  }

  [Fact]
  public void NoCatchAll_AppendsOneUsingTheFirstBrowser()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Urls = [new PatternBrowser("*.youtube.com", "ff")],
    });

    config.Rules.Last().Should()
      .Be(new Rule(new Match.Url("*", Match.UrlSyntax.DomainWildcard), new Browser("chrome", @"C:\chrome.exe")));
  }

  [Fact]
  public void CatchAllAlreadyPresent_IsNotDuplicated()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Urls = [new PatternBrowser("*", "ff")],
    });

    config.Rules.Should().ContainSingle()
      .Which.Browser.Name.Should().Be("ff");
  }

  [Fact]
  public void RuleWithBothUrlAndSource_IsDroppedAndWarned()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig("*.youtube.com", "*Discord*", "chrome")],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void RuleWithNeitherUrlNorSource_IsDroppedAndWarned()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig(null, null, "chrome")],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void MalformedRegex_DropsOnlyThatRuleAndWarns()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules =
      [
        new RuleConfig("/([unclosed/", null, "chrome"),
        new RuleConfig("*.youtube.com", null, "ff"),
      ],
    });

    config.Rules.Select(r => r.Condition.ToString())
      .Should().Equal("url \"*.youtube.com\"", "url \"*\"");
    warnings.Should().ContainSingle();
  }

  [Fact]
  public void LoneSlashPattern_IsAWildcardNotADelimitedRegex()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig("/", null, "chrome")],
    });

    config.Rules.First().Condition.Should().Be(new Match.Url("/", Match.UrlSyntax.DomainWildcard));
    warnings.Should().BeEmpty();
  }

  [Fact]
  public void DelimitedRegexUrl_StripsItsDelimiters()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Rules = [new RuleConfig(@"/sites\.google\.com/a/x/", null, "chrome")],
    });

    config.Rules.First().Condition.Should()
      .Be(new Match.Url(@"sites\.google\.com/a/x", Match.UrlSyntax.DomainAndPathRegex));
  }

  [Fact]
  public void QueryUrl_StripsItsDelimiters()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with
    {
      Rules = [new RuleConfig("?a?b?", null, "chrome")],
    });

    config.Rules.First().Condition.Should()
      .Be(new Match.Url("a?b", Match.UrlSyntax.DomainAndQueryWildcard));
  }

  [Fact]
  public void QuerySourcePattern_IsDroppedAndWarned()
  {
    (BrowseRouter.Config.Config config, IReadOnlyList<string> warnings) = Translate(File() with
    {
      Rules = [new RuleConfig(null, "?a?b?", "chrome")],
    });

    config.Rules.Should().ContainSingle().Which.Condition.ToString().Should().Be("url \"*\"");
    warnings.Should().ContainSingle(w => w.Contains("query"));
  }

  [Fact]
  public void FiltersFile_IsCarriedThrough()
  {
    (BrowseRouter.Config.Config config, _) = Translate(File() with { FiltersFile = "filters.json" });

    config.FiltersFile.Should().Be("filters.json");
  }

  private static ConfigFile File() => ConfigFile.Empty with { Browsers = Browsers };

  private static (BrowseRouter.Config.Config, IReadOnlyList<string>) Translate(ConfigFile file)
    => ConfigTranslator.ToConfig(file);
}
