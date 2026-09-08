using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model.BrowserTests;

public class EqualsMethod
{
  [Fact]
  public void SameNameAndLocation_AreEqual()
    => new Browser("chrome", @"C:\chrome.exe")
      .Should().Be(new Browser("chrome", @"C:\chrome.exe"));

  [Fact]
  public void DifferentLocation_AreNotEqual()
    => new Browser("chrome", @"C:\chrome.exe")
      .Should().NotBe(new Browser("chrome", @"D:\chrome.exe"));
}
