namespace BrowseRouter;

public class Program
{
  private static void Main(string[] args)
  {
    if (args == null || args.Length == 0)
    {
      ShowHelp();
      return;
    }

    foreach (string arg in args)
    {
      Run(arg);
    }
  }

  private static void Run(string arg)
  {
    string a = arg.Trim();

    bool isOption = a.StartsWith("-") || a.StartsWith("/");
    while (a.StartsWith("-") || a.StartsWith("/"))
    {
      a = a[1..];
    }

    if (!isOption)
    {
      new BrowserService(new ConfigService()).Launch(a);
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
      return;
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
}
