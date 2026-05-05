namespace MediaPlayer.Core.Abstractions;

/// <summary>
/// Storage for the raw bytes behind an uploaded image or video. Kept separate
/// from <c>IRepository&lt;MediaItem&gt;</c> so that large blobs can move
/// through a streaming-friendly path and are never serialized into the JSON
/// metadata blob.
/// </summary>
public interface IMediaBlobStore
{
    Task SaveAsync(string blobKey, byte[] data, string mimeType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a browser-side <c>blob:</c> URL that can be assigned to an
    /// <c>&lt;img&gt;</c>/<c>&lt;video&gt;</c> src. Null when the blob is missing.
    /// </summary>
    Task<string?> GetObjectUrlAsync(string blobKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string blobKey, CancellationToken cancellationToken = default);
}
