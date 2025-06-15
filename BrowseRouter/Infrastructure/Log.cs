using BrowseRouter.Config;

namespace BrowseRouter.Infrastructure;

public static class Log
{
  public static LogPreference Preference { get; set; } = new LogPreference();

  public static void Write(string message)
  {
    if (!Preference.IsEnabled) { return; }

    string msg = $"{DateTime.Now} {nameof(BrowseRouter)}: {message}";
    Console.WriteLine(msg);

    EnsureLogDirExists();
    TryWrite(msg);
  }

  private static void EnsureLogDirExists()
  {
    string? parent = Path.GetDirectoryName(Preference.File);
    if (parent is not null)
    {
      Actions.TryRun(() => Directory.CreateDirectory(parent), logOnlyToConsole: true);
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
        Console.WriteLine($"Failed to write log file {Preference.File} on attempt {i + 1}: {e}");
      }
      Thread.Sleep(100);
    }
  }
}
