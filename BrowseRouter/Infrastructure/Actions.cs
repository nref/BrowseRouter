namespace BrowseRouter.Infrastructure;

public static class Actions
{
  public static bool TryRun(Action a, bool logOnlyToConsole = false)
  {
    try
    {
      a();
      return true;
    }
    catch (Exception e)
    {
      if (logOnlyToConsole)
      {
        Console.WriteLine(e);
      }
      else
      {
        Log.Write($"{e}");
      }

      return false;
    }
  }
}
