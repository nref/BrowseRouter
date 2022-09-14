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
      IEnumerable<UrlPreference> prefs = _config.GetUrlPreferences();
      Uri uri = UriFactory.Get(url);

      if (!prefs.TryGetPreference(uri, out UrlPreference pref))
      {
        Log.Write($"Unable to find a browser matching {url}.");
        return;
      }

      (string path, string args) = Executable.GetPathAndArgs(pref.Browser.Location);

      // We need to use an absolute URI value, to prevent uri.ToString() - in this case some symbols in HTML encoding are replaced (for example %20)
      Process.Start(path, $"{args} {uri.AbsoluteUri}");
    }
    catch (Exception e)
    {
      Log.Write($"{e}");
    }
  }
}