namespace BrowseRouter;

public class UrlPreference
{
  public string UrlPattern { get; set; }
  public Browser Browser { get; set; }

  public override string ToString() => $"\"{UrlPattern}\" => {Browser}";
}
