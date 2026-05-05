namespace MediaPlayer.Core.Domain;

/// <summary>
/// A logical screen at a <see cref="Location"/>. A kiosk is bound to one
/// playlist; the same kiosk identity can be claimed by any browser via the
/// hidden display picker.
/// </summary>
public sealed record Kiosk : IEntity
{
    public required Guid Id { get; init; }
    public required Guid LocationId { get; init; }
    public required string Name { get; init; }
    public Guid? AssignedPlaylistId { get; init; }
}
