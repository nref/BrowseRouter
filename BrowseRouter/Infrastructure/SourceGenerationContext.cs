using System.Text.Json.Serialization;
using BrowseRouter.Config;

namespace BrowseRouter;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(FilterPreference))]
[JsonSerializable(typeof(List<FilterPreference>), TypeInfoPropertyName = "FilterPreferenceList")]
[JsonSerializable(typeof(Config.Config))]
[JsonSerializable(typeof(NotifyConfig))]
[JsonSerializable(typeof(LogConfig))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}