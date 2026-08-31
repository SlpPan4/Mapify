import { get } from './client.js'

export function getMaps() {
  return get('api/maps')
}
