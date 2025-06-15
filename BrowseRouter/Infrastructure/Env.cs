namespace BrowseRouter.Infrastructure;

public static class Env
{
  public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
}
