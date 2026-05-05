namespace MediaPlayer.Core.Domain;

/// <summary>
/// A single piece of media in the library. The same record covers all three
/// kinds — uploaded image/video (<see cref="BlobKey"/> populated) and embedded
/// website (<see cref="Url"/> populated). Per-playlist behavior (duration,
/// transition, etc.) lives on <see cref="PlaylistItem"/>; this record only
/// carries library-level metadata.
/// </summary>
public sealed record MediaItem : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required MediaKind Kind { get; init; }

    /// <summary>Key into the IndexedDB blob store for Image/Video kinds; null for Iframe.</summary>
    public string? BlobKey { get; init; }

    /// <summary>Embedded website URL for Iframe kind; null for Image/Video.</summary>
    public string? Url { get; init; }

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Default playback length when this media is added to a playlist.</summary>
    public int DefaultDurationSeconds { get; init; } = 10;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public long? FileSizeBytes { get; init; }
    public string? MimeType { get; init; }
}
