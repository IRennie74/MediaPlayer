namespace MediaPlayer.Core.Domain;

/// <summary>
/// The visual kind of a <see cref="MediaItem"/>. Drives which slide component
/// renders it (image element, video element, or sandboxed iframe).
/// </summary>
public enum MediaKind
{
    Image = 0,
    Video = 1,
    Iframe = 2,
}
