using Microsoft.JSInterop;

namespace MediaPlayer.Client.Services;

/// <summary>
/// Thin wrapper over <c>wwwroot/js/display.js</c>. Centralizes the fullscreen
/// API calls and the global keyboard listener so the kiosk page does not
/// touch <see cref="IJSRuntime"/> directly.
/// </summary>
public sealed class DisplayInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public DisplayInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/display.js").AsTask());
    }

    public async Task RegisterHotkeyAsync<T>(DotNetObjectReference<T> reference) where T : class
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("registerHotkey", reference).ConfigureAwait(false);
    }

    public async Task UnregisterHotkeyAsync()
    {
        if (!moduleTask.IsValueCreated) return;
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterHotkey").ConfigureAwait(false);
    }

    public async Task RequestFullscreenAsync()
    {
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("requestFullscreen").ConfigureAwait(false);
    }

    public async Task ExitFullscreenAsync()
    {
        if (!moduleTask.IsValueCreated) return;
        var module = await moduleTask.Value.ConfigureAwait(false);
        await module.InvokeVoidAsync("exitFullscreen").ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value.ConfigureAwait(false);
            await module.DisposeAsync().ConfigureAwait(false);
        }
    }
}
