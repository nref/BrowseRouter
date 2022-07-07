using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace BrowserSelector;

public class ElevationService
{
  public void EnsureAdmin(string arg)
  {
    WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
    {
      return;
    }

    Process.Start(new ProcessStartInfo
    {
      FileName = App.ExePath,
      Verb = "runas",
      Arguments = arg
    });
    Environment.Exit(0);
  }
}
