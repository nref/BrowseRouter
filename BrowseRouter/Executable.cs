namespace BrowseRouter
{
  public static class Executable
  {
    /// <summary>
    /// Return the quoted executable path and subsquent args i.e. <c>"chrome.exe --new-window"</c> → <c>("chrome.exe", "--new-window")</c>
    /// or the unquoted executable path and no args i.e. <c>"chrome.exe</c> → <c>("chrome.exe", "")</c>
    /// </summary>
    public static (string, string) GetPathAndArgs(string s)
    {
      var parts = s.Split("|");
      s = parts[0];
      int q1 = s.IndexOf('"', 0);
      int q2 = s.IndexOf('"', q1 + 1);

      // If browser path is quoted
      if (q1 >= 0 && q2 > 0)
      {
        // The quoted text is the executable, the rest command-line args.
        string path = s[(q1 + 1)..q2];
        string args = s[(q2 + 1)..].Trim();
        if (parts.Length > 1)
        {
          return (path, parts[1] + " " + args);
        }

        return (path, args);
      }

      // The single executable without any other arguments.
      if (parts.Length > 1)
      {
        return (s, parts[1]);
      }

      return (s, string.Empty);
    }
  }
}