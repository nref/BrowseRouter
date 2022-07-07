using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BrowserSelector;

public class BrowserService
{
  public void Launch(string url)
  {
    try
    {
      var urlPreferences = new ConfigService().GetUrlPreferences();
      string _url = url;
      Uri uri;

      try
      {
        uri = new Uri(_url);
      }
      catch (UriFormatException)
      {
        // Try to prepend https when given an incomplete URI e.g. "google.com"
        _url = $"https://{_url}";
        uri = new Uri(_url);
      }

      foreach (var preference in urlPreferences)
      {
        string domain = "";
        string pattern;
        string urlPattern = preference.UrlPattern;

        if (urlPattern.StartsWith("/") && urlPattern.EndsWith("/"))
        {
          // The domain from the INI file is a regex..
          domain = uri.Authority + uri.AbsolutePath;
          pattern = urlPattern.Substring(1, urlPattern.Length - 2);
        }
        else
        {
          // We're only checking the domain.
          domain = uri.Authority;

          // Escape the input for regex; the only special character we support is a *
          var regex = Regex.Escape(urlPattern);
          // Unescape * as a wildcard.
          pattern = $"^{regex.Replace("\\*", ".*")}$";
        }

        if (Regex.IsMatch(domain, pattern))
        {
          Process p;
          string loc = preference.Browser.Location;

          if (loc.IndexOf("{url}") > -1)
          {
            loc = loc.Replace("{url}", _url);
            _url = "";
          }
          if (loc.StartsWith("\"") && loc.IndexOf('"', 2) > -1)
          {
            // Assume the quoted item is the executable, while everything
            // after (the second quote), is part of the command-line arguments.
            loc = loc.Substring(1);
            int pos = loc.IndexOf('"');
            string args = loc.Substring(pos + 1).Trim();
            loc = loc.Substring(0, pos).Trim();
            p = Process.Start(loc, args + " " + _url);
          }
          else
          {
            // The browser specified in the INI file is a single executable
            // without any other arguments.
            // (normal/original behavior)
            p = Process.Start(loc, _url);
          }

          return;
        }
      }

      Log.Write($"Unable to find a suitable browser matching {url}.");
    }
    catch (Exception e)
    {
      Log.Write($"{e}");
    }
  }
}

