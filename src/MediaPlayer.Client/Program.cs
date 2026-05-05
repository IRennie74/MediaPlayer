using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using MediaPlayer.Client;
using MediaPlayer.Client.Services;
using MediaPlayer.Client.Services.Storage;
using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Domain;
using MediaPlayer.Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddBlazoredLocalStorage();

// Storage layer.
builder.Services.AddScoped<IndexedDbInterop>();
builder.Services.AddScoped<IMediaBlobStore, IndexedDbBlobStore>();
builder.Services.AddScoped<IDisplayAssignmentStore, LocalStorageDisplayAssignmentStore>();

// Auth gate (single hardcoded password — see AuthGate XML doc for the
// security caveat; acceptable for this test app only).
builder.Services.AddScoped<IAuthFlagStore, LocalStorageAuthFlagStore>();
builder.Services.AddScoped<IAuthGate, AuthGate>();

// Kiosk display interop (fullscreen + global hotkey).
builder.Services.AddScoped<DisplayInterop>();

// JSON backup / restore for cross-kiosk sync.
builder.Services.AddScoped<BackupService>();

// One repository per entity type — each bound to its own object store name.
builder.Services.AddScoped<IRepository<MediaItem>>(sp =>
    new IndexedDbRepository<MediaItem>(sp.GetRequiredService<IndexedDbInterop>(), StoreNames.MediaItems));
builder.Services.AddScoped<IRepository<Playlist>>(sp =>
    new IndexedDbRepository<Playlist>(sp.GetRequiredService<IndexedDbInterop>(), StoreNames.Playlists));
builder.Services.AddScoped<IRepository<Location>>(sp =>
    new IndexedDbRepository<Location>(sp.GetRequiredService<IndexedDbInterop>(), StoreNames.Locations));
builder.Services.AddScoped<IRepository<Kiosk>>(sp =>
    new IndexedDbRepository<Kiosk>(sp.GetRequiredService<IndexedDbInterop>(), StoreNames.Kiosks));

var app = builder.Build();

// Rehydrate the persisted admin auth flag before the first render so
// AuthGuard does not bounce a logged-in operator to the login page on refresh.
await app.Services.GetRequiredService<IAuthGate>().InitializeAsync();

await app.RunAsync();
