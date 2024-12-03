namespace BrowseRouter
{
  public class LogPreference
  {
    public static string DefaultLogRoot => $"{Env.LocalAppData}/BrowseRouter/";
    public static string DefaultLogFile => $"{DefaultLogRoot}{DateTime.Today:yyyy-MM-dd}.log";

    public bool IsEnabled { get; set; }
    public string File { get; set; } = DefaultLogFile;
  }
}
