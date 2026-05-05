using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Core.Services;

/// <summary>
/// Builds a <see cref="Backup"/> from the live repositories, and applies a
/// previously-built one back. Pure C#; no JSON or browser concerns leak in
/// here so this is straightforward to unit-test with in-memory repositories.
/// </summary>
public sealed class BackupService
{
    private readonly IRepository<MediaItem> mediaRepo;
    private readonly IRepository<Playlist> playlistRepo;
    private readonly IRepository<Location> locationRepo;
    private readonly IRepository<Kiosk> kioskRepo;
    private readonly IMediaBlobStore blobStore;

    public BackupService(
        IRepository<MediaItem> mediaRepo,
        IRepository<Playlist> playlistRepo,
        IRepository<Location> locationRepo,
        IRepository<Kiosk> kioskRepo,
        IMediaBlobStore blobStore)
    {
        this.mediaRepo = mediaRepo;
        this.playlistRepo = playlistRepo;
        this.locationRepo = locationRepo;
        this.kioskRepo = kioskRepo;
        this.blobStore = blobStore;
    }

    public async Task<Backup> CreateAsync(CancellationToken cancellationToken = default)
    {
        var media = await mediaRepo.FindAllAsync(cancellationToken).ConfigureAwait(false);
        var playlists = await playlistRepo.FindAllAsync(cancellationToken).ConfigureAwait(false);
        var locations = await locationRepo.FindAllAsync(cancellationToken).ConfigureAwait(false);
        var kiosks = await kioskRepo.FindAllAsync(cancellationToken).ConfigureAwait(false);

        var blobs = new List<BackupBlob>();
        foreach (var item in media)
        {
            if (string.IsNullOrEmpty(item.BlobKey)) continue;
            var bytes = await blobStore.GetBytesAsync(item.BlobKey, cancellationToken).ConfigureAwait(false);
            if (bytes is null) continue;
            blobs.Add(new BackupBlob
            {
                Key = item.BlobKey,
                MimeType = item.MimeType ?? "application/octet-stream",
                Base64 = Convert.ToBase64String(bytes),
            });
        }

        return new Backup
        {
            MediaItems = media,
            Playlists = playlists,
            Locations = locations,
            Kiosks = kiosks,
            Blobs = blobs,
        };
    }

    /// <summary>
    /// Upserts every entity from the backup into the live store. Existing
    /// rows with the same Id are overwritten; rows not present in the backup
    /// are left alone (no destructive clear by default — the operator can
    /// always wipe the browser data manually if a clean import is needed).
    /// </summary>
    public async Task RestoreAsync(Backup backup, CancellationToken cancellationToken = default)
    {
        foreach (var b in backup.Blobs)
        {
            var bytes = Convert.FromBase64String(b.Base64);
            await blobStore.SaveAsync(b.Key, bytes, b.MimeType, cancellationToken).ConfigureAwait(false);
        }
        foreach (var l in backup.Locations) await locationRepo.UpdateAsync(l, cancellationToken).ConfigureAwait(false);
        foreach (var k in backup.Kiosks) await kioskRepo.UpdateAsync(k, cancellationToken).ConfigureAwait(false);
        foreach (var m in backup.MediaItems) await mediaRepo.UpdateAsync(m, cancellationToken).ConfigureAwait(false);
        foreach (var p in backup.Playlists) await playlistRepo.UpdateAsync(p, cancellationToken).ConfigureAwait(false);
    }
}
