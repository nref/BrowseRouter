using System.Security.Principal;

namespace BrowseRouter;

public class ElevationService
{
  public void RequireAdmin()
  {
    WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
    {
      Log.Write($"{App.FriendlyName} needs elevated privileges. Try to run it as admin.");
      Environment.Exit(-1);
    }
  }
}
