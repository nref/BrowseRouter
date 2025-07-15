using System.Net;

namespace BrowseRouter.Model;

public static class UriFactory
{
  public static Uri? Get(string url) => url switch
  {
    _ when Uri.TryCreate(url, UriKind.Absolute, out var uri) => uri,
    _ when Uri.TryCreate($"https://{url}", UriKind.Absolute, out var uri) => uri,
    _ when Uri.TryCreate(WebUtility.UrlDecode(url), UriKind.Absolute, out var uri) => uri,
    _ => null,
  };
}