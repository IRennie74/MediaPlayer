namespace MediaPlayer.Core.Domain;

/// <summary>
/// Where a <see cref="TickerItem"/> lives within the kiosk-bottom ticker bar.
/// Pills sit on the left as static badges; Scrolling items are concatenated
/// into the right-hand marquee that loops endlessly.
/// </summary>
public enum TickerItemKind
{
    Pill = 0,
    Scrolling = 1,
}
