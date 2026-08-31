<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import Sidebar from './components/Sidebar.vue'
import Header from './components/Header.vue'
import ToastContainer from './components/ToastContainer.vue'
import { getPendingStratSubmissions, getPendingCategorySubmissions } from './api/submissions.js'

const router = useRouter()
const pendingStrats = ref([])
const pendingCategories = ref([])
const loading = ref(false)

const pendingCount = computed(() => pendingStrats.value.length + pendingCategories.value.length)

function formatRelativeTime(iso) {
  const diff = Date.now() - new Date(iso).getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 1) return 'Just now'
  if (minutes < 60) return `${minutes} min ago`
  if (hours < 24) return `${hours} hr ago`
  if (days === 1) return 'Yesterday'
  return `${days} days ago`
}

const notifications = computed(() => {
  const stratNotifications = pendingStrats.value.map((s) => ({
    text: `New strat submission: ${s.name}`,
    time: formatRelativeTime(s.submittedAt),
    submittedAt: s.submittedAt,
  }))
  const categoryNotifications = pendingCategories.value.map((c) => ({
    text: `New category submission: ${c.name}`,
    time: formatRelativeTime(c.submittedAt),
    submittedAt: c.submittedAt,
  }))
  return [...stratNotifications, ...categoryNotifications]
    .sort((a, b) => new Date(b.submittedAt) - new Date(a.submittedAt))
    .slice(0, 10)
})

async function loadPendingCounts() {
  try {
    const [strats, categories] = await Promise.all([
      getPendingStratSubmissions(),
      getPendingCategorySubmissions(),
    ])
    pendingStrats.value = strats || []
    pendingCategories.value = categories || []
  } catch (error) {
    console.error('Failed to load pending counts:', error)
  }
}

onMounted(() => {
  loadPendingCounts()
})

router.afterEach(() => {
  loadPendingCounts()
})
</script>

<template>
  <div class="min-h-full bg-[#0B0B0C] flex">
    <Sidebar :pending-count="pendingCount" />

    <div class="flex-1 flex flex-col min-w-0" style="margin-left: 240px;">
      <Header :notifications="notifications" @refresh="loadPendingCounts" />
      <main class="flex-1 px-6 py-6">
        <router-view />
      </main>
    </div>

    <ToastContainer />
  </div>
</template>
