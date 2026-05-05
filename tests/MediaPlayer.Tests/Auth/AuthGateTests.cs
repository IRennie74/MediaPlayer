using FluentAssertions;
using MediaPlayer.Core.Services;

namespace MediaPlayer.Tests.Auth;

public sealed class AuthGateTests
{
    private readonly InMemoryAuthFlagStore store = new();
    private AuthGate Sut => new(store);

    [Fact]
    public async Task TryLogin_WithCorrectPassword_AuthenticatesAndPersists()
    {
        var gate = Sut;
        var ok = await gate.TryLoginAsync(AuthGate.AdminPassword);

        ok.Should().BeTrue();
        gate.IsAuthenticated.Should().BeTrue();
        store.Value.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("wrong")]
    [InlineData("Password")]   // case-sensitive
    [InlineData("password ")]  // trailing whitespace not trimmed
    public async Task TryLogin_WithIncorrectPassword_DoesNotAuthenticate(string attempt)
    {
        var gate = Sut;
        var ok = await gate.TryLoginAsync(attempt);

        ok.Should().BeFalse();
        gate.IsAuthenticated.Should().BeFalse();
        store.Value.Should().BeFalse();
    }

    [Fact]
    public async Task TryLogin_RaisesChangedExactlyOnce_OnFirstSuccess()
    {
        var gate = Sut;
        var raisedCount = 0;
        gate.Changed += () => raisedCount++;

        await gate.TryLoginAsync(AuthGate.AdminPassword);
        await gate.TryLoginAsync(AuthGate.AdminPassword); // already authed

        raisedCount.Should().Be(1);
    }

    [Fact]
    public async Task Logout_ClearsAuthAndPersistedFlag()
    {
        var gate = Sut;
        await gate.TryLoginAsync(AuthGate.AdminPassword);

        await gate.LogoutAsync();

        gate.IsAuthenticated.Should().BeFalse();
        store.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Logout_WhenNotAuthenticated_IsNoOp()
    {
        var gate = Sut;
        var raisedCount = 0;
        gate.Changed += () => raisedCount++;

        await gate.LogoutAsync();

        gate.IsAuthenticated.Should().BeFalse();
        raisedCount.Should().Be(0);
    }

    [Fact]
    public async Task Initialize_RehydratesPersistedAuthState()
    {
        await store.SetAsync(true);
        var gate = Sut;

        await gate.InitializeAsync();

        gate.IsAuthenticated.Should().BeTrue();
    }
}
