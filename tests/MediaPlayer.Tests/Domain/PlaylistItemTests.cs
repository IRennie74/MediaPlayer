using FluentAssertions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Tests.Domain;

public sealed class PlaylistItemTests
{
    [Fact]
    public void Defaults_MatchPlaybackSafeSettings()
    {
        var item = new PlaylistItem
        {
            Id = Guid.NewGuid(),
            MediaItemId = Guid.NewGuid(),
            OrderIndex = 0,
        };

        item.Transition.Should().Be(TransitionKind.None);
        item.Fit.Should().Be(FitMode.Contain);
        item.VideoMuted.Should().BeTrue("muted is required for browser autoplay policies");
        item.VideoLoop.Should().BeFalse();
        item.VideoVolumePercent.Should().Be(100);
        item.IframeInteractive.Should().BeTrue();
        item.IframeZoomPercent.Should().Be(100);
        item.DurationSecondsOverride.Should().BeNull();
    }
}
