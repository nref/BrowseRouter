namespace BrowseRouter;

public static class Actions
{
  public static bool TryRun(Action a)
  {
    try
    {
      a();
      return true;
    }
    catch (Exception e)
    {
      Log.Write($"{e}");
      return false;
    }
  }
}
