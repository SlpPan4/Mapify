import { get, post, put, patch, del } from './client.js'

export function getStrats(params = {}) {
  const searchParams = new URLSearchParams()
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      searchParams.append(key, value)
    }
  })
  const query = searchParams.toString()
  return get(`api/strats${query ? '?' + query : ''}`)
}

export function getStratsSummary() {
  return get('api/strats/summary')
}

export function getStratById(id) {
  return get(`api/strats/${id}`)
}

export function createStrat(data) {
  return post('api/strats', data)
}

export function updateStrat(id, data) {
  return put(`api/strats/${id}`, data)
}

export function patchStrat(id, data) {
  return patch(`api/strats/${id}`, data)
}

export function deleteStrat(id) {
  return del(`api/strats/${id}`)
}

export function assignCategoryToStrat(stratId, categoryId) {
  return post(`api/strats/assign/strat/${stratId}/category/${categoryId}`)
}

export function removeCategoryFromStrat(stratId, categoryId) {
  return del(`api/strats/${stratId}/categories/${categoryId}`)
}

export function getStratsByCategory(id) {
  return get(`api/strats/category/${id}`)
}
