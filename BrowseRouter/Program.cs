using BrowseRouter.Interop.Win32;

namespace BrowseRouter;

public static class Program
{
  private static async Task Main(string[] args)
  {
    if (args.Length == 0)
    {
      await new DefaultBrowserService(new NotifyService(false)).RegisterOrUnregisterAsync();
      return;
    }

    // Process each URL in the arguments list.
    foreach (string arg in args)
    {
      await RunAsync(arg.Trim());
    }
  }

  private static async Task RunAsync(string arg)
  {
    Func<bool> getIsOption = () => arg.StartsWith('-') || arg.StartsWith('/');

    bool isOption = getIsOption();
    while (getIsOption())
    {
      arg = arg[1..];
    }

    if (isOption)
    {
      await RunOption(arg);
      return;
    }

    await LaunchUrlAsyc(arg);
  }

  private static async Task<bool> RunOption(string arg)
  {
    if (string.Equals(arg, "h") || string.Equals(arg, "help"))
    {
      ShowHelp();
      return true;
    }

    if (string.Equals(arg, "r") || string.Equals(arg, "register"))
    {
      await new DefaultBrowserService(new NotifyService(false)).RegisterAsync();
      return true;
    }

    if (string.Equals(arg, "u") || string.Equals(arg, "unregister"))
    {
      await new DefaultBrowserService(new NotifyService(false)).UnregisterAsync();
      return true;
    }

    return false;
  }

  private static async Task LaunchUrlAsyc(string url)
  {
    // Get the window title for whichever application is opening the URL.
    ProcessService processService = new();
    if (!processService.TryGetParentProcessTitle(out string windowTitle))
      windowTitle = User32.GetActiveWindowTitle(); //if it didn't work we get the current foreground window name instead

    ConfigService configService = new();
    Log.Preference = configService.GetLogPreference();

    NotifyPreference notifyPref = configService.GetNotifyPreference();
    INotifyService notifier = notifyPref.IsEnabled switch
    {
      true => new NotifyService(notifyPref.IsSilent),
      false => new EmptyNotifyService()
    };

    await new BrowserService(configService, notifier).LaunchAsync(url, windowTitle);
  }

  private static void ShowHelp()
  {
    Log.Write
    (
$@"{nameof(BrowseRouter)}: In Windows, launch a different browser depending on the URL.

   https://github.com/nref/BrowseRouter

   Usage:

    BrowseRouter.exe [-h | --help]
        Show help.

    BrowseRouter.exe
        Automatic registration. 
        Same as --register if not already registered, otherwise --unregister.
        If the app has moved or been renamed, updates the existing registration.

    BrowseRouter.exe [-r | --register]
        Register as a web browser, then open Settings. 
        The user must choose BrowseRouter as the default browser.
        No need to run as admin.

    BrowseRouter.exe [-u | --unregister]
        Unregister as a web browser. 

    BrowseRouter.exe https://example.org/ [...more URLs]
        Launch one or more URLs"
    );
  }
}
