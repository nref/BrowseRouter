using BrowseRouter.Model;

namespace BrowseRouter.Tests.Model.ArgsTests;

public class FormatMethod
{
  [Fact]
  public void UrlTag_IsReplacedWithTheUrl()
  {
    string args = "--open-url {url}";
    Uri uri = new Uri("https://example.com");

    var result = Args.Format(args, uri);

    result.Should().Be("--open-url https://example.com");
  }

  [Fact]
  public void UserinfoTag_IsReplacedWithTheUserInfo()
  {
    string args = "--user-info {userinfo}";
    Uri uri = new Uri("https://user:password@example.com");

    var result = Args.Format(args, uri);

    result.Should().Be("--user-info user:password");
  }

  [Fact]
  public void HostTag_IsReplacedWithTheHost()
  {
    string args = "--host {host}";
    Uri uri = new Uri("https://example.com");

    var result = Args.Format(args, uri);

    result.Should().Be("--host example.com");
  }

  [Fact]
  public void PortTag_IsReplacedWithThePort()
  {
    string args = "--port {port}";
    Uri uri = new Uri("https://example.com:8080");

    var result = Args.Format(args, uri);

    result.Should().Be("--port 8080");
  }

  [Fact]
  public void AuthorityTag_IsReplacedWithTheAuthority()
  {
    string args = "--authority {authority}";
    Uri uri = new Uri("https://user:password@example.com:8080");

    var result = Args.Format(args, uri);

    result.Should().Be("--authority example.com:8080");
  }

  [Fact]
  public void PathTag_IsReplacedWithThePath()
  {
    string args = "--path {path}";
    Uri uri = new Uri("https://example.com/some/path");

    var result = Args.Format(args, uri);

    result.Should().Be("--path /some/path");
  }

  [Fact]
  public void QueryTag_IsReplacedWithTheQuery()
  {
    string args = "--query {query}";
    Uri uri = new Uri("https://example.com?query=1");

    var result = Args.Format(args, uri);

    result.Should().Be("--query ?query=1");
  }

  [Fact]
  public void FragmentTag_IsReplacedWithTheFragment()
  {
    string args = "--fragment {fragment}";
    Uri uri = new Uri("https://example.com#fragment");

    var result = Args.Format(args, uri);

    result.Should().Be("--fragment #fragment");
  }

  [Fact]
  public void SeveralTags_AreAllReplaced()
  {
    string args = "--host {host} --path {path}";
    Uri uri = new Uri("https://example.com/some/path");

    var result = Args.Format(args, uri);

    result.Should().Be("--host example.com --path /some/path");
  }

  [Fact]
  public void NoTags_AppendTheQuotedUrl()
  {
    string args = "--open-url";
    Uri uri = new Uri("https://example.com");

    var result = Args.Format(args, uri);

    result.Should().Be("--open-url \"https://example.com\"");
  }

  [Fact]
  public void EmptyArgs_AreTheQuotedUrlAfterASpace()
  {
    string args = "";
    Uri uri = new Uri("https://example.com");

    var result = Args.Format(args, uri);

    result.Should().Be(" \"https://example.com\"");
  }

  [Fact]
  public void UnknownTag_IsKeptAndTheQuotedUrlAppended()
  {
    string args = "--unknown-tag {unknown}";
    Uri uri = new Uri("https://example.com");

    var result = Args.Format(args, uri);

    result.Should().Be("--unknown-tag {unknown} \"https://example.com\"");
  }

  [Fact]
  public void EncodedQueryInTheUrl_IsNotDecoded()
  {
    string result = Args.Format("", new Uri("https://www.google.com/search?q=C%23+Rocks"));

    result.Should().NotContain("C# Rocks");
  }
}
