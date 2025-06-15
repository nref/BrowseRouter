namespace BrowseRouter.Config;

public record FilterPreference(string Name, string Find, string Replace, int Priority)
{

  public static bool TryApply(IEnumerable<FilterPreference> filters, string input, out string output)
  {
    string temp = string.Empty;

    bool ok = filters.Any(filter => filter.TryApply(input, out temp));
    
    output = ok ? temp : input;
    return ok;
  }

  public bool TryApply(string input, out string output)
  {
    output = FindReplacer.Replace(input, Find, Replace);
    return !string.Equals(input, output);
  }
}