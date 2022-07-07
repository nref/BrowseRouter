using System.Diagnostics;

namespace BrowseRouter;

public class BrowserService
{
  private readonly IConfigService _config;

  public BrowserService(IConfigService config)
  {
    _config = config;
  }

  public void Launch(string url)
  {
    try
    {
      var prefs = _config.GetUrlPreferences();
      Uri uri = UriFactory.Get(url);

      if (!prefs.TryGetPreference(uri, out UrlPreference pref))
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