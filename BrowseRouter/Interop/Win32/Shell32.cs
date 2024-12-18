﻿using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

public static class Shell32
{
  public const uint NIM_ADD = 0x00000000;
  public const uint NIM_MODIFY = 0x00000001;
  public const uint NIM_DELETE = 0x00000002;
  public const uint NIF_MESSAGE = 0x00000001;
  public const uint NIF_ICON = 0x00000002;
  public const uint NIF_TIP = 0x00000004;
  public const uint NIF_STATE = 0x00000008;
  public const uint NIF_INFO = 0x00000010;
  public const uint NIIF_INFO = 0x00000001;
  public const uint NIIF_USER = 0x00000004;
  public const uint NIIF_NOSOUND = 0x00000010;
  public const uint NIIF_LARGE_ICON = 0x00000020;
  public const int NIS_HIDDEN = 0x00000001;
  public const uint NOTIFYICON_VERSION_4 = 4;

  [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
  public static extern bool Shell_NotifyIcon(uint dwMessage, [In] ref NotifyIconData pnid);
}