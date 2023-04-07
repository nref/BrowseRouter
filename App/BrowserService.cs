using System.Diagnostics;

namespace BrowseRouter;

public class BrowserService
{
  private readonly IConfigService _config;

  public BrowserService(IConfigService config)
  {
    _config = config;
  }

  public void Launch(string url, string windowTitle)
  {
    try
    {
      Log.Write($"Attempting to launch \"{url}\" for \"{windowTitle}\"");

      IEnumerable<UrlPreference> urlPreferences = _config.GetUrlPreferences("urls");
      IEnumerable<UrlPreference> sourcePreferences = _config.GetUrlPreferences("sources");
      Uri uri = UriFactory.Get(url);

      UrlPreference? pref = null;
      if (sourcePreferences.TryGetPreference(windowTitle, out UrlPreference sourcePref))
      {
        Log.Write($"Found source preference {sourcePref}");
        pref = sourcePref;
      }
      
      else if (urlPreferences.TryGetPreference(uri, out UrlPreference urlPref))
      {
        Log.Write($"Found URL preference {urlPref}");
        pref = urlPref;
      }

      if (pref == null)
      {
        Log.Write($"Unable to find a browser matching \"{url}\".");
        return;
      }

      (string path, string args) = Executable.GetPathAndArgs(pref.Browser.Location);

      Log.Write($"Launching {path} with args \"{args} {uri.OriginalString}\"");
      Process.Start(path, $"{args} {uri.OriginalString}");
    }
    catch (Exception e)
    {
      Log.Write($"{e}");
    }
  }
}
