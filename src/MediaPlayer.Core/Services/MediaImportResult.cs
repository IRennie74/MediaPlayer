namespace MediaPlayer.Core.Services;

/// <summary>
/// Outcome of running <see cref="MediaImportRules"/> against a candidate
/// upload or URL. Carries an optional non-blocking warning (e.g. "file is
/// large") so callers can display it without blocking the action.
/// </summary>
public sealed record MediaImportResult
{
    public bool IsValid { get; init; }
    public string? Error { get; init; }
    public string? Warning { get; init; }

    public static MediaImportResult Ok(string? warning = null) =>
        new() { IsValid = true, Warning = warning };

    public static MediaImportResult Invalid(string error) =>
        new() { IsValid = false, Error = error };
}
