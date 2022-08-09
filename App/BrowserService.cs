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
        IEnumerable<UrlPreference> urlPreferences = _config.GetUrlPreferences("urls");
        IEnumerable<UrlPreference> sourcePreferences = _config.GetUrlPreferences("sources");
      Uri uri = UriFactory.Get(url);

      UrlPreference pref = null;
      if (urlPreferences.TryGetPreference(uri, out UrlPreference urlPref))
      {
          pref = urlPref;
      }

      if (sourcePreferences.TryGetPreference(windowTitle, out UrlPreference sourcePref))
      {
          pref = sourcePref;
      }

      if (pref == null)
      {
          Log.Write($"Unable to find a browser matching {url}.");
          return;
      }

      (string path, string args) = Executable.GetPathAndArgs(pref.Browser.Location);

      Process.Start(path, $"{args} {uri}");
    }
    catch (Exception e)
    {
      Log.Write($"{e}");
    }
  }
}