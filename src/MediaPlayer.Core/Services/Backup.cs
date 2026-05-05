using MediaPlayer.Core.Domain;

namespace MediaPlayer.Core.Services;

/// <summary>
/// Snapshot of every entity in the kiosk database, suitable for cross-machine
/// sync. Round-trips through JSON cleanly because every member is either a
/// primitive, a record with init-only properties, or a list thereof.
/// </summary>
public sealed record Backup
{
    public int Version { get; init; } = 1;
    public DateTimeOffset ExportedAt { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<MediaItem> MediaItems { get; init; } = Array.Empty<MediaItem>();
    public IReadOnlyList<Playlist> Playlists { get; init; } = Array.Empty<Playlist>();
    public IReadOnlyList<Location> Locations { get; init; } = Array.Empty<Location>();
    public IReadOnlyList<Kiosk> Kiosks { get; init; } = Array.Empty<Kiosk>();
    public IReadOnlyList<BackupBlob> Blobs { get; init; } = Array.Empty<BackupBlob>();
}

/// <summary>Single media binary, base64-encoded for JSON-safe transport.</summary>
public sealed record BackupBlob
{
    public required string Key { get; init; }
    public required string MimeType { get; init; }
    public required string Base64 { get; init; }
}
