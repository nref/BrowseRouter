using System.Reflection;

namespace BrowserSelector;

public static class App
{
  public static string ExePath => Assembly.GetExecutingAssembly().Location
    .Replace(".dll", ".exe"); // .NET Core returns BrowserSettings.dll but we need BrowserSettings.exe
}
