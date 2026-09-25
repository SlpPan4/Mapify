const TRN_API_KEY = process.env.TRN_API_KEY || '';
const TRN_BASE_URL = 'https://public-api.tracker.gg/v2';
const REQUEST_TIMEOUT_MS = 10_000;
const CACHE_TTL_MS = 5 * 60 * 1000;

const profileCache = new Map();

export class TrnError extends Error {
    constructor(code, message) {
        super(message);
        this.code = code;
    }
}

function getCached(key) {
    const entry = profileCache.get(key);

    if (!entry) {
        return null;
    }

    if (Date.now() - entry.timestamp > CACHE_TTL_MS) {
        profileCache.delete(key);
        return null;
    }

    return entry.data;
}

function setCached(key, data) {
    profileCache.set(key, { data, timestamp: Date.now() });
}

function pickStat(stats, keys) {
    for (const key of keys) {
        const stat = stats?.[key];

        if (stat && stat.displayValue != null) {
            return stat.displayValue;
        }
    }

    return null;
}

function normalizeProfile(raw) {
    const data = raw?.data ?? raw;
    const segments = Array.isArray(data?.segments) ? data.segments : [];

    const overview = segments.find(segment => segment.type === 'overview');
    const overviewStats = overview?.stats ?? {};

    const profile = {
        handle: data?.platformInfo?.platformUserHandle || 'Unknown',
        platform: data?.platformInfo?.platformSlug || '',
        avatarUrl: data?.platformInfo?.avatarUrl || null,
        kd: pickStat(overviewStats, ['kd', 'kdRatio']),
        wlPercent: pickStat(overviewStats, ['wlPercent', 'wlPercentage', 'winRate']),
        kills: pickStat(overviewStats, ['kills']),
        deaths: pickStat(overviewStats, ['deaths']),
        wins: pickStat(overviewStats, ['wins']),
        losses: pickStat(overviewStats, ['losses']),
        matchesPlayed: pickStat(overviewStats, ['matchesPlayed']),
        timePlayed: pickStat(overviewStats, ['timePlayed'])
    };

    const playlists = segments
        .filter(segment => segment.type === 'playlist')
        .map(segment => ({
            name: segment.metadata?.name || segment.attributes?.playlist || 'Unknown',
            rank: pickStat(segment.stats, ['rank']),
            mmr: pickStat(segment.stats, ['mmr', 'rankPoints']),
            kd: pickStat(segment.stats, ['kd', 'kdRatio']),
            wlPercent: pickStat(segment.stats, ['wlPercent', 'wlPercentage']),
            matchesPlayed: pickStat(segment.stats, ['matchesPlayed'])
        }));

    const operators = segments
        .filter(segment => segment.type === 'operator')
        .map(segment => ({
            name: segment.metadata?.name || segment.attributes?.operator || 'Unknown',
            kills: pickStat(segment.stats, ['kills']),
            kd: pickStat(segment.stats, ['kd', 'kdRatio']),
            wins: pickStat(segment.stats, ['wins']),
            roundsPlayed: pickStat(segment.stats, ['roundsPlayed']),
            roundsPlayedValue: segment.stats?.roundsPlayed?.value ?? 0
        }))
        .sort((a, b) => b.roundsPlayedValue - a.roundsPlayedValue)
        .map(({ roundsPlayedValue, ...operator }) => operator);

    return { profile, playlists, operators };
}

export async function getProfile(platform, gamertag) {
    if (!TRN_API_KEY) {
        throw new TrnError(
            'no_key',
            'TRN_API_KEY is not configured on the bot.'
        );
    }

    const cacheKey = `${platform}:${gamertag.toLowerCase()}`;
    const cached = getCached(cacheKey);

    if (cached) {
        return cached;
    }

    let response;

    try {
        response = await fetch(
            `${TRN_BASE_URL}/r6/standard/profile/${platform}/${encodeURIComponent(gamertag)}`,
            {
                headers: { 'TRN-Api-Key': TRN_API_KEY },
                signal: AbortSignal.timeout(REQUEST_TIMEOUT_MS)
            }
        );
    } catch (error) {
        if (error.name === 'TimeoutError' || error.name === 'AbortError') {
            throw new TrnError(
                'timeout',
                'The stats service did not respond in time.'
            );
        }
        throw new TrnError(
            'unavailable',
            'The stats service is unavailable.'
        );
    }

    if (response.status === 404) {
        throw new TrnError(
            'not_found',
            `Player **${gamertag}** was not found on this platform. Check the nickname and platform.`
        );
    }

    if (response.status === 401 || response.status === 403) {
        console.error(`TRN API auth error: HTTP ${response.status}`);
        throw new TrnError(
            'auth',
            'The stats service rejected the API key. Please contact the bot administrator.'
        );
    }

    if (response.status === 429) {
        throw new TrnError(
            'rate_limited',
            'Stats request limit exceeded. Please try again in a minute.'
        );
    }

    if (!response.ok) {
        throw new TrnError(
            'unavailable',
            `The stats service returned an error (HTTP ${response.status}).`
        );
    }

    const raw = await response.json();
    const normalized = normalizeProfile(raw);

    setCached(cacheKey, normalized);

    return normalized;
}

export function profileUrl(platform, gamertag) {
    return `https://tracker.gg/r6siege/profile/${platform}/${encodeURIComponent(gamertag)}/overview`;
}
