using MediaPlayer.Core.Abstractions;

namespace MediaPlayer.Tests.Storage;

/// <summary>Test-side <see cref="IMediaBlobStore"/> backed by a Dictionary.</summary>
internal sealed class InMemoryMediaBlobStore : IMediaBlobStore
{
    private readonly Dictionary<string, (byte[] Bytes, string MimeType)> rows = new();

    public Task SaveAsync(string blobKey, byte[] data, string mimeType, CancellationToken cancellationToken = default)
    {
        rows[blobKey] = (data, mimeType);
        return Task.CompletedTask;
    }

    public Task<string?> GetObjectUrlAsync(string blobKey, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(rows.ContainsKey(blobKey) ? $"blob:test/{blobKey}" : null);

    public Task<byte[]?> GetBytesAsync(string blobKey, CancellationToken cancellationToken = default)
        => Task.FromResult(rows.TryGetValue(blobKey, out var row) ? row.Bytes : null);

    public Task DeleteAsync(string blobKey, CancellationToken cancellationToken = default)
    {
        rows.Remove(blobKey);
        return Task.CompletedTask;
    }
}
