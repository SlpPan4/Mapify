const API_BASE = (process.env.API_BASE_URL || 'http://localhost:5000').replace(/\/+$/, '');
const REQUEST_TIMEOUT_MS = 10_000;

async function request(endpoint, { method = 'GET', body } = {}) {
    const headers = {};

    if (body !== undefined) {
        headers['Content-Type'] = 'application/json';
    }

    let response;

    try {
        response = await fetch(`${API_BASE}/api${endpoint}`, {
            method,
            headers,
            body: body !== undefined ? JSON.stringify(body) : undefined,
            signal: AbortSignal.timeout(REQUEST_TIMEOUT_MS)
        });
    } catch (error) {
        if (error.name === 'TimeoutError' || error.name === 'AbortError') {
            throw new Error('Backend did not respond in time.');
        }
        throw new Error('Failed to connect to the backend.');
    }

    let result = null;

    try {
        result = await response.json();
    } catch {
        // тело не JSON — обработаем по статусу ниже
    }

    if (!response.ok) {
        throw new Error(
            result?.error ||
            result?.message ||
            `Backend returned HTTP ${response.status}.`
        );
    }

    if (result?.error) {
        throw new Error(result.error);
    }

    return result?.data ?? result;
}

export function getStrats() {
    return request('/strats');
}

export function getStrat(id) {
    return request(`/strats/${id}`);
}

export function getMap(mapId) {
    return request(`/strats/maps/${mapId}`);
}

export function getMaps() {
    return request('/maps');
}

export function submitStrat({ name, videoUrl, description, mapId }) {
    return request('/submissions/strats', {
        method: 'POST',
        body: {
            name,
            videoUrl,
            description,
            mapId,
            categoryIds: [],
            operatorIds: []
        }
    });
}
