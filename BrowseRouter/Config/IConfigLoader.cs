namespace BrowseRouter.Config;

internal interface IConfigLoader
{
    Task<Config> LoadAsync();
}
