namespace BrowseRouter.Config.Loaders;

internal interface IConfigLoader
{
    Task<Config> LoadAsync();
}
