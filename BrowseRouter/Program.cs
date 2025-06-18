using BrowseRouter.Config;
using BrowseRouter.Infrastructure;
using BrowseRouter.Interop.Win32;
using BrowseRouter.Services;

namespace BrowseRouter;

public static class Program
{
  private static async Task Main(string[] args)
  {
    // This enables e.g. showing --help in Terminal / cmd text even though this is a WinExe app.
    Kernel32.AttachToParentConsole();

    if (args.Length == 0)
    {
      await new DefaultBrowserService(new NotifyService()).RegisterOrUnregisterAsync();
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
    bool isOption = GetIsOption(arg);
    while (GetIsOption(arg))
    {
      arg = arg[1..];
    }

    if (isOption)
    {
      await RunOptionAsync(arg);
      return;
    }

    await LaunchUrlAsyc(arg);

  }

  private static bool GetIsOption(string arg) => arg.StartsWith('-') || arg.StartsWith('/');

  private static async Task<bool> RunOptionAsync(string arg)
  {
    if (string.Equals(arg, "h") || string.Equals(arg, "help"))
    {
      ShowHelp();
      return true;
    }

    if (string.Equals(arg, "r") || string.Equals(arg, "register"))
    {
      await new DefaultBrowserService(new NotifyService()).RegisterAsync();
      return true;
    }

    if (string.Equals(arg, "u") || string.Equals(arg, "unregister"))
    {
      await new DefaultBrowserService(new NotifyService()).UnregisterAsync();
      return true;
    }

    return false;
  }

  private static async Task LaunchUrlAsyc(string url)
  {
    IConfigService config = await ConfigServiceFactory.CreateAsync();
    Log.Preference = config.GetLogPreference();

    NotifyPreference notifyPref = config.GetNotifyPreference();
    INotifyService notifier = notifyPref.IsEnabled switch
    {
      true => new NotifyService(notifyPref.IsSilent),
      false => new EmptyNotifyService()
    };

    IBrowserService browsers = new BrowserService(config, notifier, new ProcessService());
    ILaunchService launchService = new LaunchService(browsers, new ProcessService(), new WindowTitleService());

    await launchService.LaunchUrlAsyc(url);
  }

  private static void ShowHelp()
  {
    Console.WriteLine
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
