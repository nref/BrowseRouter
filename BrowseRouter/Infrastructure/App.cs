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

      _exePath = Environment.ProcessPath ?? string.Empty;
      return _exePath;
    }
  }
}
