namespace MediaPlayer.Core.Domain;

/// <summary>
/// How one slide gives way to the next during playlist rotation.
/// Kept intentionally short — extra transitions can be added without
/// breaking persisted data because the underlying type is an int.
/// </summary>
public enum TransitionKind
{
    None = 0,
    Fade = 1,
}
