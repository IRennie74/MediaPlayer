using FluentAssertions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Tests.Domain;

public sealed class MediaItemTests
{
    [Fact]
    public void Defaults_AreConservative()
    {
        var item = new MediaItem
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Kind = MediaKind.Image,
        };

        item.DefaultDurationSeconds.Should().Be(10);
        item.Tags.Should().BeEmpty();
        item.BlobKey.Should().BeNull();
        item.Url.Should().BeNull();
    }

    [Fact]
    public void With_ProducesNewInstance_LeavingOriginalUnchanged()
    {
        var original = new MediaItem
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Kind = MediaKind.Iframe,
            Url = "https://example.com",
        };

        var renamed = original with { Name = "Renamed" };

        renamed.Should().NotBeSameAs(original);
        renamed.Name.Should().Be("Renamed");
        original.Name.Should().Be("Original");
    }
}
