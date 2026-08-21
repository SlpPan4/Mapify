import { get, post, del } from './client.js'

export function getCategories() {
  return get('api/categories')
}

export function getCategoryById(id) {
  return get(`api/categories/${id}`)
}

export function createCategory(data) {
  return post('api/categories', data)
}

export function deleteCategory(id) {
  return del(`api/categories/${id}`)
}
