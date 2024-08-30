using System.Diagnostics;

namespace BrowseRouter;

public static class Log
{
  private static readonly EventLog eventLog_ = new("Application") { Source = "Application" };

  public static LogPreference Preference { get; set; } = new LogPreference();

  public static void Write(string message)
  {
    if (!Preference.IsEnabled) { return; }

    string msg = $"{DateTime.Now} {nameof(BrowseRouter)}: {message}";
    Console.WriteLine(msg);
    eventLog_.WriteEntry(msg);

    EnsureLogDirExists();
    TryWrite(msg);
  }

  private static void EnsureLogDirExists()
  {
    var parent = Path.GetDirectoryName(Preference.File);
    if (parent is not null)
    {
      Directory.CreateDirectory(parent);
    }
  }

  private static void TryWrite(string message)
  {
    foreach (int i in Enumerable.Range(0, 10))
    {
      try
      {
        using var writer = new StreamWriter(Preference.File, append: true);
        writer.WriteLine(message);
        return;
      }
      catch (Exception e)
      {
        eventLog_.WriteEntry(e.ToString());
      }
    }
  }
}
