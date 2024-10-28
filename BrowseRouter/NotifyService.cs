using System.Runtime.InteropServices;
using BrowseRouter.Interop.Win32;

namespace BrowseRouter;

public interface INotifyService
{
  public Task NotifyAsync(string title, string message);
}

public class EmptyNotifyService : INotifyService
{
  public Task NotifyAsync(string title, string message) => Task.CompletedTask;
} 

public class NotifyService : INotifyService
{
  private const uint _flags = Shell32.NIF_ICON 
    | Shell32.NIF_INFO 
    | Shell32.NIF_MESSAGE 
    | Shell32.NIF_STATE; // Enables hiding system tray icon

  private static nint _hIcon;

  public NotifyService() => LoadIcon();

  private static bool LoadIcon(string file = "logo.ico", int size = 512) =>
    Comctl32.LoadIconWithScaleDown(nint.Zero, file, size, size, out _hIcon) != 0;

  public async Task NotifyAsync(string title, string message)
  {
    // Create a dummy window handle
    IntPtr hWnd = CreateDummyWindow();

    NotifyIconData nid = GetNid(hWnd, title, message);

    // Add the icon. This also adds it to the system tray.
    Shell32.Shell_NotifyIcon(Shell32.NIM_ADD, ref nid);

    // We can also update the message
    //nid.szInfo = "This is an updated notification message";

    // Remain running long enough for the pop-up message to be displayed.
    // If we exit too early, the title has a GUID rather than the app name, and no icon.
    await Task.Delay(500);

    // Remove the icon from the system tray
    //Shell32.Shell_NotifyIcon(Shell32.NIM_DELETE, ref nid);

    // Hide the icon to keep the pop-up message visible
    nid.dwState = Shell32.NIS_HIDDEN;
    Shell32.Shell_NotifyIcon(Shell32.NIM_MODIFY, ref nid);

    // Destroy the dummy window
    User32.DestroyWindow(hWnd);

    Remove(nid);
  }

  public void Remove(NotifyIconData nid)
  {
    // Remove the icon from the system tray
    Shell32.Shell_NotifyIcon(Shell32.NIM_DELETE, ref nid);
  }

  private static NotifyIconData GetNid(nint hWnd, string title, string message) => new NotifyIconData
  {
    cbSize = Marshal.SizeOf(typeof(NotifyIconData)),
    hWnd = hWnd,
    uID = 1,
    uFlags = _flags,
    uCallbackMessage = 0x500, // WM_USER + 1
    hIcon = nint.Zero,
    hBalloonIcon = _hIcon,
    szTip = "BrowseRouter",
    szInfo = message,
    szInfoTitle = title,
    dwInfoFlags = Shell32.NIIF_USER | Shell32.NIIF_LARGE_ICON,
    dwState  = 0, // For the popup to be shown, the system tray icon must not be hidden, but we can hide it immediately after
    dwStateMask = Shell32.NIS_HIDDEN,
    uVersion = Shell32.NOTIFYICON_VERSION_4
  };

  private static IntPtr CreateDummyWindow() => User32.CreateWindowEx(
    0, "STATIC", "DummyWindow", 0, 0, 0, 0, 0, nint.Zero, nint.Zero, nint.Zero, nint.Zero);
}
