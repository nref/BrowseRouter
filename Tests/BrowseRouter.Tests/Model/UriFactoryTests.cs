using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model;

public class GetMethod
{
  [Fact]
  public void HandlesValidUrl()
  {
    string url = "https://www.example.com/path";
    var act = () => UriFactory.Get(url);

    act.Should().NotThrow<UriFormatException>();
  }

  [Fact]
  public void HandlesUrlWithoutHttps()
  {
    string url = "www.example.com/path";
    var act = () => UriFactory.Get(url);

    act.Should().NotThrow<UriFormatException>();
  }

  [Fact]
  public void HandlesUrlEncodedUrl()
  {
    string url = "https%3A%2F%2Fwww.example.com%2Fpath%2F";
    var act = () => UriFactory.Get(url);

    act.Should().NotThrow<UriFormatException>();
  }
}
