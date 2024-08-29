namespace BrowseRouter;

public class LogPreference
{
  public static string DefaultLogFile => $"{Env.LocalAppData}/BrowseRouter/{DateTime.Today:yyyy-MM-dd}.log";

  public bool IsEnabled { get; set; }
  public string File { get; set; } = DefaultLogFile;
}
