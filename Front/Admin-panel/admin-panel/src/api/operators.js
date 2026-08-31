import { get, post, del } from './client.js'

export function getOperators() {
  return get('api/operators')
}

export function assignOperatorToStrat(operatorId, stratId) {
  return post(`api/operators/${operatorId}/assign/${stratId}`)
}

export function removeOperatorFromStrat(operatorId, stratId) {
  return del(`api/operators/${operatorId}/remove/${stratId}`)
}
