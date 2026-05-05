namespace MediaPlayer.Client.Services.Storage;

/// <summary>
/// Names of the IndexedDB object stores. Mirrored exactly in
/// <c>wwwroot/js/indexeddb.js</c> — keep the two in sync.
/// </summary>
internal static class StoreNames
{
    public const string MediaItems = "mediaItems";
    public const string Playlists = "playlists";
    public const string Locations = "locations";
    public const string Kiosks = "kiosks";
}
