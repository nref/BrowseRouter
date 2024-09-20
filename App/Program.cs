using BrowseRouter.Interop.Win32;

namespace BrowseRouter;

public static class Program
{
  private static async Task Main(string[] args)
  {
    if (args.Length == 0)
    {
      ShowHelp();
      return;
    }

    // Get the window title for whichever application is opening the URL.
    var windowTitle = User32.GetActiveWindowTitle();

    // Process each URL in the arguments list.
    foreach (string arg in args)
    {
      await RunAsync(arg, windowTitle);
    }
  }

  private static async Task RunAsync(string arg, string windowTitle)
  {
    string a = arg.Trim();

    bool isOption = a.StartsWith("-") || a.StartsWith("/");
    while (a.StartsWith("-") || a.StartsWith("/"))
    {
      a = a[1..];
    }

    var configService = new ConfigService();
    Log.Preference = configService.GetLogPreference();

    var notifyPref = configService.GetNotifyPreference();
    INotifyService notifier = notifyPref.IsEnabled switch
    {
      true => new NotifyService(),
      false => new EmptyNotifyService()
    };

    if (!isOption)
    {
      await new BrowserService(configService, notifier).LaunchAsync(a, windowTitle);
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

    BrowseRouter.exe https://example.org/
        Launch a URL"
    );
  }
}