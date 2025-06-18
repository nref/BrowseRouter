using System.Diagnostics;
using BrowseRouter.Interop.Win32;

namespace BrowseRouter.Services;

public interface IProcessService
{
    void Start(string path, string args);

  /// <summary>
  /// Try to get the name of the parent process of the current process.
  /// </summary>
  /// <param name="parentProcessTitle">The name of the parent process main window title (may be empty) and the specific process name.</param>
  /// <returns>True if the name was succesfully found, False otherwise.</returns>
  bool TryGetParentProcessTitle(out string parentProcessTitle);
}

public class ProcessService : IProcessService
{
  public void Start(string path, string args)
  {
    Process.Start(path, args);
  }

  public bool TryGetParentProcessTitle(out string parentProcessTitle)
  {
    Process? parentProcess = GetParentProcess();
    if (parentProcess is null || parentProcess.MainWindowTitle == string.Empty && parentProcess.ProcessName == string.Empty)
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
    return BasicProcessInfo.GetParentProcess(Process.GetCurrentProcess().Handle);
  }

  /// <summary>
  /// Gets the parent process of specified process.
  /// </summary>
  /// <param name="id">The process id.</param>
  /// <returns>An instance of the Process class.</returns>
  public static Process? GetParentProcess(int id)
  {
    Process process = Process.GetProcessById(id);
    return BasicProcessInfo.GetParentProcess(process.Handle);
  }
}
