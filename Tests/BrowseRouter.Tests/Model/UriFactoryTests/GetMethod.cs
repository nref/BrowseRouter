using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model.UriFactoryTests;

public class GetMethod
{
  [Fact]
  public void ValidUrl_ReturnsThatUri()
  {
    var uri = UriFactory.Get("https://www.example.com/path");

    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void UrlWithoutAScheme_GetsHttps()
  {
    var uri = UriFactory.Get("www.example.com/path");

    uri?.Scheme.Should().Be("https");
    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void EncodedUrl_IsDecoded()
  {
    var uri = UriFactory.Get("https%3A%2F%2Fwww.example.com%2Fpath");

    uri?.AbsoluteUri.Should().Be("https://www.example.com/path");
  }

  [Fact]
  public void EncodedQueryParameters_StayEncoded()
  {
    var uri = UriFactory.Get("https://www.example.com/path?token=6Us%2btD%2btWmVr");

    uri?.Query.Should().Be("?token=6Us%2btD%2btWmVr");
  }

  [Fact]
  public void InvalidUrl_ReturnsNull()
  {
    var uri = UriFactory.Get("");

    uri.Should().BeNull();
  }
}
