using Microsoft.Win32;

namespace BrowserSelector;

public class RegistryService
{
  private const string AppID = "BrowserSelector";
  private const string AppName = "Browser Selector";
  private const string AppDescription = "Opens a different brower based on the URL";
  private string AppIcon => App.ExePath + ",0";
  private string AppOpenUrlCommand => App.ExePath + " %1";

  private RegistryKey? Key => Registry.LocalMachine.OpenSubKey("SOFTWARE\\RegisteredApplications", true);

  public void Register()
  {
    // Register application.
    var appReg = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\{AppID}");

    // Register capabilities.
    var capabilityReg = appReg.CreateSubKey("Capabilities");
    capabilityReg.SetValue("ApplicationName", AppName);
    capabilityReg.SetValue("ApplicationIcon", AppIcon);
    capabilityReg.SetValue("ApplicationDescription", AppDescription);

    // Set up protocols we want to handle.
    var urlAssocReg = capabilityReg.CreateSubKey("URLAssociations");
    urlAssocReg.SetValue("http", AppID + "URL");
    urlAssocReg.SetValue("https", AppID + "URL");
    urlAssocReg.SetValue("ftp", AppID + "URL");

    // Register as application.
    if (Key != null)
    {
      Key.SetValue(AppID, $"SOFTWARE\\{AppID}\\Capabilities");
    }

    // Set URL Handler.
    var handlerReg = Registry.LocalMachine.CreateSubKey($"SOFTWARE\\Classes\\{AppID}URL");
    handlerReg.SetValue("", AppName);
    handlerReg.SetValue("FriendlyTypeName", AppName);

    handlerReg.CreateSubKey(string.Format("shell\\open\\command", AppID)).SetValue("", AppOpenUrlCommand);
  }

  public void Unregister()
  {
    Registry.LocalMachine.DeleteSubKeyTree($"SOFTWARE\\{AppID}", false);

    if (Key != null)
    {
      Key.DeleteValue(AppID);
    }

    Registry.LocalMachine.DeleteSubKeyTree($"SOFTWARE\\Classes\\{AppID}URL");
  }
}
