using System.Diagnostics;

Process.Start(new ProcessStartInfo
{
  FileName = "BrowseRouter",
  Arguments = "--unregister",
  UseShellExecute = true
});

