namespace BrowseRouter.Config.Loaders;

internal interface IConfigLoader
{
    Task<ConfigFile> LoadAsync();
}
