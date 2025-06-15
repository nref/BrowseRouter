namespace BrowseRouter.Config;

internal record LogConfig(
    bool Enabled,
    string File
)
{
  public static LogConfig Empty => new(false, string.Empty);
}
