namespace MediaPlayer.Core.Abstractions;

/// <summary>
/// Tiny persistence seam used by <see cref="IAuthGate"/>. Pulled out from
/// the LocalStorage SDK so the gate can be unit-tested without any browser
/// or JS-interop dependency.
/// </summary>
public interface IAuthFlagStore
{
    Task<bool> GetAsync(CancellationToken cancellationToken = default);
    Task SetAsync(bool value, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
