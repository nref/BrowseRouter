using System.Text.RegularExpressions;
using BrowseRouter.Config;

namespace BrowseRouter.Tests.Config.PatternSyntaxTests;

public class ToRegexPatternMethod
{
  [Theory]
  [InlineData("*.youtube.com", @"^.*\.youtube\.com$")]
  [InlineData("youtube.com", @"^youtube\.com$")]
  [InlineData("*visualstudio.com", @"^.*visualstudio\.com$")]
  [InlineData("*", "^.*$")]
  public void Wildcard_BecomesAnchoredRegexPattern(string wildcard, string expected)
    => PatternSyntax.ToRegexPattern(wildcard).Should().Be(expected);

  [Fact]
  public void Dot_IsEscapedRatherThanMatchingAnyCharacter()
    => Regex.IsMatch("microsoftxcom", PatternSyntax.ToRegexPattern("microsoft.com"))
      .Should().BeFalse();

  [Fact]
  public void QuestionMark_IsEscapedRatherThanMakingTheAtomOptional()
    => Regex.IsMatch("ab", PatternSyntax.ToRegexPattern("ab?"))
      .Should().BeFalse();
}
