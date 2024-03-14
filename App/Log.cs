using System.Diagnostics;

namespace BrowseRouter;

public static class Log
{
  [System.Runtime.Versioning.SupportedOSPlatform("windows")]
  private static readonly EventLog eventLog_ = new("Application") { Source = "Application" };

  private static string logFile_ => "BrowseRouter.log";

  public static void Write(string message)
  {
    string msg = $"{DateTime.Now} {nameof(BrowseRouter)}: {message}";
    Console.WriteLine(msg);
    if (System.OperatingSystem.IsWindows())
    {
      eventLog_.WriteEntry(msg);
    }
    TryWrite(msg);
  }

  private static void TryWrite(string message)
  {
    foreach (int i in Enumerable.Range(0, 10))
    {
      try
      {
        using var writer = new StreamWriter(logFile_, append: true);
        writer.WriteLine(message);
        return;
      }
      catch (Exception e)
      {
        if (System.OperatingSystem.IsWindows())
        {
          eventLog_.WriteEntry(e.ToString());
        }
      }
    }
  }
}
