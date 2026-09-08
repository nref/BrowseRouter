using System.Text.RegularExpressions;

namespace BrowseRouter.Config;

internal static class PatternSyntax
{
  public static bool TryGetDelimited(string text, char delimiter, out string inner)
  {
    if (text.Length >= 2 && text[0] == delimiter && text[^1] == delimiter)
    {
      inner = text[1..^1];
      return true;
    }

    inner = text;
    return false;
  }

  public static string ToRegexPattern(string wildcard)
    => $"^{Regex.Escape(wildcard).Replace("\\*", ".*")}$";
}
