using FluentAssertions;
using MediaPlayer.Core.Domain;
using MediaPlayer.Core.Services;

namespace MediaPlayer.Tests.Services;

public sealed class PlaylistMutatorTickerTests
{
    private static Playlist NewPlaylist(int tickerCount = 0)
    {
        var items = Enumerable.Range(0, tickerCount).Select(i => new TickerItem
        {
            Id = Guid.NewGuid(),
            Text = $"Item {i}",
            OrderIndex = i,
        }).ToArray();
        return new Playlist
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            TickerItems = items,
        };
    }

    [Fact]
    public void AppendTickerItem_AssignsSequentialOrderIndex()
    {
        var p = NewPlaylist(2);

        var updated = PlaylistMutator.AppendTickerItem(p, new TickerItem
        {
            Id = Guid.NewGuid(),
            Text = "New",
            OrderIndex = -1,
            Kind = TickerItemKind.Scrolling,
        });

        updated.TickerItems.Should().HaveCount(3);
        updated.TickerItems[2].OrderIndex.Should().Be(2);
        updated.TickerItems[2].Text.Should().Be("New");
        updated.TickerItems[2].Kind.Should().Be(TickerItemKind.Scrolling);
    }

    [Fact]
    public void AppendTickerItem_BumpsUpdatedAt()
    {
        var p = NewPlaylist();
        var beforeUpdated = p.UpdatedAt;

        var updated = PlaylistMutator.AppendTickerItem(p, new TickerItem
        {
            Id = Guid.NewGuid(),
            Text = "X",
            OrderIndex = 0,
        });

        updated.UpdatedAt.Should().BeAfter(beforeUpdated);
    }

    [Fact]
    public void RemoveTickerItem_ReindexesRemaining()
    {
        var p = NewPlaylist(3);
        var middleId = p.TickerItems[1].Id;

        var updated = PlaylistMutator.RemoveTickerItem(p, middleId);

        updated.TickerItems.Should().HaveCount(2);
        updated.TickerItems.Select(i => i.OrderIndex).Should().Equal(0, 1);
    }

    [Fact]
    public void RemoveTickerItem_OnMissingId_ReturnsEquivalentList()
    {
        var p = NewPlaylist(2);

        var updated = PlaylistMutator.RemoveTickerItem(p, Guid.NewGuid());

        updated.TickerItems.Should().HaveCount(2);
    }

    [Fact]
    public void UpdateTickerItem_ReplacesMatchingIdAndPreservesOrder()
    {
        var p = NewPlaylist(3);
        var target = p.TickerItems[1];
        var changed = target with { Text = "Edited", Kind = TickerItemKind.Scrolling, ColorHint = "Warning" };

        var updated = PlaylistMutator.UpdateTickerItem(p, changed);

        updated.TickerItems[1].Text.Should().Be("Edited");
        updated.TickerItems[1].Kind.Should().Be(TickerItemKind.Scrolling);
        updated.TickerItems[1].ColorHint.Should().Be("Warning");
        updated.TickerItems.Select(i => i.OrderIndex).Should().Equal(0, 1, 2);
    }

    [Fact]
    public void MoveTickerUp_OnFirst_IsNoOp()
    {
        var p = NewPlaylist(3);
        var firstId = p.TickerItems[0].Id;

        var updated = PlaylistMutator.MoveTickerUp(p, firstId);

        updated.TickerItems.Select(i => i.Id).Should().Equal(p.TickerItems.Select(i => i.Id));
    }

    [Fact]
    public void MoveTickerUp_OnSecond_SwapsWithFirst()
    {
        var p = NewPlaylist(3);
        var first = p.TickerItems[0].Id;
        var second = p.TickerItems[1].Id;

        var updated = PlaylistMutator.MoveTickerUp(p, second);

        updated.TickerItems[0].Id.Should().Be(second);
        updated.TickerItems[1].Id.Should().Be(first);
        updated.TickerItems.Select(i => i.OrderIndex).Should().Equal(0, 1, 2);
    }

    [Fact]
    public void MoveTickerDown_OnLast_IsNoOp()
    {
        var p = NewPlaylist(3);
        var lastId = p.TickerItems[^1].Id;

        var updated = PlaylistMutator.MoveTickerDown(p, lastId);

        updated.TickerItems.Select(i => i.Id).Should().Equal(p.TickerItems.Select(i => i.Id));
    }

    [Fact]
    public void MoveTickerDown_SwapsWithSuccessor()
    {
        var p = NewPlaylist(3);
        var first = p.TickerItems[0].Id;
        var second = p.TickerItems[1].Id;

        var updated = PlaylistMutator.MoveTickerDown(p, first);

        updated.TickerItems[0].Id.Should().Be(second);
        updated.TickerItems[1].Id.Should().Be(first);
    }
}
