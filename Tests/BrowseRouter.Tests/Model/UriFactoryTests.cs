using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model;

public class GetMethod
{
  [Fact]
  public void HandlesValidUrl()
  {
    string url = "https://www.example.com/path";
    var uri = UriFactory.Get(url);

    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void PrependsHttps_ForUrlWithoutScheme()
  {
    string url = "www.example.com/path";
    var uri = UriFactory.Get(url);

    uri?.Scheme.Should().Be("https");
    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void DecodesUrlEncodedUrl()
  {
    string url = "https%3A%2F%2Fwww.example.com%2Fpath";
    var uri = UriFactory.Get(url);

    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void PreservesQueryParameters()
  {
    string url = "https://www.example.com/path?token=6Us%2btD%2btWmVr";
    var uri = UriFactory.Get(url);

    uri?.Query.Should().Be("?token=6Us%2btD%2btWmVr");
  }

  [Fact]
  public void ReturnsNull_ForInvalidUrl()
  {
    string url = "";
    var uri = UriFactory.Get(url);

    uri.Should().BeNull();
  }
}
