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

    new ElevationService().RequireAdmin();

    if (string.Equals(a, "register", StringComparison.OrdinalIgnoreCase))
    {
      new RegistryService().Register();
      return;
    }

    if (string.Equals(a, "unregister", StringComparison.OrdinalIgnoreCase))
    {
      new RegistryService().Unregister();
    }
  }

  private static void ShowHelp()
  {
    Log.Write
    (
$@"{nameof(BrowseRouter)}: In Windows, launch a different browser depending on the url.

   Usage:

    BrowseRouter.exe --register
        Register as a web browser.

    BrowseRouter.exe --unregister
        Unregister as a web browser. 
        Once you have registered the app as a browser, you should use visit ""Set Default Browser"" in Windows to set this app as the default browser.

    BrowseRouter.exe http://example.org/
        Launch a URL"
    );
  }

  // DllImports used to get window title for source program.
  [DllImport("user32.dll")]
  static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")]
  static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

  private static string GetActiveWindowTitle()
  {
    string result = "";
    const int nChars = 256;
    StringBuilder Buff = new(nChars);
    IntPtr handle = GetForegroundWindow();

    if (GetWindowText(handle, Buff, nChars) > 0)
    {
      result = Buff.ToString();
    }
    return result;
  }
}