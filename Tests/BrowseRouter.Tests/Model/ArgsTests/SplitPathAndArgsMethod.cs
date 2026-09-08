using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model.ArgsTests;

public class SplitPathAndArgsMethod
{
  [Fact]
  public void QuotedPathWithArgs_SplitsIntoPathAndArgs()
  {
    var result = Args.SplitPathAndArgs("\"chrome.exe\" --new-window");

    result.Should().Be(("chrome.exe", "--new-window"));
  }

  [Fact]
  public void UnquotedPath_ReturnsThePathAndNoArgs()
  {
    var result = Args.SplitPathAndArgs("chrome.exe");

    result.Should().Be(("chrome.exe", ""));
  }

  [Fact]
  public void QuotedPathAlone_ReturnsThePathAndNoArgs()
  {
    var result = Args.SplitPathAndArgs("\"chrome.exe\"");

    result.Should().Be(("chrome.exe", ""));
  }

  [Fact]
  public void UnbalancedQuote_ReturnsTheWholeInputAsThePath()
  {
    string input = "\"chrome.exe";

    var result = Args.SplitPathAndArgs(input);

    result.Should().Be((input, ""));
  }

  [Fact]
  public void QuotedPathWithSpaces_SplitsAtTheClosingQuote()
  {
    var result = Args.SplitPathAndArgs(
      @"""C:\Program Files\Mozilla Firefox\firefox.exe"" ext+container:name=Work&url={url}");

    result.Should().Be((@"C:\Program Files\Mozilla Firefox\firefox.exe", "ext+container:name=Work&url={url}"));
  }
}
