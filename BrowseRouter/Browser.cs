namespace BrowseRouter;

public class Browser
{
  public string Name { get; set; }
  public string Location { get; set; }

  public override string ToString() => $"\"{Name}\" (\"{Location}\")";
}
