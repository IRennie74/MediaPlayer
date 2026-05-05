using FluentAssertions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Tests.Domain;

public sealed class DisplayAssignmentTests
{
    [Fact]
    public void NewAssignment_DefaultsAssignedAtToNow()
    {
        var before = DateTimeOffset.UtcNow;
        var assignment = new DisplayAssignment
        {
            KioskId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
        };
        var after = DateTimeOffset.UtcNow;

        assignment.AssignedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void With_PreservesUnchangedFields()
    {
        var original = new DisplayAssignment
        {
            KioskId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
        };

        var moved = original with { KioskId = Guid.NewGuid() };

        moved.LocationId.Should().Be(original.LocationId);
        moved.AssignedAt.Should().Be(original.AssignedAt);
        moved.KioskId.Should().NotBe(original.KioskId);
    }
}
