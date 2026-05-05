namespace MediaPlayer.Core.Abstractions;

/// <summary>
/// Single-user, single-password admin gate. NOT real authentication —
/// trivially bypassable client-side. Acceptable only for this test app.
/// </summary>
public interface IAuthGate
{
    bool IsAuthenticated { get; }

    /// <summary>Restore the persisted auth flag from storage. Call once on app startup.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <returns>True if the password matched and the session is now authenticated.</returns>
    Task<bool> TryLoginAsync(string password, CancellationToken cancellationToken = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>Raised whenever <see cref="IsAuthenticated"/> changes.</summary>
    event Action? Changed;
}
