const API_URL = 'http://localhost:5000/api';

async function request(endpoint, options = {}) {
    const response = await fetch(
        `${API_URL}${endpoint}`,
        options
    );

    let result;

    try {
        result = await response.json();
    } catch {
        throw new Error(
            `API returned invalid JSON (${response.status})`
        );
    }

    if (!response.ok) {
        throw new Error(
            result.error ||
            result.message ||
            `API returned HTTP ${response.status}`
        );
    }

    if (result.error) {
        throw new Error(result.error);
    }

    return result.data;
}

export async function getStrats() {
    return await request('/strats');
}

export async function getMap(mapId) {
    return await request(`/strats/maps/${mapId}`);
}