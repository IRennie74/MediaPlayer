using Blazored.LocalStorage;
using MediaPlayer.Core.Abstractions;

namespace MediaPlayer.Client.Services;

/// <summary>
/// LocalStorage-backed <see cref="IAuthFlagStore"/>. The whole "session" is a
/// single boolean; refreshing the page should not log the operator out.
/// </summary>
public sealed class LocalStorageAuthFlagStore : IAuthFlagStore
{
    private const string Key = "mediaplayer.isAuthed";

    private readonly ILocalStorageService localStorage;

    public LocalStorageAuthFlagStore(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<bool> GetAsync(CancellationToken cancellationToken = default)
    {
        if (!await localStorage.ContainKeyAsync(Key, cancellationToken).ConfigureAwait(false))
        {
            return false;
        }
        return await localStorage.GetItemAsync<bool>(Key, cancellationToken).ConfigureAwait(false);
    }

    public Task SetAsync(bool value, CancellationToken cancellationToken = default)
        => localStorage.SetItemAsync(Key, value, cancellationToken).AsTask();

    public Task ClearAsync(CancellationToken cancellationToken = default)
        => localStorage.RemoveItemAsync(Key, cancellationToken).AsTask();
}
