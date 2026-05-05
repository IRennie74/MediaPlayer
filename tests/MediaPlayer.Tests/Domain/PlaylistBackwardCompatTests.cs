using System.Text.Json;
using FluentAssertions;
using MediaPlayer.Core.Domain;
using MediaPlayer.Core.Services;
using MediaPlayer.Tests.Storage;

namespace MediaPlayer.Tests.Domain;

/// <summary>
/// Existing kiosks have playlists in IndexedDB that were serialized before
/// the ticker fields existed. These tests pin the contract: those legacy
/// rows must deserialize cleanly with safe defaults, and a round-trip
/// through <see cref="BackupService"/> must preserve the new fields.
/// </summary>
public sealed class PlaylistBackwardCompatTests
{
    [Fact]
    public void DeserializeLegacyJson_DefaultsTickerEnabledToFalseAndItemsToEmpty()
    {
        // Pre-ticker payload — no TickerEnabled or TickerItems keys.
        var legacyJson = """
            {
              "Id": "00000000-0000-0000-0000-000000000001",
              "Name": "Reception",
              "Items": [],
              "ShuffleEnabled": false,
              "UpdatedAt": "2026-05-05T12:00:00+00:00"
            }
            """;

        var p = JsonSerializer.Deserialize<Playlist>(legacyJson)!;

        p.Name.Should().Be("Reception");
        p.TickerEnabled.Should().BeFalse();
        p.TickerItems.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task BackupRoundTrip_PreservesTickerFields()
    {
        var media = new InMemoryRepository<MediaItem>();
        var playlists = new InMemoryRepository<Playlist>();
        var locations = new InMemoryRepository<Location>();
        var kiosks = new InMemoryRepository<Kiosk>();
        var blobs = new InMemoryMediaBlobStore();

        var original = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = "With ticker",
            TickerEnabled = true,
            TickerItems = new[]
            {
                new TickerItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Now hiring!",
                    OrderIndex = 0,
                    Kind = TickerItemKind.Pill,
                    ColorHint = "Success",
                },
                new TickerItem
                {
                    Id = Guid.NewGuid(),
                    Text = "Remember to wear PPE in the production area.",
                    OrderIndex = 1,
                    Kind = TickerItemKind.Scrolling,
                },
            },
        };
        await playlists.CreateAsync(original);

        var exporter = new BackupService(media, playlists, locations, kiosks, blobs);
        var backup = await exporter.CreateAsync();

        var json = JsonSerializer.Serialize(backup);
        var rehydrated = JsonSerializer.Deserialize<Backup>(json)!;

        var restored = rehydrated.Playlists.Single();
        restored.TickerEnabled.Should().BeTrue();
        restored.TickerItems.Should().HaveCount(2);
        restored.TickerItems[0].Kind.Should().Be(TickerItemKind.Pill);
        restored.TickerItems[0].ColorHint.Should().Be("Success");
        restored.TickerItems[1].Kind.Should().Be(TickerItemKind.Scrolling);
        restored.TickerItems[1].ColorHint.Should().BeNull();
    }
}
