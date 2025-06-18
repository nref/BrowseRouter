using BrowseRouter.Services;

namespace BrowseRouter.Tests.TestDoubles;

internal class SpyProcessService : IProcessService
{
  public string? LastPath { get; private set; }
  public string? LastArgs { get; private set; }

  public void Start(string path, string args)
  {
    LastPath = path;
    LastArgs = args;
  }

  public bool TryGetParentProcessTitle(out string parentProcessTitle)
  {
    parentProcessTitle = string.Empty;
    return false;
  }
}
