using FluentAssertions;
using MediaPlayer.Core.Services;

namespace MediaPlayer.Tests.Services;

public sealed class MediaImportRulesTests
{
    // ---- Uploads ----

    [Fact]
    public void ValidateUpload_AcceptsTypicalJpeg()
    {
        var result = MediaImportRules.ValidateUpload("photo.jpg", 1_000_000, "image/jpeg", video: false);

        result.IsValid.Should().BeTrue();
        result.Warning.Should().BeNull();
    }

    [Fact]
    public void ValidateUpload_WarnsOnLargeButValidFile()
    {
        var size = MediaImportRules.SoftMaxFileBytes + 1;
        var result = MediaImportRules.ValidateUpload("big.mp4", size, "video/mp4", video: true);

        result.IsValid.Should().BeTrue();
        result.Warning.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ValidateUpload_RejectsOversizedFile()
    {
        var size = MediaImportRules.HardMaxFileBytes + 1;
        var result = MediaImportRules.ValidateUpload("huge.mp4", size, "video/mp4", video: true);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("hard limit");
    }

    [Theory]
    [InlineData("application/octet-stream")]
    [InlineData("text/html")]
    [InlineData("image/svg+xml")]   // not in our allowlist
    public void ValidateUpload_RejectsDisallowedMime(string mime)
    {
        var result = MediaImportRules.ValidateUpload("x", 1024, mime, video: false);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("Unsupported");
    }

    [Fact]
    public void ValidateUpload_RejectsZeroBytes()
    {
        var result = MediaImportRules.ValidateUpload("x", 0, "image/png", video: false);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("empty");
    }

    [Fact]
    public void ValidateUpload_RejectsBlankName()
    {
        var result = MediaImportRules.ValidateUpload("   ", 1024, "image/png", video: false);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("Name");
    }

    // ---- Iframe URLs ----

    [Theory]
    [InlineData("https://robotape.com")]
    [InlineData("http://localhost:5000/dashboard")]
    [InlineData("https://roboclip.com/page?x=1")]
    public void ValidateIframeUrl_AcceptsAbsoluteHttpAndHttps(string url)
    {
        var result = MediaImportRules.ValidateIframeUrl("Test", url);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("/relative/path")]
    [InlineData("ftp://example.com")]
    [InlineData("javascript:alert(1)")]
    public void ValidateIframeUrl_RejectsInvalidOrNonHttpScheme(string url)
    {
        var result = MediaImportRules.ValidateIframeUrl("Test", url);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateIframeUrl_RequiresName()
    {
        var result = MediaImportRules.ValidateIframeUrl("", "https://example.com");

        result.IsValid.Should().BeFalse();
        result.Error.Should().Contain("Name");
    }
}
