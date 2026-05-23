using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

public static partial class Kernel32
{
  [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
  public static partial nint GetModuleHandle(string? lpModuleName);

  private const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;

  [LibraryImport("kernel32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static partial bool AttachConsole(uint dwProcessId);

  public static void AttachToParentConsole() => AttachConsole(ATTACH_PARENT_PROCESS);
}