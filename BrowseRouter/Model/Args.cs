using System.Text;

namespace BrowseRouter.Model;

public static class Args
{
  /// <summary>
  /// Return the quoted executable path and subsquent args i.e. <c>"chrome.exe" --new-window</c> → <c>("chrome.exe", "--new-window")</c>
  /// or the unquoted executable path and no args i.e. <c>"chrome.exe</c> → <c>("chrome.exe", "")</c>
  /// </summary>
  public static (string, string) SplitPathAndArgs(string s)
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

    // If not quoted, try to split on first space to separate path from args.
    // But only if the remainder doesn't contain a backslash, which would indicate
    // the first space is within the path itself (e.g., "C:\Program Files\...").
    // Users with paths containing spaces must quote the path.
    int spaceIndex = s.IndexOf(' ');
    if (spaceIndex > 0)
    {
      string afterSpace = s[(spaceIndex + 1)..];
      if (!afterSpace.Contains('\\'))
      {
        string path = s[..spaceIndex];
        return (path, afterSpace);
      }
    }

    // The single executable without any other arguments.
    return (s, "");
  }

  /// <summary>
  /// Complete the arguments to call the browser with, with the uri requested and according to the recognized tags in them.
  /// If no tags is recognized the uri is simply added to the end of the arguments.
  /// </summary>
  /// <param name="originalArgs">The unformatted arguments</param>
  /// <param name="uri">The URI to format the arguments with</param>
  /// <returns>The formatted arguments</returns>
  public static string Format(string originalArgs, Uri uri)
  {
    int tagReplacedCount = 0;
    StringBuilder args = new();

    int nextIndexToAdd = 0;
    int tagStartIndex = originalArgs.IndexOf('{');

    while (tagStartIndex >= 0)
    {
      args.Append(originalArgs.AsSpan(nextIndexToAdd, tagStartIndex - nextIndexToAdd));
      nextIndexToAdd = tagStartIndex;

      int tagEndIndex = originalArgs.IndexOf('}', tagStartIndex);
      if (tagEndIndex < 0) // if there's no more '}' in the rest of the args string
        break;

      bool successfullyReplacedTag = true;

      ReadOnlySpan<char> tagContent = originalArgs.AsSpan(tagStartIndex + 1, tagEndIndex - 1 - tagStartIndex);
      switch (tagContent)
      {
        case "url":
          args.Append(uri.OriginalString);
          break;
        case "userinfo":
          args.Append(uri.UserInfo);
          break;
        case "host":
          args.Append(uri.Host);
          break;
        case "port":
          args.Append(uri.Port);
          break;
        case "authority":
          args.Append(uri.Authority);
          break;
        case "path":
          args.Append(uri.AbsolutePath);
          break;
        case "query":
          args.Append(uri.Query);
          break;
        case "fragment":
          args.Append(uri.Fragment);
          break;

        default:
          successfullyReplacedTag = false;
          break;
      }

      if (successfullyReplacedTag)
      {
        tagReplacedCount++;

        nextIndexToAdd = tagEndIndex + 1;
        tagStartIndex = originalArgs.IndexOf('{', tagEndIndex);
      }
      else
        tagStartIndex = originalArgs.IndexOf('{', tagStartIndex + 1);
    }

    if (nextIndexToAdd < originalArgs.Length)
      args.Append(originalArgs.AsSpan(nextIndexToAdd));

    if (tagReplacedCount == 0)
      args.Append($" \"{uri.OriginalString}\"");

    return args.ToString();
  }
}