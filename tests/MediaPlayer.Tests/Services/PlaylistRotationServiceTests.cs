using FluentAssertions;
using MediaPlayer.Core.Services;

namespace MediaPlayer.Tests.Services;

public sealed class PlaylistRotationServiceTests
{
    private readonly PlaylistRotationService sut = new();

    [Fact]
    public void Next_OnEmpty_ReturnsNull()
    {
        sut.Next(0, null, shuffle: false).Should().BeNull();
    }

    [Fact]
    public void Next_OnSingleItem_AlwaysReturnsZero()
    {
        sut.Next(1, null, shuffle: false).Should().Be(0);
        sut.Next(1, 0, shuffle: false).Should().Be(0);
        sut.Next(1, 0, shuffle: true).Should().Be(0);
    }

    [Fact]
    public void Next_InOrder_AdvancesAndWrapsAround()
    {
        sut.Next(3, null, shuffle: false).Should().Be(0);
        sut.Next(3, 0, shuffle: false).Should().Be(1);
        sut.Next(3, 1, shuffle: false).Should().Be(2);
        sut.Next(3, 2, shuffle: false).Should().Be(0); // wraps
    }

    [Fact]
    public void Next_Shuffle_NeverPicksCurrentIndex()
    {
        // Seeded RNG keeps this deterministic: with seed 42 the picks are
        // stable across runs, but more importantly we verify the invariant
        // (next != current) over a wide range of pulls.
        var rng = new Random(42);
        var current = 1;
        for (var i = 0; i < 1000; i++)
        {
            var next = sut.Next(5, current, shuffle: true, rng);
            next.Should().NotBe(current);
            next.Should().BeInRange(0, 4);
            current = next!.Value;
        }
    }
}
