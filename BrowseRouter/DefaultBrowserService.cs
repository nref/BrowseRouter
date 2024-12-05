using System.Diagnostics;
using Microsoft.Win32;

namespace BrowseRouter;

public class DefaultBrowserService(INotifyService notifier)
{
  private const string _appName = "BrowseRouter";
  private const string _appID = "BrowseRouter";
  private const string _appDescription = "Opens a different brower based on the URL";
  private string AppIcon => App.ExePath + ",0";
  private string AppOpenUrlCommand => App.ExePath + " %1";

  private string AppKey => $"SOFTWARE\\{_appID}";
  private string UrlKey => $"SOFTWARE\\Classes\\{_appID}URL";
  private string CapabilityKey => $"SOFTWARE\\{_appID}\\Capabilities";

  private readonly RegistryKey? _registerKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RegisteredApplications", true);
  private RegistryKey? AppRegKey => Registry.CurrentUser.OpenSubKey(AppKey);
  private RegistryKey? UrlRegKey => Registry.CurrentUser.OpenSubKey(UrlKey);

  private string? ExistingOpenUrlCommand => UrlRegKey
        ?.OpenSubKey("shell\\open\\command")
        ?.GetValue("") as string;

  private enum RegisterStatus
  {
    Registered,
    Unregistered,
    Updated
  }

  private RegisterStatus GetRegisterStatus() => AppRegKey switch
  {
    null => RegisterStatus.Unregistered,
    _ => ExistingOpenUrlCommand == AppOpenUrlCommand
        ? RegisterStatus.Registered
        : RegisterStatus.Updated
  };

  public async Task RegisterOrUnregisterAsync()
  {
    RegisterStatus status = GetRegisterStatus();

    if (status == RegisterStatus.Unregistered)
    {
      await RegisterAsync();
      return;
    }

    if (status == RegisterStatus.Registered)
    {
      await UnregisterAsync();
      return;
    }

    if (status == RegisterStatus.Updated)
    {
      Unregister(); // Unregister the old path
      Register(); // Register with the new path
      await notifier.NotifyAsync("Updated location.", $"{_appName} has been re-registered with a new path.");
    }
  }

  public async Task RegisterAsync()
  {
    Register();
    await notifier.NotifyAsync("Registered as a browser.", $"Please set {_appName} as the default browser in Settings.");
  }

  private void Register()
  {
    Log.Write("Registering...");
    RegistryKey? appReg = Registry.CurrentUser.CreateSubKey(AppKey);

    RegisterCapabilities(appReg);

    _registerKey?.SetValue(_appID, CapabilityKey);

    HandleUrls();

    OpenSettings();
    Log.Write($"Done. Please set {_appName} as the default browser in Settings.");
  }

  private static void OpenSettings() => Process.Start(new ProcessStartInfo
  {
    FileName = $"ms-settings:defaultapps?registeredAppUser={_appName}",
    UseShellExecute = true
  });

  private void RegisterCapabilities(RegistryKey appReg)
  {
    // Register capabilities.
    RegistryKey? capabilityReg = appReg.CreateSubKey("Capabilities");
    capabilityReg.SetValue("ApplicationName", _appName);
    capabilityReg.SetValue("ApplicationIcon", AppIcon);
    capabilityReg.SetValue("ApplicationDescription", _appDescription);

    // Set up protocols we want to handle.
    RegistryKey? urlAssocReg = capabilityReg.CreateSubKey("URLAssociations");
    urlAssocReg.SetValue("http", _appID + "URL");
    urlAssocReg.SetValue("https", _appID + "URL");
    urlAssocReg.SetValue("ftp", _appID + "URL");
  }

  /// <summary>
  /// Set URL Handler
  /// </summary>
  private void HandleUrls()
  {
    RegistryKey? handlerReg = Registry.CurrentUser.CreateSubKey(UrlKey);
    handlerReg.SetValue("", _appName);
    handlerReg.SetValue("FriendlyTypeName", _appName);
    handlerReg.CreateSubKey("shell\\open\\command").SetValue("", AppOpenUrlCommand);
  }

  public async Task UnregisterAsync()
  {
    Unregister();
    await notifier.NotifyAsync("Unregistered as a browser.", $"{_appName} is no longer registered as a web browser.");
  }

  private void Unregister()
  {
    Log.Write("Unregistering...");
    Actions.TryRun(() => Registry.CurrentUser.DeleteSubKeyTree(AppKey, false));
    Actions.TryRun(() => _registerKey?.DeleteValue(_appID));
    Actions.TryRun(() => Registry.CurrentUser.DeleteSubKeyTree(UrlKey));
    Log.Write("Done");
  }
}