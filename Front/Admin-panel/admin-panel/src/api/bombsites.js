import { post, put, del } from './client.js'

export function createBombsite(mapId, name) {
  return post(`api/admin/maps/${mapId}/bombsites`, { name })
}

export function updateBombsite(mapId, id, name) {
  return put(`api/admin/maps/${mapId}/bombsites/${id}`, { name })
}

export function deleteBombsite(mapId, id) {
  return del(`api/admin/maps/${mapId}/bombsites/${id}`)
}
