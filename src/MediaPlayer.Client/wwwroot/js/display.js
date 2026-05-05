// Kiosk display interop — fullscreen API and a global hotkey for the
// hidden display-picker overlay. Exposes a small surface so the C# side
// can subscribe and unsubscribe cleanly when DisplayPage disposes.

let dotnetRef = null;
let keyHandler = null;

export function registerHotkey(reference) {
    dotnetRef = reference;
    keyHandler = (e) => {
        // Ctrl+Shift+D opens the picker without breaking the slideshow look.
        if (e.ctrlKey && e.shiftKey && (e.key === 'D' || e.key === 'd')) {
            e.preventDefault();
            dotnetRef?.invokeMethodAsync('OpenDisplayPicker');
        }
    };
    document.addEventListener('keydown', keyHandler);
}

export function unregisterHotkey() {
    if (keyHandler) document.removeEventListener('keydown', keyHandler);
    keyHandler = null;
    dotnetRef = null;
}

export async function requestFullscreen() {
    const el = document.documentElement;
    try {
        if (el.requestFullscreen) await el.requestFullscreen();
        else if (el.webkitRequestFullscreen) el.webkitRequestFullscreen();
    } catch {
        // Browser refused (e.g. no user gesture yet) — silently fall back.
    }
}

export function exitFullscreen() {
    try {
        if (document.exitFullscreen) document.exitFullscreen();
    } catch {}
}

export function isFullscreen() {
    return !!document.fullscreenElement;
}

export async function toggleFullscreen() {
    if (document.fullscreenElement) {
        try { if (document.exitFullscreen) await document.exitFullscreen(); } catch {}
    } else {
        const el = document.documentElement;
        try {
            if (el.requestFullscreen) await el.requestFullscreen();
            else if (el.webkitRequestFullscreen) el.webkitRequestFullscreen();
        } catch {}
    }
}
