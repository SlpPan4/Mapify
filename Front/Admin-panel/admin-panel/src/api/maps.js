import { get, post, put, del } from './client.js'

export function getMaps() {
  return get('api/maps')
}

export function getMap(id) {
  return get(`api/maps/${id}`)
}

export function createMap(name) {
  return post('api/admin/maps', { name })
}

export function updateMap(id, name) {
  return put(`api/admin/maps/${id}`, { name })
}

export function deleteMap(id) {
  return del(`api/admin/maps/${id}`)
}
