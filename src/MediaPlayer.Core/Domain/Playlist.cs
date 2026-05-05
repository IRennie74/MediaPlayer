namespace MediaPlayer.Core.Domain;

/// <summary>
/// An ordered set of <see cref="PlaylistItem"/>s assigned to one or more kiosks.
/// Items are immutable; reorder by producing a new list with updated
/// <see cref="PlaylistItem.OrderIndex"/> values via a <c>with</c> expression.
/// </summary>
public sealed record Playlist : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public IReadOnlyList<PlaylistItem> Items { get; init; } = Array.Empty<PlaylistItem>();
    public bool ShuffleEnabled { get; init; }
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
}
