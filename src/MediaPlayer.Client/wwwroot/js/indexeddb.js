// Minimal IndexedDB wrapper — one shared database, one object store per
// entity type, plus a separate "mediaBlobs" store that holds raw Blob
// objects so images and videos can be served back as `blob:` URLs without
// round-tripping bytes through the .NET runtime.

const DB_NAME = 'mediaplayer';
const DB_VERSION = 1;
const ENTITY_STORES = ['mediaItems', 'playlists', 'locations', 'kiosks'];
const BLOB_STORE = 'mediaBlobs';

let dbPromise = null;

function openDb() {
    if (dbPromise) return dbPromise;
    dbPromise = new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);
        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            for (const name of ENTITY_STORES) {
                if (!db.objectStoreNames.contains(name)) {
                    db.createObjectStore(name);
                }
            }
            if (!db.objectStoreNames.contains(BLOB_STORE)) {
                db.createObjectStore(BLOB_STORE);
            }
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
    return dbPromise;
}

async function runStore(storeName, mode, action) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(storeName, mode);
        const store = transaction.objectStore(storeName);
        let result;
        try {
            const request = action(store);
            if (request) {
                request.onsuccess = () => { result = request.result; };
                request.onerror = () => reject(request.error);
            }
        } catch (err) {
            reject(err);
            return;
        }
        transaction.oncomplete = () => resolve(result);
        transaction.onerror = () => reject(transaction.error);
        transaction.onabort = () => reject(transaction.error);
    });
}

// ---- Entity records (stored as plain JS objects) ------------------------

export async function getAll(storeName) {
    return runStore(storeName, 'readonly', store => store.getAll());
}

export async function getById(storeName, key) {
    return runStore(storeName, 'readonly', store => store.get(key));
}

export async function put(storeName, key, value) {
    return runStore(storeName, 'readwrite', store => store.put(value, key));
}

export async function remove(storeName, key) {
    return runStore(storeName, 'readwrite', store => store.delete(key));
}

export async function clearStore(storeName) {
    return runStore(storeName, 'readwrite', store => store.clear());
}

// ---- Media blobs --------------------------------------------------------

export async function saveBlob(key, base64, mimeType) {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
    const blob = new Blob([bytes], { type: mimeType || 'application/octet-stream' });
    return runStore(BLOB_STORE, 'readwrite', store => store.put(blob, key));
}

export async function getBlobUrl(key) {
    const blob = await runStore(BLOB_STORE, 'readonly', store => store.get(key));
    if (!blob) return null;
    return URL.createObjectURL(blob);
}

export async function getBlobBase64(key) {
    const blob = await runStore(BLOB_STORE, 'readonly', store => store.get(key));
    if (!blob) return null;
    const buffer = await blob.arrayBuffer();
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.length; i++) binary += String.fromCharCode(bytes[i]);
    return btoa(binary);
}

export function downloadJson(filename, content) {
    const blob = new Blob([content], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

export async function deleteBlob(key) {
    return runStore(BLOB_STORE, 'readwrite', store => store.delete(key));
}

export function revokeObjectUrl(url) {
    if (url) URL.revokeObjectURL(url);
}
