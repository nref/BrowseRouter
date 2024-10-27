using System.Diagnostics;

Process.Start(new ProcessStartInfo
{
  FileName = "BrowseRouter",
  Arguments = "", // No args means register
  UseShellExecute = true
});

