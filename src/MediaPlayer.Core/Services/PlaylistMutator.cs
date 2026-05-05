using MediaPlayer.Core.Domain;

namespace MediaPlayer.Core.Services;

/// <summary>
/// Pure helpers that produce a new <see cref="Playlist"/> for each operation.
/// Keeps the editor UI free of list-fiddling logic and makes each transform
/// independently unit-testable.
/// </summary>
public static class PlaylistMutator
{
    public static Playlist AppendItem(Playlist playlist, Guid mediaItemId)
    {
        var newItem = new PlaylistItem
        {
            Id = Guid.NewGuid(),
            MediaItemId = mediaItemId,
            OrderIndex = playlist.Items.Count,
        };
        var items = playlist.Items.Concat(new[] { newItem }).ToArray();
        return playlist with { Items = items, UpdatedAt = DateTimeOffset.UtcNow };
    }

    public static Playlist RemoveItem(Playlist playlist, Guid itemId)
    {
        var items = playlist.Items.Where(i => i.Id != itemId);
        return playlist with { Items = Reindex(items), UpdatedAt = DateTimeOffset.UtcNow };
    }

    public static Playlist UpdateItem(Playlist playlist, PlaylistItem updated)
    {
        var items = playlist.Items.Select(i => i.Id == updated.Id ? updated : i).ToArray();
        return playlist with { Items = items, UpdatedAt = DateTimeOffset.UtcNow };
    }

    public static Playlist MoveUp(Playlist playlist, Guid itemId)
    {
        var ordered = playlist.Items.OrderBy(i => i.OrderIndex).ToList();
        var index = ordered.FindIndex(i => i.Id == itemId);
        if (index <= 0) return playlist;
        (ordered[index - 1], ordered[index]) = (ordered[index], ordered[index - 1]);
        return playlist with { Items = Reindex(ordered), UpdatedAt = DateTimeOffset.UtcNow };
    }

    public static Playlist MoveDown(Playlist playlist, Guid itemId)
    {
        var ordered = playlist.Items.OrderBy(i => i.OrderIndex).ToList();
        var index = ordered.FindIndex(i => i.Id == itemId);
        if (index < 0 || index >= ordered.Count - 1) return playlist;
        (ordered[index + 1], ordered[index]) = (ordered[index], ordered[index + 1]);
        return playlist with { Items = Reindex(ordered), UpdatedAt = DateTimeOffset.UtcNow };
    }

    private static IReadOnlyList<PlaylistItem> Reindex(IEnumerable<PlaylistItem> items)
        => items.Select((item, idx) => item with { OrderIndex = idx }).ToArray();
}
