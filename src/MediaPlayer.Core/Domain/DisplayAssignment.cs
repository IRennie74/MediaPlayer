namespace MediaPlayer.Core.Domain;

/// <summary>
/// Per-browser record of which <see cref="Kiosk"/> this physical screen has
/// been claimed as. Lives in LocalStorage (not IndexedDB) because it is small
/// and naturally per-machine — every browser must be able to be its own kiosk.
/// </summary>
public sealed record DisplayAssignment
{
    public required Guid KioskId { get; init; }
    public required Guid LocationId { get; init; }
    public DateTimeOffset AssignedAt { get; init; } = DateTimeOffset.UtcNow;
}
