using System.Text.Json;
using BrowseRouter.Config;

namespace BrowseRouter.Tests.Config;

public class FilterPreferenceTests
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
