using System.Diagnostics;
using Microsoft.Win32;

namespace BrowseRouter;

public class RegistryService(INotifyService notifier)
{
  private const string AppName = "BrowseRouter";
  private const string AppID = "BrowseRouter";
  private const string AppDescription = "Opens a different brower based on the URL";
  private string AppIcon => App.ExePath + ",0";
  private string AppOpenUrlCommand => App.ExePath + " %1";

  private string AppKey => $"SOFTWARE\\{AppID}";
  private string UrlKey => $"SOFTWARE\\Classes\\{AppID}URL";
  private string CapabilityKey => $"SOFTWARE\\{AppID}\\Capabilities";

  private RegistryKey? _registerKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RegisteredApplications", true);

  public async Task RegisterAsync()
  {
    Log.Write("Registering...");
    RegistryKey? appReg = Registry.CurrentUser.CreateSubKey(AppKey);

    RegisterCapabilities(appReg);

    _registerKey?.SetValue(AppID, CapabilityKey);

    HandleUrls();

    OpenSettings();
    Log.Write($"Done. Please set {AppName} as the default browser in Settings.");
    await notifier.NotifyAsync("Registered as a browser.", "Please set BrowseRouter as the default browser in Settings.");
  }

  private static void OpenSettings() => Process.Start(new ProcessStartInfo
  {
    FileName = $"ms-settings:defaultapps?registeredAppUser={AppName}",
    UseShellExecute = true
  });

  private void RegisterCapabilities(RegistryKey appReg)
  {
    // Register capabilities.
    RegistryKey? capabilityReg = appReg.CreateSubKey("Capabilities");
    capabilityReg.SetValue("ApplicationName", AppName);
    capabilityReg.SetValue("ApplicationIcon", AppIcon);
    capabilityReg.SetValue("ApplicationDescription", AppDescription);

    // Set up protocols we want to handle.
    RegistryKey? urlAssocReg = capabilityReg.CreateSubKey("URLAssociations");
    urlAssocReg.SetValue("http", AppID + "URL");
    urlAssocReg.SetValue("https", AppID + "URL");
    urlAssocReg.SetValue("ftp", AppID + "URL");
  }

  /// <summary>
  /// Set URL Handler
  /// </summary>
  private void HandleUrls()
  {
    RegistryKey? handlerReg = Registry.CurrentUser.CreateSubKey(UrlKey);
    handlerReg.SetValue("", AppName);
    handlerReg.SetValue("FriendlyTypeName", AppName);
    handlerReg.CreateSubKey("shell\\open\\command").SetValue("", AppOpenUrlCommand);
  }

  public void Unregister()
  {
    Log.Write("Unregistering...");
    Try(() => Registry.CurrentUser.DeleteSubKeyTree(AppKey, false));
    Try(() => _registerKey?.DeleteValue(AppID));
    Try(() => Registry.CurrentUser.DeleteSubKeyTree(UrlKey));
    Log.Write("Done");
  }

  private static void Try(Action a)
  {
    try
    {
      a();
    }
    catch
    {
    }
  }
}
