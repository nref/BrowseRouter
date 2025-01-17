using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32
{
  /// <summary>
  /// A utility class to determine a process parent. Originally copied from https://stackoverflow.com/a/3346055
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct BasicProcessInfo
  {
    // These members must match PROCESS_BASIC_INFORMATION
    internal IntPtr Reserved1;
    internal IntPtr PebBaseAddress;
    internal IntPtr Reserved2_0;
    internal IntPtr Reserved2_1;
    internal IntPtr UniqueProcessId;
    internal IntPtr InheritedFromUniqueProcessId;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref BasicProcessInfo processInformation, int processInformationLength, out int returnLength);


    /// <summary>
    /// Gets the parent process of a specified process.
    /// </summary>
    /// <param name="handle">The process handle.</param>
    /// <returns>An instance of the Process class.</returns>
    public static Process? GetParentProcess(IntPtr handle)
    {
      BasicProcessInfo pbi = new();
      int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
      if (status != 0)
        throw new Win32Exception(status);

      try
      {
        return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
      }
      catch (ArgumentException)
      {
        // not found
        return null;
      }
    }
  }
}
