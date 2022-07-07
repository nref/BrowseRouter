using Microsoft.Win32;

namespace BrowseRouter;

public class RegistryService
{
  private const string AppID = "BrowseRouter";
  private const string AppName = "BrowseRouter";
  private const string AppDescription = "Opens a different brower based on the URL";
  private string AppIcon => App.ExePath + ",0";
  private string AppOpenUrlCommand => App.ExePath + " %1";

  private string AppKey => $"SOFTWARE\\{AppID}";
  private string UrlKey => $"SOFTWARE\\Classes\\{AppID}URL";
  private string CapabilityKey => $"SOFTWARE\\{AppID}\\Capabilities";

  private RegistryKey? _registerKey => Registry.LocalMachine.OpenSubKey("SOFTWARE\\RegisteredApplications", true);

  public void Register()
  {
    // Register application.
    RegistryKey? appReg = Registry.LocalMachine.CreateSubKey(AppKey);

    RegisterCapabilities(appReg);

    // Register as application.
    if (_registerKey != null)
    {
      _registerKey.SetValue(AppID, CapabilityKey);
    }

    HandleUrls();
  }

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
    RegistryKey? handlerReg = Registry.LocalMachine.CreateSubKey(UrlKey);
    handlerReg.SetValue("", AppName);
    handlerReg.SetValue("FriendlyTypeName", AppName);
    handlerReg.CreateSubKey("shell\\open\\command").SetValue("", AppOpenUrlCommand);
  }

  public void Unregister()
  {
    Registry.LocalMachine.DeleteSubKeyTree(AppKey, false);

    if (_registerKey != null)
    {
      _registerKey.DeleteValue(AppID);
    }

    Registry.LocalMachine.DeleteSubKeyTree(UrlKey);
  }
}
