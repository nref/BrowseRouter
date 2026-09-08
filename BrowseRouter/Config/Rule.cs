using BrowseRouter.Model;

namespace BrowseRouter.Config;

public record Rule(Match Condition, Browser Browser)
{
  public override string ToString() => $"{Condition} => {Browser}";
}
