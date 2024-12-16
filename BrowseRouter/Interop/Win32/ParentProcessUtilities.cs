using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32
{
  /// <summary>
  /// A utility class to determine a process parent. Originally copied from https://stackoverflow.com/a/3346055
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct ParentProcessUtilities
  {
    // These members must match PROCESS_BASIC_INFORMATION
    internal IntPtr Reserved1;
    internal IntPtr PebBaseAddress;
    internal IntPtr Reserved2_0;
    internal IntPtr Reserved2_1;
    internal IntPtr UniqueProcessId;
    internal IntPtr InheritedFromUniqueProcessId;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);


    /// <summary>
    /// Try to get the name of the parent process of the current process.
    /// </summary>
    /// <param name="parentProcessTitle">The name of the parent process main window title (may be empty) and the specific process name.</param>
    /// <returns>True if the name was succesfully found, False otherwise.</returns>
    public static bool TryGetParentProcessTitle(out string parentProcessTitle)
    {
      Process? parentProcess = GetParentProcess();
      if (parentProcess is null || (parentProcess.MainWindowTitle == string.Empty && parentProcess.ProcessName == string.Empty) )
      {
        parentProcessTitle = string.Empty;
        return false;
      }

      parentProcessTitle = parentProcess.MainWindowTitle + " -> " + parentProcess.ProcessName;
      return true;
    }

    /// <summary>
    /// Gets the parent process of the current process.
    /// </summary>
    /// <returns>An instance of the Process class.</returns>
    public static Process? GetParentProcess()
    {
      return GetParentProcess(Process.GetCurrentProcess().Handle);
    }

    /// <summary>
    /// Gets the parent process of specified process.
    /// </summary>
    /// <param name="id">The process id.</param>
    /// <returns>An instance of the Process class.</returns>
    public static Process? GetParentProcess(int id)
    {
      Process process = Process.GetProcessById(id);
      return GetParentProcess(process.Handle);
    }

    /// <summary>
    /// Gets the parent process of a specified process.
    /// </summary>
    /// <param name="handle">The process handle.</param>
    /// <returns>An instance of the Process class.</returns>
    public static Process? GetParentProcess(IntPtr handle)
    {
      ParentProcessUtilities pbi = new();
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
