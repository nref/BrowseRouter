using System.Reflection;

namespace BrowseRouter;

public static class App
{
  static string ComputeExePath()
  {
    var assembly = Assembly.GetExecutingAssembly();
    // .NET Core returns BrowserSettings.dll but we need BrowserSettings.exe
    var exePath = Assembly.GetExecutingAssembly().Location
        .Replace(".dll", ".exe");
    if (!string.IsNullOrEmpty(exePath))
    {
      return exePath;
    }

    var dir = AppDomain.CurrentDomain.BaseDirectory;
    return Path.Combine(dir, FriendlyName + ".exe");
  }

  public static string FriendlyName { get; } = AppDomain.CurrentDomain.FriendlyName;

  public static string ExePath { get; } = ComputeExePath();
}
