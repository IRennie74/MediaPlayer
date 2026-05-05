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

    /// <summary>Whether the bottom ticker bar shows when this playlist is on a kiosk.</summary>
    public bool TickerEnabled { get; init; }

    /// <summary>Per-playlist ticker entries. Defaults to empty for legacy data.</summary>
    public IReadOnlyList<TickerItem> TickerItems { get; init; } = Array.Empty<TickerItem>();

    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
}
