using MediaPlayer.Core.Abstractions;

namespace MediaPlayer.Tests.Auth;

internal sealed class InMemoryAuthFlagStore : IAuthFlagStore
{
    public bool Value { get; private set; }

    public Task<bool> GetAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Value);

    public Task SetAsync(bool value, CancellationToken cancellationToken = default)
    {
        Value = value;
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        Value = false;
        return Task.CompletedTask;
    }
}
