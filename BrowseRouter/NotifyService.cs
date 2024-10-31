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
  private static nint _hInstance = Kernel32.GetModuleHandle(App.ExePath);

  public NotifyService() => LoadIcon();

  private static bool LoadIcon(int size = 512) =>
    Comctl32.LoadIconWithScaleDown(_hInstance, Icon.Application, size, size, out _hIcon) != 0;

  public async Task NotifyAsync(string title, string message)
  {
    // Create a dummy window handle
    nint hWnd = CreateDummyWindow();

    NotifyIconData nid = GetNid(hWnd, title, message);

    // Add the icon. This also adds it to the system tray.
    Shell32.Shell_NotifyIcon(Shell32.NIM_ADD, ref nid);

    // We can also update the message
    //nid.szInfo = "This is an updated notification message";

    // Remain running long enough for the pop-up message to be displayed.
    // If we exit too early, the title has a GUID rather than the app name, and no icon.
    await Task.Delay(500);

    bool isWindows11 = Environment.OSVersion.Version.Build > 2200;

    if (isWindows11)
      return;

    // Windows 11 removes the tray icon when the app exits.
    // On Windows 10, we have to remove it manually.
    // But also on Windows 10, removing the icon immediately hides the pop-up message, so we have to delay.
    // On Windows 11, the pop-up remains for the full duration,
    // regardless if the app has exited or we delete the tray icon.
    var delay = TimeSpan.FromSeconds(10);
    await Task.Delay(delay);
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
    hIcon = _hIcon,
    hBalloonIcon = _hIcon,
    szTip = "BrowseRouter",
    szInfo = message,
    szInfoTitle = title,
    dwInfoFlags = Shell32.NIIF_USER | Shell32.NIIF_LARGE_ICON,
    dwState  = 0, // For the popup to be shown, the system tray icon must not be hidden, but we can hide it immediately after
    dwStateMask = Shell32.NIS_HIDDEN,
    uVersion = Shell32.NOTIFYICON_VERSION_4
  };

  private static nint CreateDummyWindow() => User32.CreateWindowEx(
    0, "STATIC", "DummyWindow", 0, 0, 0, 0, 0, nint.Zero, nint.Zero, nint.Zero, nint.Zero);
}
