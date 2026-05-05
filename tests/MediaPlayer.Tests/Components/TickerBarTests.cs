using Bunit;
using FluentAssertions;
using MediaPlayer.Client.Components.Display;
using MediaPlayer.Core.Domain;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace MediaPlayer.Tests.Components;

public sealed class TickerBarTests : Bunit.TestContext
{
    public TickerBarTests()
    {
        JSInterop.Mode = Bunit.JSRuntimeMode.Loose;
        Services
            .AddBlazorise(o => o.Immediate = true)
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    [Fact]
    public void EmptyItems_RendersNothing()
    {
        var cut = RenderComponent<TickerBar>(p => p.Add(b => b.Items, Array.Empty<TickerItem>()));

        cut.Markup.Trim().Should().BeEmpty();
    }

    [Fact]
    public void PillsRenderAsBadges_AndScrollingTextJoined()
    {
        var items = new TickerItem[]
        {
            new() { Id = Guid.NewGuid(), Text = "URGENT", OrderIndex = 0, Kind = TickerItemKind.Pill, ColorHint = "Danger" },
            new() { Id = Guid.NewGuid(), Text = "Note", OrderIndex = 1, Kind = TickerItemKind.Pill },
            new() { Id = Guid.NewGuid(), Text = "Welcome to the lobby", OrderIndex = 2, Kind = TickerItemKind.Scrolling },
            new() { Id = Guid.NewGuid(), Text = "Visit our showroom", OrderIndex = 3, Kind = TickerItemKind.Scrolling },
        };

        var cut = RenderComponent<TickerBar>(p => p.Add(b => b.Items, items));

        cut.FindAll(".badge").Should().HaveCount(2);
        cut.Markup.Should().Contain("URGENT");
        cut.Markup.Should().Contain("Welcome to the lobby");
        cut.Markup.Should().Contain("Visit our showroom");
        cut.Markup.Should().Contain("ticker-scroll");
    }

    [Fact]
    public void MarqueeDuration_ScalesWithTextLength_FloorsAt15Seconds()
    {
        var shortItems = new[] {
            new TickerItem { Id = Guid.NewGuid(), Text = "Hi", OrderIndex = 0, Kind = TickerItemKind.Scrolling }
        };
        var longText = new string('x', 400);
        var longItems = new[] {
            new TickerItem { Id = Guid.NewGuid(), Text = longText, OrderIndex = 0, Kind = TickerItemKind.Scrolling }
        };

        var shortCut = RenderComponent<TickerBar>(p => p.Add(b => b.Items, shortItems));
        var longCut  = RenderComponent<TickerBar>(p => p.Add(b => b.Items, longItems));

        shortCut.Markup.Should().Contain("15s");      // floor
        longCut.Markup.Should().Contain("100s");      // 400 / 4
    }
}
