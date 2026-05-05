using System.Text.Json;
using FluentAssertions;
using MediaPlayer.Core.Domain;
using MediaPlayer.Core.Services;
using MediaPlayer.Tests.Storage;

namespace MediaPlayer.Tests.Services;

public sealed class BackupServiceTests
{
    private readonly InMemoryRepository<MediaItem> mediaRepo = new();
    private readonly InMemoryRepository<Playlist> playlistRepo = new();
    private readonly InMemoryRepository<Location> locationRepo = new();
    private readonly InMemoryRepository<Kiosk> kioskRepo = new();
    private readonly InMemoryMediaBlobStore blobs = new();

    private BackupService Sut => new(mediaRepo, playlistRepo, locationRepo, kioskRepo, blobs);

    [Fact]
    public async Task Create_OnEmptyDatabase_ReturnsEmptyButValidBackup()
    {
        var backup = await Sut.CreateAsync();

        backup.Version.Should().Be(1);
        backup.MediaItems.Should().BeEmpty();
        backup.Playlists.Should().BeEmpty();
        backup.Locations.Should().BeEmpty();
        backup.Kiosks.Should().BeEmpty();
        backup.Blobs.Should().BeEmpty();
    }

    [Fact]
    public async Task RoundTrip_RestoresAllEntitiesAndBlobBytes()
    {
        // Arrange — original DB
        var blobKey = Guid.NewGuid().ToString();
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };
        await blobs.SaveAsync(blobKey, imageBytes, "image/png");

        var image = new MediaItem
        {
            Id = Guid.NewGuid(),
            Name = "Logo",
            Kind = MediaKind.Image,
            BlobKey = blobKey,
            MimeType = "image/png",
            FileSizeBytes = imageBytes.Length,
        };
        var iframe = new MediaItem
        {
            Id = Guid.NewGuid(),
            Name = "Robotape",
            Kind = MediaKind.Iframe,
            Url = "https://robotape.com",
        };
        await mediaRepo.CreateAsync(image);
        await mediaRepo.CreateAsync(iframe);

        var location = new Location { Id = Guid.NewGuid(), Name = "HQ" };
        await locationRepo.CreateAsync(location);
        var kiosk = new Kiosk { Id = Guid.NewGuid(), Name = "Lobby 1", LocationId = location.Id };
        await kioskRepo.CreateAsync(kiosk);

        var playlist = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = "Reception",
            Items = new[]
            {
                new PlaylistItem { Id = Guid.NewGuid(), MediaItemId = image.Id, OrderIndex = 0 },
                new PlaylistItem { Id = Guid.NewGuid(), MediaItemId = iframe.Id, OrderIndex = 1 },
            },
        };
        await playlistRepo.CreateAsync(playlist);

        // Act — export to JSON, then import into a brand-new SUT.
        var exported = await Sut.CreateAsync();
        var json = JsonSerializer.Serialize(exported);
        var rehydrated = JsonSerializer.Deserialize<Backup>(json);

        var freshMedia = new InMemoryRepository<MediaItem>();
        var freshPlaylists = new InMemoryRepository<Playlist>();
        var freshLocations = new InMemoryRepository<Location>();
        var freshKiosks = new InMemoryRepository<Kiosk>();
        var freshBlobs = new InMemoryMediaBlobStore();
        var importer = new BackupService(freshMedia, freshPlaylists, freshLocations, freshKiosks, freshBlobs);

        await importer.RestoreAsync(rehydrated!);

        // Assert
        (await freshMedia.FindAllAsync()).Should().HaveCount(2);
        (await freshPlaylists.FindAllAsync()).Should().ContainSingle()
            .Which.Items.Should().HaveCount(2);
        (await freshLocations.FindAllAsync()).Should().ContainSingle();
        (await freshKiosks.FindAllAsync()).Should().ContainSingle();
        (await freshBlobs.GetBytesAsync(blobKey)).Should().Equal(imageBytes);
    }
}
