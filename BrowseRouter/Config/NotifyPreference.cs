namespace BrowseRouter.Config;

public class NotifyPreference
{
  public const bool IsEnabledDefaultValue = true;
  public const bool IsSilentDefaultValue = true;
  public bool IsEnabled { get; set; }
  public bool IsSilent { get; set; }
}
