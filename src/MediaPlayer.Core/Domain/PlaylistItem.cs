namespace MediaPlayer.Core.Domain;

/// <summary>
/// One row of a playlist. Holds a reference to a <see cref="MediaItem"/> plus
/// per-item playback overrides — duration, transition, fit, and type-specific
/// options that mimic a stripped-down video editor (just for individual slides,
/// no timeline scrubbing).
/// </summary>
public sealed record PlaylistItem : IEntity
{
    public required Guid Id { get; init; }
    public required Guid MediaItemId { get; init; }
    public required int OrderIndex { get; init; }

    /// <summary>Overrides <see cref="MediaItem.DefaultDurationSeconds"/> if set.</summary>
    public int? DurationSecondsOverride { get; init; }

    public TransitionKind Transition { get; init; } = TransitionKind.None;
    public FitMode Fit { get; init; } = FitMode.Contain;

    // Video-only options.
    public bool VideoMuted { get; init; } = true;
    public bool VideoLoop { get; init; }
    public int VideoVolumePercent { get; init; } = 100;
    public int VideoStartSeconds { get; init; }

    // Iframe-only options.
    public bool IframeInteractive { get; init; } = true;
    public int IframeZoomPercent { get; init; } = 100;
}
