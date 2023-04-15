namespace BrowseRouter;

public class UrlPreference
{
  public UrlPreference(string urlPattern, Browser browser)
  {
    UrlPattern = urlPattern;
    Browser = browser;
  }

  public string UrlPattern { get; set; }
  public Browser Browser { get; set; }

  public override string ToString() => $"\"{UrlPattern}\" => {Browser}";
}
