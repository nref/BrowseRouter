using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

public static class Kernel32
{
  [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
  public static extern nint GetModuleHandle(string? lpModuleName);

  private const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;

  [DllImport("kernel32.dll")]
  private static extern bool AttachConsole(uint dwProcessId);

  public static void AttachToParentConsole() => AttachConsole(ATTACH_PARENT_PROCESS);
}