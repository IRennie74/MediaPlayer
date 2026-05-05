namespace MediaPlayer.Core.Domain;

/// <summary>A physical site (e.g. "Barrie HQ") that owns one or more kiosks.</summary>
public sealed record Location : IEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
