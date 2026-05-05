using Microsoft.JSInterop;

namespace MediaPlayer.Client.Services.Storage;

/// <summary>
/// Thin C# facade over <c>wwwroot/js/indexeddb.js</c>. Owns the imported
/// JS module reference so it is loaded exactly once per session.
/// </summary>
public sealed class IndexedDbInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public IndexedDbInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/indexeddb.js").AsTask());
    }

    public async Task<T[]> GetAllAsync<T>(string storeName, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<T[]>("getAll", cancellationToken, storeName).ConfigureAwait(false);
    }

    public async Task<T?> GetByIdAsync<T>(string storeName, string key, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<T?>("getById", cancellationToken, storeName, key).ConfigureAwait(false);
    }

    public async Task PutAsync<T>(string storeName, string key, T value, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("put", cancellationToken, storeName, key, value).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string storeName, string key, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("remove", cancellationToken, storeName, key).ConfigureAwait(false);
    }

    public async Task ClearStoreAsync(string storeName, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("clearStore", cancellationToken, storeName).ConfigureAwait(false);
    }

    public async Task SaveBlobAsync(string key, byte[] data, string mimeType, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        var base64 = Convert.ToBase64String(data);
        await module.InvokeVoidAsync("saveBlob", cancellationToken, key, base64, mimeType).ConfigureAwait(false);
    }

    public async Task<string?> GetBlobUrlAsync(string key, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<string?>("getBlobUrl", cancellationToken, key).ConfigureAwait(false);
    }

    public async Task DeleteBlobAsync(string key, CancellationToken cancellationToken = default)
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("deleteBlob", cancellationToken, key).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            await module.DisposeAsync().ConfigureAwait(false);
        }
    }
}
