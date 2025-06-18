using System.Text.Json;

namespace BrowseRouter.Config.Loaders;

internal class JsonConfigLoader(string path) : IConfigLoader
{
  public async Task<Config> LoadAsync()
    => JsonSerializer.Deserialize(await File.ReadAllTextAsync(path), SourceGenerationContext.Default.Config) 
        ?? Config.Empty;
}
