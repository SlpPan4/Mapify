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
      <Header @refresh="loadPendingCounts" />
      <main class="flex-1 px-6 py-6">
        <router-view />
      </main>
    </div>

    <ToastContainer />
  </div>
</template>
