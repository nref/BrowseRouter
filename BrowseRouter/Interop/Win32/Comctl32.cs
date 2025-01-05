using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

internal static class Comctl32
{
  /// <summary>
  /// Requires an app manifest file on .NET Core
  /// </summary>
  [DllImport("Comctl32.dll", CharSet = CharSet.Unicode)]
  public static extern nint LoadIconWithScaleDown(nint hinst, string pszName, int cx, int cy, out nint phico);
}
