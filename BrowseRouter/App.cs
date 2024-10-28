using System.Reflection;

namespace BrowseRouter;

public static class App
{
  private static string exePath;

  public static string ExePath
  {
    get
    {
      if (!string.IsNullOrEmpty(exePath))
      {
        return exePath;
      }

      var assembly = Assembly.GetExecutingAssembly();
      // .NET Core returns BrowserSettings.dll but we need BrowserSettings.exe
      exePath = Assembly.GetExecutingAssembly().Location
          .Replace(".dll", ".exe");

      if (!string.IsNullOrEmpty(exePath))
      {
        return exePath;
      }

      var dir = AppDomain.CurrentDomain.BaseDirectory;
      exePath = Path.Combine(dir, AppDomain.CurrentDomain.FriendlyName + ".exe");
      return exePath;
    }
  }
}
