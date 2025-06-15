using System.Text.RegularExpressions;

namespace BrowseRouter;

public static partial class FindReplacer
{
  [GeneratedRegex(@"unescape\(\$(\d+)\)", RegexOptions.Compiled)]
  private static partial Regex UnescapeRegex();
  [GeneratedRegex(@"\$(\d+)", RegexOptions.Compiled)]
  private static partial Regex GroupRegex();
  
  private static readonly Regex _unescapeRegex  = UnescapeRegex();
  private static readonly Regex _groupRegex = GroupRegex();

  public static string Replace(string input, string pattern, string template) =>
    Replace(input, new Regex(pattern, RegexOptions.Compiled), template);

  private static string Replace(string input, Regex rx, string template) =>
    rx.Replace(input, match =>
    {
      var tmp = _unescapeRegex.Replace(template, m =>
      {
        var n = int.Parse(m.Groups[1].Value);
        return Uri.UnescapeDataString(match.Groups[n].Value);
      });

      return _groupRegex.Replace(tmp, m =>
      {
        var n = int.Parse(m.Groups[1].Value);
        return match.Groups[n].Value;
      });
    });
}
