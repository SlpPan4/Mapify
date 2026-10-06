import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/HomeView.vue'),
    },
    {
      path: '/strats',
      name: 'strats',
      component: () => import('@/views/StratsView.vue'),
    },
    {
      path: '/strats/:id',
      name: 'strat-detail',
      component: () => import('@/views/StratDetailView.vue'),
    },
    {
      path: '/constructor',
      name: 'constructor',
      component: () => import('@/views/ConstructorView.vue'),
    },
    {
      path: '/submit',
      name: 'submit',
      component: () => import('@/views/SubmitView.vue'),
    },
    {
      path: '/offline',
      name: 'offline',
      component: () => import('@/views/OfflineView.vue'),
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/NotFoundView.vue'),
    },
  ],
  scrollBehavior: () => ({ top: 0 }),
})

export default router
