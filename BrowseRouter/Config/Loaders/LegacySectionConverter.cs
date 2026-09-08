using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrowseRouter.Config.Loaders;

internal class LegacySectionConverter : JsonConverter<List<PatternBrowser>>
{
  public override List<PatternBrowser> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException($"Expected an object, found {reader.TokenType}.");

    List<PatternBrowser> entries = [];

    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
    {
      string pattern = reader.GetString()!;
      reader.Read();

      entries.Add(new PatternBrowser(
        pattern,
        reader.TokenType == JsonTokenType.Null ? null : reader.GetString()));
    }

    return entries;
  }

  public override void Write(Utf8JsonWriter writer, List<PatternBrowser> value, JsonSerializerOptions options)
    => throw new NotSupportedException();
}
