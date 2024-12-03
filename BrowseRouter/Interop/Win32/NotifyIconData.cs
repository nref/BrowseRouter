using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct NotifyIconData
  {
    public int cbSize;
    public nint hWnd;
    public int uID;
    public uint uFlags;
    public int uCallbackMessage;
    public nint hIcon;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szTip;
    public int dwState;
    public int dwStateMask;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string szInfo;
    public uint uVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string szInfoTitle;
    public uint dwInfoFlags;
    public Guid guidItem;
    public nint hBalloonIcon;
  }
}
