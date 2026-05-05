using MediaPlayer.Core.Abstractions;

namespace MediaPlayer.Client.Services.Storage;

/// <summary>
/// IndexedDB-backed implementation of <see cref="IMediaBlobStore"/>.
/// The JS layer holds the bytes as a real <c>Blob</c> so callers can hand
/// the returned <c>blob:</c> URL straight to an &lt;img&gt; or &lt;video&gt;.
/// </summary>
public sealed class IndexedDbBlobStore : IMediaBlobStore
{
    private readonly IndexedDbInterop interop;

    public IndexedDbBlobStore(IndexedDbInterop interop)
    {
        this.interop = interop;
    }

    public Task SaveAsync(string blobKey, byte[] data, string mimeType, CancellationToken cancellationToken = default)
        => interop.SaveBlobAsync(blobKey, data, mimeType, cancellationToken);

    public Task<string?> GetObjectUrlAsync(string blobKey, CancellationToken cancellationToken = default)
        => interop.GetBlobUrlAsync(blobKey, cancellationToken);

    public Task DeleteAsync(string blobKey, CancellationToken cancellationToken = default)
        => interop.DeleteBlobAsync(blobKey, cancellationToken);
}
