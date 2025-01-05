namespace BrowserRouter.Tests.ArgsTests;

public class FormatMethod
{
    [Fact]
    public void FormatsUrlTag()
    {
        // Arrange
        string args = "--open-url {url}";
        Uri uri = new Uri("https://example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--open-url https://example.com");
    }

    [Fact]
    public void FormatsUserinfoTag()
    {
        // Arrange
        string args = "--user-info {userinfo}";
        Uri uri = new Uri("https://user:password@example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--user-info user:password");
    }

    [Fact]
    public void FormatsHostTag()
    {
        // Arrange
        string args = "--host {host}";
        Uri uri = new Uri("https://example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--host example.com");
    }

    [Fact]
    public void FormatsPortTag()
    {
        // Arrange
        string args = "--port {port}";
        Uri uri = new Uri("https://example.com:8080");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--port 8080");
    }

    [Fact]
    public void FormatsAuthorityTag()
    {
        // Arrange
        string args = "--authority {authority}";
        Uri uri = new Uri("https://user:password@example.com:8080");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--authority example.com:8080");
    }

    [Fact]
    public void FormatsPathTag()
    {
        // Arrange
        string args = "--path {path}";
        Uri uri = new Uri("https://example.com/some/path");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--path /some/path");
    }

    [Fact]
    public void FormatsQueryTag()
    {
        // Arrange
        string args = "--query {query}";
        Uri uri = new Uri("https://example.com?query=1");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--query ?query=1");
    }

    [Fact]
    public void FormatsFragmentTag()
    {
        // Arrange
        string args = "--fragment {fragment}";
        Uri uri = new Uri("https://example.com#fragment");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--fragment #fragment");
    }

    [Fact]
    public void FormatsMultipleTags()
    {
        // Arrange
        string args = "--host {host} --path {path}";
        Uri uri = new Uri("https://example.com/some/path");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--host example.com --path /some/path");
    }

    [Fact]
    public void HandlesNoTags()
    {
        // Arrange
        string args = "--open-url";
        Uri uri = new Uri("https://example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--open-url \"https://example.com\"");
    }

    [Fact]
    public void HandlesNoArgs()
    {
        // Arrange
        string args = "";
        Uri uri = new Uri("https://example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be(" \"https://example.com\""); // Mind the leading space
    }

    [Fact]
    public void IgnoresUnknownTags()
    {
        // Arrange
        string args = "--unknown-tag {unknown}";
        Uri uri = new Uri("https://example.com");

        // Act
        var result = Args.Format(args, uri);

        // Assert
        result.Should().Be("--unknown-tag {unknown} \"https://example.com\"");
    }
}
