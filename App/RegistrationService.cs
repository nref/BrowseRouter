using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace BrowseRouter;

public interface IRegistrationService
{
  void Register();
  void Unregister();
}

public static class RegistrationServiceFactory
{
  public static IRegistrationService Create()
  {
    if (System.OperatingSystem.IsWindows())
        return new RegistryRegistrationService();
    else if (System.OperatingSystem.IsLinux())
        return new FreedesktopRegistrationService();
    else
        throw new NotImplementedException();
  }
}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class RegistryRegistrationService : IRegistrationService
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

[System.Runtime.Versioning.SupportedOSPlatform("linux")]
public class FreedesktopRegistrationService : IRegistrationService
{
  [DllImport("libc")]
  private static extern uint geteuid();

  // Returns true if the current process has effective root permissions (incl. sudo).
  private static bool IsRoot()
  {
    return geteuid() == 0;
  }

  // Returns the name of the applications directory that contains .desktop files.
  // If the current process has root permissions the global directory is returned, otherwise the
  // local one.
  // Typically these are /usr/share/applications (global) or ~/.local/share/applications (local).
  // See: https://developers.redhat.com/blog/2018/11/07/dotnet-special-folder-api-linux
  private static string GetApplicationsPath()
  {
    var applicationDataFolder = IsRoot()
                              ? Environment.SpecialFolder.CommonApplicationData
                              : Environment.SpecialFolder.LocalApplicationData;
    return Path.Combine(Environment.GetFolderPath(applicationDataFolder), "applications");
  }

  private static string GetDesktopFilePath()
  {
    return Path.Combine(GetApplicationsPath(), App.FriendlyName + ".desktop");
  }

  // Create a .desktop file in the applications directory which makes the app show up in the start
  // menu and as a candidate for the default web browser setting.
  private void CreateDesktopFile()
  {
    var desktopFilePath = GetDesktopFilePath();

    // Ensure that the applications directory exists.
    // On a fresh installation this may not be the case.
    {
      var appPath = GetApplicationsPath();
      try {
        // This is a no-op if the directory already exists
        Directory.CreateDirectory(GetApplicationsPath());
      }
      catch (Exception e) {
        Log.Write($"Unable to create applications directory '{appPath}': {e.Message}");
      }
    }

    // Create the .desktop file
    Log.Write($"Creating desktop file: {desktopFilePath}");
    var contents = $@"[Desktop Entry]
Name={App.FriendlyName}
GenericName=Web Browser Selector
Comment=Choose what browser to use for specific links
Type=Application
Categories=Network;
Exec={App.ExePath} %u
MimeType=text/html;text/xml;application/xhtml_xml;x-scheme-handler/http;x-scheme-handler/https;
StartupNotify=false
";
    File.WriteAllText(desktopFilePath, contents);
  }

  // Try to update the MIME cache (/usr/share/applications/mimeinfo.cache).
  // Some systems have update-desktop-database installed (on Debian it's in the desktop-file-utils
  // package) but not every desktop manager needs it. Plasma seems to be fine without it, GNOME
  // requires it. Those that need it will likely have installed it in using dependencies.
  private void UpdateMimeCache()
  {
    Log.Write("Updating MIME cache");
    var command = "update-desktop-database";
    try {
      System.Diagnostics.Process.Start(command);
    }
    catch (Exception e)
    {
      Log.Write($"Unable to run '{command}' to update MIME cache (this is probably harmless): {e.Message}");
    }
  }

  // Delete a .desktop file previously created by CreateDesktopFile().
  private void DeleteDesktopFile()
  {
    var desktopFilePath = GetDesktopFilePath();
    if (File.Exists(desktopFilePath)) {
      Log.Write($"Deleting desktop file: {desktopFilePath}");
      try {
        File.Delete(desktopFilePath);
      }
      catch (Exception e)
      {
        Log.Write($"Unable to delete desktop file '{desktopFilePath}': {e.Message}");
      }
    }
    else {
      Log.Write($"Desktop file '{desktopFilePath}' not found.");
    }
  }

  public void Register()
  {
    CreateDesktopFile();
    UpdateMimeCache();
  }

  public void Unregister()
  {
    DeleteDesktopFile();
    UpdateMimeCache();
  }
}
