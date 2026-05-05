using MediaPlayer.Core.Abstractions;

namespace MediaPlayer.Core.Services;

/// <summary>
/// Default <see cref="IAuthGate"/>. Compares the supplied input against a
/// hardcoded constant and persists the boolean result through an
/// <see cref="IAuthFlagStore"/>.
/// </summary>
/// <remarks>
/// SECURITY NOTE — the password sits in client-side WASM bytes; anyone with
/// the browser dev tools can flip the LocalStorage flag and bypass it. This
/// is acceptable only because the brief explicitly framed this as a test app
/// with no sensitive data. Replace with real auth before any real deployment.
/// </remarks>
public sealed class AuthGate : IAuthGate
{
    /// <summary>Hardcoded admin password from the spec.</summary>
    public const string AdminPassword = "password";

    private readonly IAuthFlagStore store;
    private bool isAuthenticated;

    public AuthGate(IAuthFlagStore store)
    {
        this.store = store;
    }

    public bool IsAuthenticated => isAuthenticated;

    public event Action? Changed;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await store.GetAsync(cancellationToken).ConfigureAwait(false);
        if (persisted != isAuthenticated)
        {
            isAuthenticated = persisted;
            Changed?.Invoke();
        }
    }

    public async Task<bool> TryLoginAsync(string password, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(password, AdminPassword, StringComparison.Ordinal))
        {
            return false;
        }

        if (!isAuthenticated)
        {
            isAuthenticated = true;
            await store.SetAsync(true, cancellationToken).ConfigureAwait(false);
            Changed?.Invoke();
        }
        return true;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!isAuthenticated)
        {
            return;
        }
        isAuthenticated = false;
        await store.ClearAsync(cancellationToken).ConfigureAwait(false);
        Changed?.Invoke();
    }
}
