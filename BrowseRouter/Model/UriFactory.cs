namespace BrowseRouter.Model;

public static class UriFactory
{
  public static Uri Get(string url)
  {
    try
    {
      return new Uri(url);
    }
    catch (UriFormatException)
    {
      // Try to prepend https when given an incomplete URI e.g. "google.com"
      return new Uri($"https://{url}");
    }
  }
}