namespace BrowseRouter
{
  public static class Env
  {
    public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
  }
}
