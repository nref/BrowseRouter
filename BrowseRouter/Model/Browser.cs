namespace BrowseRouter.Model;

public record Browser(string Name, string Location)
{
  public override string ToString() => $"\"{Name}\" (\"{Location}\")";
}
