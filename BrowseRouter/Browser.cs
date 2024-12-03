namespace BrowseRouter
{
  public class Browser
  {
    public required string Name { get; set; }
    public required string Location { get; set; }

    public override string ToString() => $"\"{Name}\" (\"{Location}\")";
  }
}
