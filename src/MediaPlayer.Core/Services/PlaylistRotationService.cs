namespace MediaPlayer.Core.Services;

/// <summary>
/// Pure rotation logic for the kiosk slideshow. Given the playlist length
/// and the currently-shown index, returns the next index to display.
/// Has no Blazor / JS / timer dependencies so it can be unit-tested directly.
/// </summary>
public sealed class PlaylistRotationService
{
    /// <summary>Returns the next index to show, or null if the playlist is empty.</summary>
    /// <param name="count">Total number of items in the playlist.</param>
    /// <param name="currentIndex">The currently-shown index, or null if nothing is showing yet.</param>
    /// <param name="shuffle">When true, picks a random index different from the current one.</param>
    /// <param name="random">Optional RNG for shuffle determinism in tests.</param>
    public int? Next(int count, int? currentIndex, bool shuffle, Random? random = null)
    {
        if (count <= 0) return null;
        if (count == 1) return 0;

        if (!shuffle)
        {
            var prev = currentIndex ?? -1;
            return (prev + 1) % count;
        }

        random ??= Random.Shared;
        int next;
        do
        {
            next = random.Next(count);
        }
        while (next == currentIndex);
        return next;
    }
}
