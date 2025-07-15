using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model.ArgsTests;

public class FormatMethod
{
  [Fact]
  public void DoesNotDecodeQueryString()
  {
    string result = Args.Format("", new Uri("https://www.google.com/search?q=C%23+Rocks"));

    result.Should().NotContain("C# Rocks");
  }
}
