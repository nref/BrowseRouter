using System.Diagnostics;

namespace BrowseRouter;

public static class Log
{
  public static EventLog _log = new("Application") { Source = "Application" };
  public static void Write(string message) => _log.WriteEntry(message);
}
