namespace BrowseRouter.Infrastructure;

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

      string dir = AppDomain.CurrentDomain.BaseDirectory;
      _exePath = Path.Combine(dir, AppDomain.CurrentDomain.FriendlyName + ".exe");
      return _exePath;
    }
  }
}
