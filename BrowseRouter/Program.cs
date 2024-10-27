using BrowseRouter.Interop.Win32;

namespace BrowseRouter;

public static class Program
{
  private static async Task Main(string[] args)
  {
    if (args.Length == 0)
    {
      await new RegistryService(new NotifyService()).RegisterAsync();
      return;
    }

    // Process each URL in the arguments list.
    foreach (string arg in args)
    {
      await RunAsync(arg.Trim().ToLower());
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
      RunOption(arg);
      return;
    }

    await LaunchUrlAsyc(arg);
  }

  private static bool RunOption(string arg)
  {
    if (string.Equals(arg, "h") || string.Equals(arg, "help"))
    {
      ShowHelp();
      return true;
    }

    if (string.Equals(arg, "u") || string.Equals(arg, "unregister"))
    {
      new RegistryService(new NotifyService()).Unregister();
      return true;
    }

    return false;
  }

  private static async Task LaunchUrlAsyc(string url)
  {
    // Get the window title for whichever application is opening the URL.
    var windowTitle = User32.GetActiveWindowTitle();

    var configService = new ConfigService();
    Log.Preference = configService.GetLogPreference();

    var notifyPref = configService.GetNotifyPreference();
    INotifyService notifier = notifyPref.IsEnabled switch
    {
      true => new NotifyService(),
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