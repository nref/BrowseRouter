namespace BrowseRouter;

public class Browser
{
  public Browser(string name, string location)
  {
    Name = name;
    Location = location;
  }

  public string Name { get; set; }
  public string Location { get; set; }

  public override string ToString() => $"\"{Name}\" (\"{Location}\")";
}
