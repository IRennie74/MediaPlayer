# Anthropic Company Test without Rules
This project is a C# app built only by claude and 4 prompts. This project tests claudes ability to program correct c# blazor and blazorise code without any claude.md rules setup. Claude built a visually good app that works, but behind the scene it failed to follow some architecture rules that any c# blazor developer knows. The issues are as follows: poor choices on packages, using out of date packages, generally incorrect achitecture, and switching between clean blazor and blazorise to instead basic css and html.

# MediaPlayer

A Blazor WebAssembly digital signage / kiosk application. Built strictly with
[Blazorise](https://blazorise.com/) (Bootstrap 5 provider) — no hand-written
CSS file. Runs entirely in the browser; no backend.

## What it does

- **Public display** at `/` — a chromeless full-screen slideshow that loops
  through images, videos, and embedded websites according to the playlist
  assigned to this kiosk. Looks like nothing but a slideshow.
- **Admin** at `/admin` (password gated) — manage Playlists, Media Library,
  Locations, and Kiosks. Each playlist item can be customized like a
  stripped-down video editor (duration, transition, fit, plus video and
  iframe-specific options).
- **Display picker** — on the kiosk page press `Ctrl + Shift + D` (or click
  the invisible 60×60 patch in the top-right corner) to claim this browser as
  a specific kiosk. Survives refreshes via LocalStorage.
- **Backup / restore** — export the entire library + playlists + locations +
  kiosks (media blobs included) to a single JSON file you can import on
  another kiosk to sync content.

## Quick start

```bash
# 1. Install the .NET 8 SDK
#    https://dotnet.microsoft.com/download/dotnet/8.0

# 2. Restore + run
dotnet run --project src/MediaPlayer.Client

# 3. Open http://localhost:5159 (port may differ — see launchSettings.json)
```

The app is full client-side; `dotnet run` only serves the static WASM bundle.

## Default credentials

```
Password: password
```

This is a hardcoded constant in `MediaPlayer.Core.Services.AuthGate`.
**It is not real security** — it lives in the WASM bytes the browser downloads
and a determined user can flip the LocalStorage flag to bypass it. It is
acceptable here only because the brief framed this as a test app with no
sensitive data. Replace before any real deployment.

## Tech stack

| Concern             | Choice                                                                        |
|---------------------|-------------------------------------------------------------------------------|
| UI framework        | Blazor WebAssembly (.NET 8)                                                   |
| Components          | Blazorise + Bootstrap 5 provider + FontAwesome icons                          |
| Persistence         | IndexedDB (entities + media blobs) + LocalStorage (auth flag, kiosk identity) |
| Tests               | xUnit + FluentAssertions + bUnit + coverlet                                   |

## Layout

```
src/MediaPlayer.Core/        # Domain records, IRepository, pure services
  Domain/                    # MediaItem, Playlist, PlaylistItem, Location, Kiosk, ...
  Abstractions/              # IRepository<T>, IAuthGate, IMediaBlobStore, ...
  Services/                  # AuthGate, MediaImportRules, PlaylistMutator,
                             # PlaylistRotationService, BackupService

src/MediaPlayer.Client/      # The WASM app
  Pages/                     # DisplayPage (kiosk), AdminLogin, Admin/*
  Layout/                    # MainLayout (admin shell), DisplayLayout (chromeless)
  Components/Shell/          # TopBar, Sidebar
  Components/Cards/          # PageHeaderCard, ContentCard, SummaryCard
  Components/Common/         # AuthGuard, ConfirmDeleteModal, ExportImportCard
  Components/Media/          # AddMediaModal
  Components/Playlist/       # MediaPickerModal, PlaylistItemEditModal
  Components/Display/        # SlideHost, ImageSlide, VideoSlide, IframeSlide,
                             # DisplayPickerModal
  Services/                  # AuthFlagStore, DisplayInterop
  Services/Storage/          # IndexedDb interop + repositories
  wwwroot/js/                # indexeddb.js, display.js (the only JS we ship)

tests/MediaPlayer.Tests/     # xUnit + bUnit
```

## Routes

| Path                              | Layout         | Description                       |
|-----------------------------------|----------------|-----------------------------------|
| `/`                               | DisplayLayout  | Kiosk slideshow                   |
| `/display` / `/display/{kioskId}` | DisplayLayout  | Same; URL pre-seeds kiosk claim   |
| `/admin/login`                    | DisplayLayout  | Password gate                     |
| `/admin`                          | MainLayout     | Dashboard + summary cards + backup|
| `/admin/playlists`                | MainLayout     | Playlist list                     |
| `/admin/playlists/{id}`           | MainLayout     | Playlist editor (per-item options)|
| `/admin/media`                    | MainLayout     | Media library                     |
| `/admin/locations`                | MainLayout     | Location CRUD                     |
| `/admin/kiosks`                   | MainLayout     | Kiosk CRUD + playlist assignment  |

## Tests

```bash
dotnet test
```

50+ tests covering:

- Domain record defaults and `with`-expression immutability
- `AuthGate` (correct/wrong password, case sensitivity, persistence, events)
- `MediaImportRules` (size, MIME allowlist, URL scheme, soft-warn vs hard-cap)
- `PlaylistMutator` (append, remove, move up/down, update, reindex)
- `PlaylistRotationService` (empty, single-item, in-order wrap, shuffle invariant)
- `BackupService` (roundtrip via JSON of entities + media blobs)
- bUnit smoke tests for `AdminLogin` (wrong password shows alert; correct
  password authenticates and navigates)

Coverage on `MediaPlayer.Core`: **~99% line / 90% branch**.

`MediaPlayer.Client` (Razor components + JS interop wrappers) is light on
unit tests — most of that code is thin binding around the Core services and
needs a real browser (IndexedDB, fullscreen API) to exercise meaningfully.
The included bUnit smoke test demonstrates the pattern; further component
tests can be added in the same `tests/MediaPlayer.Tests/Components/` folder.

## Known limitations & caveats

- **Iframe embedding** — many websites send `X-Frame-Options: DENY` or a CSP
  `frame-ancestors` directive that *prevents* embedding. The browser enforces
  this; the app cannot bypass it. Use the **Test embed** button in the Add
  Media → Website tab to check before saving.
- **IndexedDB quota** — varies by browser, typically a percentage of free
  disk. Per-file soft cap is 50 MB (warn), hard cap 200 MB (block).
- **Per-browser data** — every kiosk machine has its own IndexedDB. To sync
  content between kiosks, use **Dashboard → Backup → Export to file** on the
  source machine and **Import** on each target.
- **Video autoplay** — the kiosk page requests fullscreen on the first user
  click; this also satisfies the browser's autoplay-needs-a-gesture policy.
  Until the operator clicks once, videos may be paused.
- **Hardcoded password** — see "Default credentials" above.

## Adding new media

In Admin → Media Library, click **Add Media**. Three tabs:

1. **Image** — pick a PNG/JPEG/WebP/GIF up to 50 MB (warn) / 200 MB (block).
2. **Video** — pick an MP4/WebM/Ogg with the same limits.
3. **Website** — paste any `https://` URL. Use **Test embed** to confirm the
   destination allows iframing before saving.

## Building a playlist

In Admin → Playlists → New Playlist → Edit. Click **Add Media** to open the
picker, then use Up/Down to reorder and the yellow Edit button on each row
to set per-item options (duration override, transition, fit mode; for video
tracks: muted/loop/volume/start time; for iframes: interactive toggle and
zoom).

Assign the playlist to a kiosk in Admin → Kiosks.

## Claiming a screen

1. On the kiosk computer, open the public URL (`/`).
2. Press `Ctrl + Shift + D` to open the picker.
3. Pick the location and the kiosk identity for this screen → Save.
4. Click anywhere on the page once to enter fullscreen and unlock video
   autoplay.

The browser will remember its assignment via LocalStorage; a refresh keeps
the slideshow running automatically.
