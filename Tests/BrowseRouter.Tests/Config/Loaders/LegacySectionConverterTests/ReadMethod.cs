using System.Text.Json;
using BrowseRouter.Config.Loaders;

namespace BrowseRouter.Tests.Config.Loaders.LegacySectionConverterTests;

public class ReadMethod
{
  [Fact]
  public void FourKeyObject_ReturnsEntriesInDocumentOrderNotSortedOrder()
  {
    ConfigFile file = Deserialize("""{ "urls": { "z": "1", "a": "2", "m": "3", "b": "4" } }""");

    file.Urls!.Select(e => e.Pattern).Should().Equal("z", "a", "m", "b");
  }

  [Fact]
  public void RepeatedPattern_KeepsBothEntries()
  {
    ConfigFile file = Deserialize("""{ "urls": { "*.test": "chrome" }, "sources": { "*Slack*": "ff", "*Teams*": "edge" } }""");

    file.Sources!.Select(e => e.Browser).Should().Equal("ff", "edge");
    file.Urls!.Single().Browser.Should().Be("chrome");
  }

  [Fact]
  public void NullValue_YieldsANullBrowser()
  {
    ConfigFile file = Deserialize("""{ "urls": { "*.test": null } }""");

    file.Urls!.Single().Browser.Should().BeNull();
  }

  [Fact]
  public void EmptyObject_YieldsAnEmptyList()
  {
    ConfigFile file = Deserialize("""{ "urls": { } }""");

    file.Urls.Should().BeEmpty();
  }

  private static ConfigFile Deserialize(string json)
    => JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ConfigFile)!;
}
