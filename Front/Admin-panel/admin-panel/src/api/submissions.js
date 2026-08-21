import { get, post, del } from './client.js'

export function getPendingStratSubmissions() {
  return get('api/submissions/admin/strats')
}

export function getPendingCategorySubmissions() {
  return get('api/submissions/admin/categories')
}

export function approveStratSubmission(id) {
  return post(`api/submissions/admin/strats/${id}/approve`)
}

export function rejectStratSubmission(id) {
  return del(`api/submissions/admin/strats/${id}`)
}

export function approveCategorySubmission(id) {
  return post(`api/submissions/admin/categories/${id}/approve`)
}

export function rejectCategorySubmission(id) {
  return del(`api/submissions/admin/categories/${id}`)
}
