using System.Security.Principal;

namespace BrowseRouter;

public interface IElevationService
{
  void RequireAdmin();
}

public static class ElevationServiceFactory
{
  public static IElevationService Create()
  {
    if (System.OperatingSystem.IsWindows())
      return new WindowsElevationService();
    else
      return new NopElevationService();
  }
}

public class NopElevationService : IElevationService
{
  public void RequireAdmin() {}
}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class WindowsElevationService : IElevationService
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
