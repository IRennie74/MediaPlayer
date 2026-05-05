using MediaPlayer.Core.Domain;

namespace MediaPlayer.Core.Abstractions;

/// <summary>
/// Per-browser persistence of which kiosk this physical screen claims to be.
/// Backed by LocalStorage so every kiosk machine has its own assignment.
/// </summary>
public interface IDisplayAssignmentStore
{
    Task<DisplayAssignment?> GetAsync(CancellationToken cancellationToken = default);
    Task SetAsync(DisplayAssignment assignment, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
