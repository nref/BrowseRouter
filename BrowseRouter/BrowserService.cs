using System.Diagnostics;

namespace BrowseRouter
{
  public class BrowserService(IConfigService config, INotifyService notifier)
  {
    public async Task LaunchAsync(string url, string windowTitle)
    {
      try
      {
        Log.Write($"Attempting to launch \"{url}\" for \"{windowTitle}\"");

        var urlPreferences = config.GetUrlPreferences("urls");
        var sourcePreferences = config.GetUrlPreferences("sources");
        var uri = UriFactory.Get(url);

        UrlPreference? pref = null;
        if (sourcePreferences.TryGetPreference(windowTitle, out var sourcePref))
        {
          Log.Write($"Found source preference {sourcePref}");
          pref = sourcePref;
        }

        else if (urlPreferences.TryGetPreference(uri, out var urlPref))
        {
          Log.Write($"Found URL preference {urlPref}");
          pref = urlPref;
        }

        if (pref == null)
        {
          Log.Write($"Unable to find a browser matching \"{url}\".");
          return;
        }

        var (path, args) = Executable.GetPathAndArgs(pref.Browser.Location);

        Log.Write($"Launching {path} with args \"{args} {uri.OriginalString}\"");

        var name = GetAppName(path);
        await notifier.NotifyAsync($"Opening {name}", $"URL: {url}");
        
        path = Environment.ExpandEnvironmentVariables(path);

        Process.Start(path, $"{args} \"{uri.OriginalString}\"");
      }
      catch (Exception e)
      {
        Log.Write($"{e}");
      }
    }

    private static string GetAppName(string path)
    {
      // Get just the app name from the exe at path
      var name = Path.GetFileNameWithoutExtension(path);
      // make first letter uppercase
      name = name[0].ToString().ToUpper() + name[1..];
      return name;
    }
  }
}
