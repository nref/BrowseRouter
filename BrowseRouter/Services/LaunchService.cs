namespace BrowseRouter.Services;

public interface ILaunchService
{
  Task LaunchUrlAsyc(string url);
}

public class LaunchService(
  IBrowserService browsers,
  IProcessService processes,
  IWindowTitleService titles
) : ILaunchService
{
  public async Task LaunchUrlAsyc(string url)
  {
    // Get the window title for whichever application is opening the URL.
    if (!processes.TryGetParentProcessTitle(out string windowTitle))
      // If it didn't work, fallback to the current foreground window name
      windowTitle = titles.GetWindowTitle();

    await browsers.LaunchAsync(url, windowTitle);
  }
}
