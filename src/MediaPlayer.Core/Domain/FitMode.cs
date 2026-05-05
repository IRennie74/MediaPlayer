namespace MediaPlayer.Core.Domain;

/// <summary>
/// How a slide's content fits into the screen. Maps directly to the
/// CSS <c>object-fit</c> values used by the slide components.
/// </summary>
public enum FitMode
{
    /// <summary>Whole content visible, letterbox if aspect mismatches.</summary>
    Contain = 0,
    /// <summary>Fills the screen; crops to preserve aspect.</summary>
    Cover = 1,
    /// <summary>Stretches to fill the screen; ignores aspect.</summary>
    Fill = 2,
}
