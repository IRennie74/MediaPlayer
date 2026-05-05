using Blazored.LocalStorage;
using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Client.Services.Storage;

/// <summary>
/// Stores the kiosk identity for *this browser only*. LocalStorage is the
/// right tool: tiny payload, must survive refreshes, and must be isolated
/// per machine so two kiosks never share an assignment.
/// </summary>
public sealed class LocalStorageDisplayAssignmentStore : IDisplayAssignmentStore
{
    private const string Key = "mediaplayer.displayAssignment";

    private readonly ILocalStorageService localStorage;

    public LocalStorageDisplayAssignmentStore(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<DisplayAssignment?> GetAsync(CancellationToken cancellationToken = default)
    {
        if (!await localStorage.ContainKeyAsync(Key, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }
        return await localStorage.GetItemAsync<DisplayAssignment>(Key, cancellationToken).ConfigureAwait(false);
    }

    public Task SetAsync(DisplayAssignment assignment, CancellationToken cancellationToken = default)
        => localStorage.SetItemAsync(Key, assignment, cancellationToken).AsTask();

    public Task ClearAsync(CancellationToken cancellationToken = default)
        => localStorage.RemoveItemAsync(Key, cancellationToken).AsTask();
}
