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
  // Scroll to top only on real page changes. Query-only updates (the filter
  // mirror on the home page) must keep the scroll position, otherwise every
  // select change would throw the user back to the top on a phone.
  scrollBehavior: (to, from, savedPosition) => {
    if (savedPosition) return savedPosition
    if (to.path === from.path) return false
    return { top: 0 }
  },
})

export default router
