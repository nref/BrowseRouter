using BrowseRouter.Config;

namespace BrowseRouter.Tests.Config.PatternSyntaxTests;

public class TryGetDelimitedMethod
{
  [Fact]
  public void DelimitedText_ReturnsTrueAndTheInnerText()
  {
    PatternSyntax.TryGetDelimited("/^a$/", '/', out string inner).Should().BeTrue();

    inner.Should().Be("^a$");
  }

  [Fact]
  public void DelimiterPair_ReturnsTrueAndEmptyInnerText()
  {
    PatternSyntax.TryGetDelimited("//", '/', out string inner).Should().BeTrue();

    inner.Should().BeEmpty();
  }

  [Theory]
  [InlineData("/", '/')]
  [InlineData("?", '?')]
  [InlineData("", '/')]
  [InlineData("a", '/')]
  [InlineData("/a", '/')]
  [InlineData("a/", '/')]
  [InlineData("/a/", '?')]
  public void NotDelimited_ReturnsFalseAndTheTextUnchanged(string text, char delimiter)
  {
    PatternSyntax.TryGetDelimited(text, delimiter, out string inner).Should().BeFalse();

    inner.Should().Be(text);
  }
}
