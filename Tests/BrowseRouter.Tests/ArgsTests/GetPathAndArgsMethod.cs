using BrowseRouter.Model;

namespace BrowserRouter.Tests.ArgsTests;

public class GetPathAndArgsMethod
{
  [Fact]
  public void ReturnsQuotedPathAndArgs_WhenInputContainsQuotes()
  {
    // Arrange
    string input = "\"chrome.exe\" --new-window";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be(("chrome.exe", "--new-window"));
  }

  [Fact]
  public void ReturnsPathOnly_WhenInputDoesNotContainQuotesOrArgs()
  {
    // Arrange
    string input = "chrome.exe";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be(("chrome.exe", ""));
  }

  [Fact]
  public void ReturnsEmptyArgs_WhenInputHasOnlyQuotedPath()
  {
    // Arrange
    string input = "\"chrome.exe\"";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be(("chrome.exe", ""));
  }

  [Fact]
  public void ReturnsInputAsPath_WhenInputContainsInvalidQuotes()
  {
    // Arrange
    string input = "\"chrome.exe";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be((input, ""));
  }

  [Fact]
  public void SplitsUnquotedPathWithArgs_OnFirstSpace()
  {
    // Arrange - Issue #94: User wants to pass args with {url} tag to browser
    // e.g., "firefox.exe ext+container:name=Work&url={url}"
    string input = "firefox.exe ext+container:name=Work&url={url}";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert - Should split on first space, treating rest as args
    result.Should().Be(("firefox.exe", "ext+container:name=Work&url={url}"));
  }

  [Fact]
  public void SplitsUnquotedPathWithArgs_PreservesMultipleArgs()
  {
    // Arrange
    string input = "firefox.exe --new-window --url {url}";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be(("firefox.exe", "--new-window --url {url}"));
  }
}