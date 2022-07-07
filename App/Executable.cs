namespace BrowseRouter;

public static class Executable
{
  /// <summary>
  /// Return the quoted executable path and subsquent args i.e. <c>"chrome.exe --new-window"</c> → <c>("chrome.exe", "--new-window")</c>
  /// or the unquoted executable path and no args i.e. <c>"chrome.exe</c> → <c>("chrome.exe", "")</c>
  /// </summary>
  public static (string, string) GetPathAndArgs(string s)
  {
    int q1 = s.IndexOf('"', 0);
    int q2 = s.IndexOf('"', q1 + 1);

    // If browser path is quoted
    if (q1 >= 0 && q2 > 0)
    {
      // The quoted text is the executable, the rest command-line args.
      string path = s[(q1 + 1)..q2];
      string args = s[(q2 + 1)..].Trim();
      return (path, args);
    }

    // The single executable without any other arguments.
    return (s, "");
  }

}