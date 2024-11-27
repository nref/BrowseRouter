using System.Reflection;

namespace BrowseRouter;

public static class App
{
  private static string? _exePath;

  public static string ExePath
  {
    get
    {
      if (!string.IsNullOrEmpty(_exePath))
      {
        return _exePath;
      }

      var assembly = Assembly.GetExecutingAssembly();
      // .NET Core returns BrowserSettings.dll but we need BrowserSettings.exe
      _exePath = assembly.Location
          .Replace(".dll", ".exe");

      if (!string.IsNullOrEmpty(_exePath))
      {
        return _exePath;
      }

      var dir = AppDomain.CurrentDomain.BaseDirectory;
      _exePath = Path.Combine(dir, AppDomain.CurrentDomain.FriendlyName + ".exe");
      return _exePath;
    }
  }
}
