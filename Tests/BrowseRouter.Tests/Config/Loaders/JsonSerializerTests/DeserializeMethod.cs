using System.Text.Json;
using BrowseRouter.Config.Loaders;

namespace BrowseRouter.Tests.Config.Loaders.JsonSerializerTests;

public class DeserializeMethod
{
  [Fact]
  public void AbsentSections_AreNull()
  {
    ConfigFile file = Deserialize("""{ "filtersFile": "filters.json" }""");

    file.Notify.Should().BeNull();
    file.Log.Should().BeNull();
    file.Browsers.Should().BeNull();
    file.Rules.Should().BeNull();
    file.Sources.Should().BeNull();
    file.Urls.Should().BeNull();
    file.FiltersFile.Should().Be("filters.json");
  }

  [Fact]
  public void AbsentBrowserOnARule_IsNull()
  {
    ConfigFile file = Deserialize("""{ "rules": [ { "url": "*" } ] }""");

    file.Rules!.Single().Browser.Should().BeNull();
    file.Rules!.Single().Url.Should().Be("*");
    file.Rules!.Single().Source.Should().BeNull();
  }

  [Fact]
  public void EmptyRulesArray_IsAnEmptyListNotNull()
  {
    ConfigFile file = Deserialize("""{ "rules": [ ] }""");

    file.Rules.Should().BeEmpty();
  }

  [Fact]
  public void RulesInFileOrder_AreKeptInThatOrder()
  {
    ConfigFile file = Deserialize("""
      {
        "rules": [
          { "url": "*.youtube.com", "browser": "chrome" },
          { "source": "*Discord -> Discord", "browser": "ff-jerrk" }
        ]
      }
      """);

    file.Rules!.Select(r => r.Browser).Should().Equal("chrome", "ff-jerrk");
  }

  [Fact]
  public void CommentsAndTrailingCommas_AreAccepted()
  {
    ConfigFile file = Deserialize("""
      {
        // a comment
        "rules": [ { "url": "*", "browser": "chrome" }, ],
      }
      """);

    file.Rules!.Single().Browser.Should().Be("chrome");
  }

  private static ConfigFile Deserialize(string json)
    => JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ConfigFile)!;
}
