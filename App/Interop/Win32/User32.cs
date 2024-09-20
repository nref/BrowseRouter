using System.Runtime.InteropServices;
using System.Text;

namespace BrowseRouter.Interop.Win32;

public static partial class User32
{
  // DllImports used to get window title for source program.
  [DllImport("user32.dll")]
  public static extern IntPtr GetForegroundWindow();

  [DllImport("user32.dll")]
  public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

  public static string GetActiveWindowTitle()
  {
    string result = "";
    const int nChars = 256;
    StringBuilder buff = new(nChars);
    IntPtr handle = GetForegroundWindow();

    if (GetWindowText(handle, buff, nChars) > 0)
    {
      result = buff.ToString();
    }
    return result;
  }

  [DllImport("user32.dll")]
  public static extern IntPtr CreateWindowEx(uint dwExStyle, 
    string lpClassName, 
    string lpWindowName, 
    uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

  [DllImport("user32.dll")]
  public static extern bool DestroyWindow(IntPtr hWnd);
}
