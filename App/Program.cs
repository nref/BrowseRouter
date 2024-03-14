namespace BrowseRouter;
using System.Runtime.InteropServices;
using System.Text;

public class Program
{
  private static void Main(string[] args)
  {
    if (args.Length == 0)
    {
      ShowHelp();
      return;
    }

    // Get the window title for whichever application is opening the URL.
    var windowTitle = GetActiveWindowTitle();

    // Process each URL in the arguments list.
    foreach (string arg in args)
    {
      Run(arg, windowTitle);
    }
  }

  private static void Run(string arg, string windowTitle)
  {
    string a = arg.Trim();

    bool isOption = a.StartsWith("-") || a.StartsWith("/");
    while (a.StartsWith("-") || a.StartsWith("/"))
    {
      a = a[1..];
    }

    if (!isOption)
    {
      new BrowserService(new ConfigService()).Launch(a, windowTitle);
      return;
    }

    ElevationServiceFactory.Create().RequireAdmin();

    {
      var registration = RegistrationServiceFactory.Create();

      if (string.Equals(a, "register", StringComparison.OrdinalIgnoreCase))
      {
        registration.Register();
        return;
      }

      if (string.Equals(a, "unregister", StringComparison.OrdinalIgnoreCase))
      {
        registration.Unregister();
      }
    }
  }

  private static void ShowHelp()
  {
    var self = Path.GetFileName(App.ExePath);
    Log.Write
    (
$@"{App.FriendlyName}: Launch a different browser depending on the URL.

  Usage:

    {self} --register
        Register as a web browser.

    {self} --unregister
        Unregister as a web browser.
        Once you have registered the app as a browser, you may need to set it as the default browser.
        On Windows open ""Settings -> Apps -> Default apps"".
        On KDE/Plasma open ""System Settings -> Applications -> Default Applications"".
        On GNOME open ""Settings -> Default Applications"".

    {self} http://example.org/
        Launch a URL"
    );
  }

  // DllImports used to get window title for source program.
  [DllImport("user32.dll")]
  static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")]
  static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

/// <summary>
/// Returns the currently active window title.
/// Only implemented on Windows. Returns an empty string on other platforms.
/// </summary>
  private static string GetActiveWindowTitle()
  {
    string result = "";
    if (System.OperatingSystem.IsWindows())
    {
      const int nChars = 256;
      StringBuilder Buff = new(nChars);
      IntPtr handle = GetForegroundWindow();

      if (GetWindowText(handle, Buff, nChars) > 0)
      {
        result = Buff.ToString();
      }
    }
    return result;
  }
}
