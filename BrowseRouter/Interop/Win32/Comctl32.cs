using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

internal static partial class Comctl32
{
  /// <summary>
  /// Requires an app manifest file on .NET Core
  /// </summary>
  [LibraryImport("Comctl32.dll", StringMarshalling = StringMarshalling.Utf16)]
  public static partial nint LoadIconWithScaleDown(nint hinst, string pszName, int cx, int cy, out nint phico);
}
