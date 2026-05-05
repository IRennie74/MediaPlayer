using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using MediaPlayer.Client.Pages;
using MediaPlayer.Client.Services;
using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Services;
using MediaPlayer.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace MediaPlayer.Tests.Components;

/// <summary>
/// One representative bUnit test that proves wired-up component testing
/// works against this code base (DI, real AuthGate, FakeNavigationManager).
/// Wider Razor-component coverage is intentionally deferred — most Client
/// code is thin binding around the Core services, which already have 97%
/// unit-test coverage.
/// </summary>
public sealed class AdminLoginTests : Bunit.TestContext
{
    public AdminLoginTests()
    {
        // Loose JS interop — Blazorise components fire JS calls on render and
        // dispose; we don't need to assert on those, just let them no-op.
        JSInterop.Mode = Bunit.JSRuntimeMode.Loose;

        Services
            .AddBlazorise(o => o.Immediate = true)
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        Services.AddSingleton<IAuthFlagStore, InMemoryAuthFlagStore>();
        Services.AddSingleton<IAuthGate, AuthGate>();
    }

    [Fact]
    public void WrongPassword_ShowsErrorAlert_AndDoesNotNavigate()
    {
        var nav = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<AdminLogin>();

        cut.Find("input[type=password]").Input("nope");
        cut.Find("button.btn-success").Click();

        cut.Markup.Should().Contain("Incorrect password");
        nav.Uri.Should().NotEndWith("/admin");
    }

    [Fact]
    public void CorrectPassword_AuthenticatesAndNavigatesToDashboard()
    {
        var nav = Services.GetRequiredService<FakeNavigationManager>();
        var gate = Services.GetRequiredService<IAuthGate>();
        var cut = RenderComponent<AdminLogin>();

        cut.Find("input[type=password]").Input(AuthGate.AdminPassword);
        cut.Find("button.btn-success").Click();

        gate.IsAuthenticated.Should().BeTrue();
        nav.Uri.Should().EndWith("/admin");
    }
}
