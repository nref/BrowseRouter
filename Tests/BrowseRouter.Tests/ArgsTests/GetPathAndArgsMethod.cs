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
  public void SplitsQuotedPathWithSpaces_AndArgs()
  {
    // Arrange
    string input = @"""C:\Program Files\Mozilla Firefox\firefox.exe"" ext+container:name=Work&url={url}";

    // Act
    var result = Args.SplitPathAndArgs(input);

    // Assert
    result.Should().Be((@"C:\Program Files\Mozilla Firefox\firefox.exe", "ext+container:name=Work&url={url}"));
  }
}