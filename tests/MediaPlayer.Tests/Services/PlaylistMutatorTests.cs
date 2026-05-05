using FluentAssertions;
using MediaPlayer.Core.Domain;
using MediaPlayer.Core.Services;

namespace MediaPlayer.Tests.Services;

public sealed class PlaylistMutatorTests
{
    private static Playlist NewPlaylist(int itemCount = 0)
    {
        var items = Enumerable.Range(0, itemCount).Select(i => new PlaylistItem
        {
            Id = Guid.NewGuid(),
            MediaItemId = Guid.NewGuid(),
            OrderIndex = i,
        }).ToArray();
        return new Playlist { Id = Guid.NewGuid(), Name = "Test", Items = items };
    }

    [Fact]
    public void AppendItem_AddsAtEndWithCorrectOrderIndex()
    {
        var playlist = NewPlaylist(2);
        var mediaId = Guid.NewGuid();

        var updated = PlaylistMutator.AppendItem(playlist, mediaId);

        updated.Items.Should().HaveCount(3);
        updated.Items[2].MediaItemId.Should().Be(mediaId);
        updated.Items[2].OrderIndex.Should().Be(2);
        updated.UpdatedAt.Should().BeAfter(playlist.UpdatedAt);
    }

    [Fact]
    public void RemoveItem_ReindexesRemainingItems()
    {
        var playlist = NewPlaylist(3);
        var middleId = playlist.Items[1].Id;

        var updated = PlaylistMutator.RemoveItem(playlist, middleId);

        updated.Items.Should().HaveCount(2);
        updated.Items.Select(i => i.OrderIndex).Should().Equal(0, 1);
    }

    [Fact]
    public void RemoveItem_OnMissingId_ReturnsEquivalentPlaylist()
    {
        var playlist = NewPlaylist(2);

        var updated = PlaylistMutator.RemoveItem(playlist, Guid.NewGuid());

        updated.Items.Should().HaveCount(2);
    }

    [Fact]
    public void MoveUp_OnSecondItem_SwapsWithFirst()
    {
        var playlist = NewPlaylist(3);
        var firstId = playlist.Items[0].Id;
        var secondId = playlist.Items[1].Id;

        var updated = PlaylistMutator.MoveUp(playlist, secondId);

        updated.Items[0].Id.Should().Be(secondId);
        updated.Items[1].Id.Should().Be(firstId);
        updated.Items.Select(i => i.OrderIndex).Should().Equal(0, 1, 2);
    }

    [Fact]
    public void MoveUp_OnFirstItem_IsNoOp()
    {
        var playlist = NewPlaylist(3);
        var firstId = playlist.Items[0].Id;

        var updated = PlaylistMutator.MoveUp(playlist, firstId);

        updated.Items.Select(i => i.Id).Should().Equal(playlist.Items.Select(i => i.Id));
    }

    [Fact]
    public void MoveDown_OnLastItem_IsNoOp()
    {
        var playlist = NewPlaylist(3);
        var lastId = playlist.Items[^1].Id;

        var updated = PlaylistMutator.MoveDown(playlist, lastId);

        updated.Items.Select(i => i.Id).Should().Equal(playlist.Items.Select(i => i.Id));
    }

    [Fact]
    public void UpdateItem_ReplacesMatchingIdAndPreservesOrder()
    {
        var playlist = NewPlaylist(3);
        var target = playlist.Items[1];
        var changed = target with { DurationSecondsOverride = 30, Transition = TransitionKind.Fade };

        var updated = PlaylistMutator.UpdateItem(playlist, changed);

        updated.Items[1].DurationSecondsOverride.Should().Be(30);
        updated.Items[1].Transition.Should().Be(TransitionKind.Fade);
        updated.Items.Select(i => i.OrderIndex).Should().Equal(0, 1, 2);
    }
}
