using System.Runtime.InteropServices;

namespace BrowseRouter.Interop.Win32;

public static class Kernel32
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern nint GetModuleHandle(string? lpModuleName);
}