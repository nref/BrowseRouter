namespace BrowseRouter.Config;

internal record NotifyConfig(
    bool Enabled = NotifyPreference.IsEnabledDefaultValue,
    bool Silent = NotifyPreference.IsSilentDefaultValue 
)
{
  public static NotifyConfig Empty => new(false, true);
}
