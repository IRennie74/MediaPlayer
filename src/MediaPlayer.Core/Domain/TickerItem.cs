namespace MediaPlayer.Core.Domain;

/// <summary>
/// A single entry in a playlist's bottom-of-screen ticker. Lives inside the
/// owning <see cref="Playlist"/> aggregate (no separate IndexedDB store) so
/// existing repositories carry it for free.
/// </summary>
public sealed record TickerItem : IEntity
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required int OrderIndex { get; init; }

    public TickerItemKind Kind { get; init; } = TickerItemKind.Pill;

    /// <summary>
    /// Optional Blazorise color hint — one of "Primary" / "Info" / "Success"
    /// / "Warning" / "Danger" / "Light". Stored as a string so the persisted
    /// JSON is forward-compatible if the palette grows. Null → default Light.
    /// </summary>
    public string? ColorHint { get; init; }
}
