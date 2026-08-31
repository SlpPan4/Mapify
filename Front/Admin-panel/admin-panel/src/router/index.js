import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import PendingSubmissionsView from '../views/PendingSubmissionsView.vue'
import StratsView from '../views/StratsView.vue'
import StratDetailView from '../views/StratDetailView.vue'
import CategoriesView from '../views/CategoriesView.vue'
import OperatorsView from '../views/OperatorsView.vue'
import MapsView from '../views/MapsView.vue'

const routes = [
  { path: '/', redirect: '/dashboard' },
  { path: '/dashboard', name: 'Dashboard', component: DashboardView },
  { path: '/pending', name: 'Pending Submissions', component: PendingSubmissionsView },
  { path: '/strats', name: 'Strats', component: StratsView },
  { path: '/strats/:id', name: 'Strat Detail', component: StratDetailView, props: true },
  { path: '/categories', name: 'Categories', component: CategoriesView },
  { path: '/operators', name: 'Operators', component: OperatorsView },
  { path: '/maps', name: 'Maps', component: MapsView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
