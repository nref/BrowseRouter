using System.Text.RegularExpressions;

namespace BrowseRouter;

public static class UrlPreferenceExtensions
{
  public static bool TryGetPreference(this IEnumerable<UrlPreference> prefs, Uri uri, out UrlPreference pref)
  {
    pref = prefs.FirstOrDefault(pref =>
    {
      (string domain, string pattern) = pref.GetDomainAndPattern(uri);
      return Regex.IsMatch(domain, pattern);
    })!;

    return pref != null;
  }

  public static (string, string) GetDomainAndPattern(this UrlPreference pref, Uri uri)
  {
    string urlPattern = pref.UrlPattern;

    if (urlPattern.StartsWith("/") && urlPattern.EndsWith("/"))
    {
      // The domain from the INI file is a regex
      string domain = uri.Authority + uri.AbsolutePath;
      string pattern = urlPattern.Substring(1, urlPattern.Length - 2);

      return (domain, pattern);
    }

    {
      // We're only checking the domain.
      string domain = uri.Authority;

      // Escape the input for regex; the only special character we support is a *
      var regex = Regex.Escape(urlPattern);

      // Unescape * as a wildcard.
      string pattern = $"^{regex.Replace("\\*", ".*")}$";

      return (domain, pattern);
    }
  }
}