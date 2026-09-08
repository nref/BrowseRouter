using System.Text.Json;
using BrowseRouter.Config;

namespace BrowseRouter.Tests.Config.FilterPreferenceTests;

public class TryApplyMethod
{
  // Regression guard for issue #100: MS Teams moved SafeLinks to a new domain.
  // The shipped filter must still unwrap the embedded URL.
  [Fact]
  public void ShippedFilters_UnwrapTeamsSafelink()
  {
    List<FilterPreference> filters = LoadShippedFilters();

    string safelink =
      "https://teams.public.onecdn.static.microsoft/evergreen-assets/safelinks/1/atp-safelinks.html?url=https%3A%2F%2Fwww.example.org%2Fpath%2F";

    bool didFilter = FilterPreference.TryApply(filters, safelink, out string output);

    didFilter.Should().BeTrue();
    output.Should().Be("https://www.example.org/path/");
  }

  // The shipped filter must unwrap the embedded URL from a LinkedIn safety redirect.
  [Fact]
  public void ShippedFilters_UnwrapLinkedInSafetyRedirect()
  {
    List<FilterPreference> filters = LoadShippedFilters();

    string safetyLink =
      "https://www.linkedin.com/safety/go/?url=https%3A%2F%2Fexample.com%2F&urlhash=qtTU&isSdui=true&lipi=urn%3Ali%3Apage%3Ad_flagship3_feed";

    bool didFilter = FilterPreference.TryApply(filters, safetyLink, out string output);

    didFilter.Should().BeTrue();
    output.Should().Be("https://example.com/");
  }

  private static List<FilterPreference> LoadShippedFilters()
  {
    string path = Path.Combine(AppContext.BaseDirectory, "filters.json");
    string json = File.ReadAllText(path);

    // filters.json uses // comments and trailing commas, like the app reads it.
    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      AllowTrailingCommas = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
    };

    return JsonSerializer.Deserialize<List<FilterPreference>>(json, options)
      ?? throw new InvalidOperationException("Could not load filters.json");
  }
}
