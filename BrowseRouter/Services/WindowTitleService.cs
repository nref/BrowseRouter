using BrowseRouter.Interop.Win32;

namespace BrowseRouter.Services;

public interface IWindowTitleService
{
  string GetWindowTitle();
}

public class WindowTitleService : IWindowTitleService
{
  public string GetWindowTitle() => User32.GetActiveWindowTitle();
}
