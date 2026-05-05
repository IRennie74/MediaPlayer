namespace MediaPlayer.Core.Services;

/// <summary>
/// Pure validation rules for adding to the media library. No I/O — the actual
/// upload (base64 → IndexedDB blob) happens in the Client project.
/// </summary>
public static class MediaImportRules
{
    /// <summary>Above this size we still allow upload but warn the operator.</summary>
    public const long SoftMaxFileBytes = 50L * 1024 * 1024;     // 50 MB

    /// <summary>Hard cap — blocked outright. IndexedDB quotas vary across browsers.</summary>
    public const long HardMaxFileBytes = 200L * 1024 * 1024;    // 200 MB

    public static readonly IReadOnlySet<string> AllowedImageMimes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/jpg", "image/webp", "image/gif",
    };

    public static readonly IReadOnlySet<string> AllowedVideoMimes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4", "video/webm", "video/ogg",
    };

    public static MediaImportResult ValidateUpload(string name, long sizeBytes, string mimeType, bool video)
    {
        if (string.IsNullOrWhiteSpace(name))
            return MediaImportResult.Invalid("Name is required.");

        if (sizeBytes <= 0)
            return MediaImportResult.Invalid("File appears to be empty.");

        if (sizeBytes > HardMaxFileBytes)
            return MediaImportResult.Invalid($"File exceeds the {HardMaxFileBytes / 1024 / 1024} MB hard limit.");

        var allowed = video ? AllowedVideoMimes : AllowedImageMimes;
        if (!allowed.Contains(mimeType))
            return MediaImportResult.Invalid($"Unsupported MIME type '{mimeType}'.");

        var warning = sizeBytes > SoftMaxFileBytes
            ? $"File is larger than {SoftMaxFileBytes / 1024 / 1024} MB and may strain IndexedDB quota."
            : null;
        return MediaImportResult.Ok(warning);
    }

    public static MediaImportResult ValidateIframeUrl(string name, string url)
    {
        if (string.IsNullOrWhiteSpace(name))
            return MediaImportResult.Invalid("Name is required.");

        if (string.IsNullOrWhiteSpace(url))
            return MediaImportResult.Invalid("URL is required.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return MediaImportResult.Invalid("URL is not a valid absolute URI.");

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return MediaImportResult.Invalid("Only http and https URLs are supported.");

        return MediaImportResult.Ok(warning: null);
    }
}
