using System.Runtime.InteropServices;
using System.Text;

namespace BrowseRouter.Interop.Win32;

public static partial class User32
{
  // DllImports used to get window title for source program.
  [LibraryImport("user32.dll")]
  public static partial nint GetForegroundWindow();

  [LibraryImport("user32.dll", EntryPoint = "GetWindowTextW", StringMarshalling = StringMarshalling.Utf16)]
  public static partial int GetWindowText(nint hWnd, [Out] char[] lpString, int nMaxCount);

  public static string GetActiveWindowTitle()
  {
    string result = "";
    const int nChars = 256;
    char[] buff = new char[nChars];
    nint handle = GetForegroundWindow();

    int length = GetWindowText(handle, buff, nChars);
    if (length > 0)
    {
      result = new string(buff, 0, length);
    }
    return result;
  }

  [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", StringMarshalling = StringMarshalling.Utf16)]
  public static partial nint CreateWindowEx(uint dwExStyle, 
    string lpClassName, 
    string lpWindowName, 
    uint dwStyle, int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

  [LibraryImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static partial bool DestroyWindow(nint hWnd);
}
