const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/'

function ensureTrailingSlash(url) {
  return url.endsWith('/') ? url : url + '/'
}

function buildUrl(path) {
  const base = ensureTrailingSlash(BASE_URL)
  const cleanPath = path.startsWith('/') ? path.slice(1) : path
  return base + cleanPath
}

async function parseResponse(response) {
  const text = await response.text()
  const data = text ? JSON.parse(text) : null

  if (!response.ok) {
    const error = data?.error || data?.message || `HTTP ${response.status}`
    throw new Error(error)
  }

  return data?.data ?? data
}

export async function get(path) {
  const response = await fetch(buildUrl(path), {
    headers: { Accept: 'application/json' },
  })
  return parseResponse(response)
}

export async function post(path, body) {
  const response = await fetch(buildUrl(path), {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
  return parseResponse(response)
}

export async function put(path, body) {
  const response = await fetch(buildUrl(path), {
    method: 'PUT',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
  return parseResponse(response)
}

export async function patch(path, body) {
  const response = await fetch(buildUrl(path), {
    method: 'PATCH',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
  return parseResponse(response)
}

export async function del(path) {
  const response = await fetch(buildUrl(path), {
    method: 'DELETE',
    headers: { Accept: 'application/json' },
  })
  return parseResponse(response)
}
