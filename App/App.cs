using System.Reflection;

namespace BrowseRouter;

public static class App
{
  static string ComputeExePath()
  {
    // There exist a variety of methods to obtain the executable path:
    //
    // System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName
    //   Windows, normal build:           …\bin\Debug\net6.0\BrowseRouter.exe
    //   Windows, single file publishing: …\bin\Debug\net6.0\win-x64\BrowseRouter.exe
    //   Linux, normal build:             …/bin/Debug/net6.0/BrowseRouter
    //   Linux, single file publishing:   …/bin/Debug/net6.0/linux-x64/publish/BrowseRouter
    //   Renames of the executable are also handled correctly by this method.
    //
    // Assembly.GetExecutingAssembly().Location
    //   Windows, normal build:           …\bin\Debug\net6.0\BrowseRouter.dll
    //   Windows, single file publishing: …\bin\Debug\net6.0\win-x64\BrowseRouter.dll
    //   Linux, normal build:             …/bin/Debug/net6.0/BrowseRouter.dll
    //   Linux, single file publishing:   (null)
    //
    // AppContext.BaseDirectory
    // AppDomain.CurrentDomain.BaseDirectory
    //   Windows, normal build:           …\bin\Debug\net6.0\
    //   Windows, single file publishing: …\bin\Debug\net6.0\win-x64\
    //   Linux, normal build:             …/bin/Debug/net6.0/
    //   Linux, single file publishing:   …/bin/Debug/net6.0/linux-x64/publish/
    //
    // GetCurrentProcess().MainModule is the most reliable one and works across platforms.
    // Theoretically it seems possible for MainModule or MainModule.FileName to be null, so we
    // provide a fallback based on AppDomain.CurrentDomain.BaseDirectory as well.

    var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
    if (module != null && module.FileName != null)
      return module.FileName;

    // Fallback method
    var dir = AppDomain.CurrentDomain.BaseDirectory;
    var ext = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
            ? ".exe"
            : "";
    return Path.Combine(dir, FriendlyName + ext);
  }

  public static string FriendlyName { get; } = AppDomain.CurrentDomain.FriendlyName;

  public static string ExePath { get; } = ComputeExePath();

  public static string BaseDir { get; } = AppDomain.CurrentDomain.BaseDirectory;
}
