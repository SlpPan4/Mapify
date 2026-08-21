<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import SideBadge from '../components/SideBadge.vue'
import StratFormModal from '../components/StratFormModal.vue'
import VideoPlayer from '../components/VideoPlayer.vue'
import { useToast } from '../composables/useToast.js'
import {
  getStratById,
  updateStrat,
  deleteStrat,
  assignCategoryToStrat,
  removeCategoryFromStrat,
} from '../api/strats.js'
import { assignOperatorToStrat, removeOperatorFromStrat } from '../api/operators.js'
import { getCategories } from '../api/categories.js'
import { getOperators } from '../api/operators.js'
import { getMaps } from '../api/maps.js'

const route = useRoute()
const router = useRouter()
const { addToast } = useToast()

const strat = ref(null)
const categories = ref([])
const operators = ref([])
const maps = ref([])
const loading = ref(false)
const editOpen = ref(false)
const confirmDelete = ref(false)

async function loadStrat() {
  loading.value = true
  try {
    const [detail, cats, ops, ms] = await Promise.all([
      getStratById(route.params.id),
      getCategories(),
      getOperators(),
      getMaps(),
    ])
    strat.value = detail
    categories.value = cats || []
    operators.value = ops || []
    maps.value = ms || []
  } catch (error) {
    addToast(error.message || 'Failed to load strat', 'error')
  } finally {
    loading.value = false
  }
}

async function handleSave(form) {
  try {
    const map = maps.value.find((m) => m.id === form.mapId)
    await updateStrat(strat.value.id, {
      name: form.name,
      videoUrl: form.videoUrl || '',
      mapName: map?.name || '',
      description: form.description,
    })

    const currentCategoryIds = strat.value.categories?.map((c) => c.id) || []
    const currentOperatorIds = strat.value.operators?.map((o) => o.id) || []

    const toRemoveCategories = currentCategoryIds.filter((id) => !form.categoryIds.includes(id))
    const toAddCategories = form.categoryIds.filter((id) => !currentCategoryIds.includes(id))
    const toRemoveOperators = currentOperatorIds.filter((id) => !form.operatorIds.includes(id))
    const toAddOperators = form.operatorIds.filter((id) => !currentOperatorIds.includes(id))

    for (const id of toRemoveCategories) {
      await removeCategoryFromStrat(strat.value.id, id)
    }
    for (const id of toAddCategories) {
      await assignCategoryToStrat(strat.value.id, id)
    }
    for (const id of toRemoveOperators) {
      await removeOperatorFromStrat(id, strat.value.id)
    }
    for (const id of toAddOperators) {
      await assignOperatorToStrat(id, strat.value.id)
    }

    addToast('Strat updated', 'success')
    editOpen.value = false
    await loadStrat()
  } catch (error) {
    addToast(error.message || 'Failed to update strat', 'error')
  }
}

async function handleDelete() {
  try {
    await deleteStrat(strat.value.id)
    addToast('Strat deleted', 'success')
    router.push('/strats')
  } catch (error) {
    addToast(error.message || 'Failed to delete strat', 'error')
  }
}

function formatDate(iso) {
  const d = new Date(iso)
  return {
    date: d.toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }),
    time: d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }),
  }
}

onMounted(() => {
  loadStrat()
})
</script>

<template>
  <div v-if="loading" class="animate-fade-in text-center text-[#9CA3AF] py-20">Loading…</div>
  <div v-else-if="!strat" class="animate-fade-in text-center text-[#9CA3AF] py-20">Strat not found</div>
  <div v-else class="animate-fade-in max-w-4xl">
    <div class="flex items-center gap-2 mb-6">
      <button
        @click="router.push('/strats')"
        class="flex items-center gap-1.5 text-[#9CA3AF] hover:text-white text-sm transition-colors group"
      >
        <svg class="w-3.5 h-3.5 transition-transform group-hover:-translate-x-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M15 19l-7-7 7-7" />
        </svg>
        Strats
      </button>
      <span class="text-[#2A2A2E]">/</span>
      <span class="text-white text-sm font-medium truncate">{{ strat.name }}</span>
    </div>

    <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden mb-5">
      <div class="h-[3px] bg-gradient-to-r from-[#DC2626] via-[#DC2626] to-transparent" />
      <div class="px-6 py-5 flex items-start justify-between gap-6">
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-3 mb-2 flex-wrap">
            <span class="text-[#9CA3AF] text-xs font-mono">STR-{{ strat.id }}</span>
            <SideBadge :side="strat.categories[0]?.side || strat.operators[0]?.side || 'Attack'" />
          </div>
          <h1 class="text-white font-bold text-2xl leading-tight mb-3">{{ strat.name }}</h1>
          <div class="flex items-center gap-4 text-sm text-[#9CA3AF] flex-wrap">
            <div class="flex items-center gap-1.5">
              <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span>{{ strat.map.name }}</span>
            </div>
          </div>
        </div>

        <div class="flex items-center gap-2 shrink-0">
          <button
            @click="editOpen = true"
            class="flex items-center gap-2 px-3.5 py-2 bg-[#141416] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-sm font-medium rounded border border-[#2A2A2E] transition-colors"
          >
            <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
            Edit
          </button>
          <div v-if="confirmDelete" class="flex items-center gap-2 animate-fade-in">
            <span class="text-[#EF4444] text-xs font-medium whitespace-nowrap">Confirm delete?</span>
            <button @click="handleDelete" class="px-3 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-xs font-semibold rounded transition-colors">Yes, delete</button>
            <button @click="confirmDelete = false" class="px-3 py-2 bg-[#141416] text-[#9CA3AF] hover:text-white text-xs rounded border border-[#2A2A2E] transition-colors">Cancel</button>
          </div>
          <button
            v-else
            @click="confirmDelete = true"
            class="flex items-center gap-2 px-3.5 py-2 bg-[#141416] hover:bg-[rgba(220,38,38,0.1)] text-[#DC2626] text-sm font-medium rounded border border-[#2A2A2E] hover:border-[#DC2626] transition-colors"
          >
            <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            Delete
          </button>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-[1fr_300px] gap-5">
      <div class="space-y-5">
        <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
          <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Description</div>
          <p class="text-white/90 text-sm leading-relaxed">{{ strat.description || 'No description provided.' }}</p>
        </div>

        <div v-if="strat.videoUrl" class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
          <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Video Reference</div>
          <VideoPlayer :url="strat.videoUrl" />
          <a
            :href="strat.videoUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="flex items-center gap-2 text-[#60A5FA] text-sm hover:text-blue-300 transition-colors mt-3"
          >
            <svg class="w-3.5 h-3.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
            </svg>
            <span class="truncate">{{ strat.videoUrl }}</span>
          </a>
        </div>
      </div>

      <div class="space-y-5">
        <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
          <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Operators ({{ strat.operators.length }})</div>
          <div v-if="strat.operators.length > 0" class="flex flex-col gap-2">
            <div
              v-for="op in strat.operators"
              :key="op.id"
              class="flex items-center gap-3 px-3 py-2.5 rounded border"
              :class="op.side === 'Attack'
                ? 'bg-[rgba(220,38,38,0.06)] border-[rgba(220,38,38,0.15)]'
                : 'bg-[rgba(37,99,235,0.06)] border-[rgba(37,99,235,0.15)]'"
            >
              <div
                class="w-7 h-7 rounded flex items-center justify-center text-[10px] font-bold shrink-0"
                :class="op.side === 'Attack' ? 'bg-[rgba(220,38,38,0.15)] text-[#EF4444]' : 'bg-[rgba(37,99,235,0.15)] text-[#60A5FA]'"
              >
                {{ op.name.slice(0, 2).toUpperCase() }}
              </div>
              <span class="text-sm font-semibold" :class="op.side === 'Attack' ? 'text-[#EF4444]' : 'text-[#60A5FA]'">{{ op.name }}</span>
              <span class="ml-auto text-[10px] font-mono" :class="op.side === 'Attack' ? 'text-[rgba(220,38,38,0.6)]' : 'text-[rgba(37,99,235,0.6)]'">{{ op.side.toUpperCase() }}</span>
            </div>
          </div>
          <p v-else class="text-[#9CA3AF] text-sm italic">No operators assigned.</p>
        </div>

        <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
          <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Categories ({{ strat.categories.length }})</div>
          <div v-if="strat.categories.length > 0" class="flex flex-wrap gap-2">
            <span
              v-for="c in strat.categories"
              :key="c.id"
              class="px-3 py-1.5 bg-[#141416] border border-[#2A2A2E] rounded text-xs text-[#9CA3AF] font-mono hover:border-[#DC2626] hover:text-white transition-colors cursor-default"
            >
              {{ c.name }}
            </span>
          </div>
          <p v-else class="text-[#9CA3AF] text-sm italic">No categories assigned.</p>
        </div>

        <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 space-y-4">
          <div>
            <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Map</div>
            <div class="flex items-center gap-2">
              <svg class="w-4 h-4 text-[#DC2626]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <span class="text-white font-medium text-sm">{{ strat.map.name }}</span>
            </div>
          </div>
          <div class="h-px bg-[#2A2A2E]" />
          <div>
            <div class="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]" style="font-family: 'Rajdhani', sans-serif; font-weight: 600;">Record ID</div>
            <span class="text-[#9CA3AF] font-mono text-sm">STR-{{ strat.id }}</span>
          </div>
        </div>
      </div>
    </div>

    <StratFormModal
      :open="editOpen"
      mode="edit"
      :strat="strat"
      :maps="maps"
      :categories="categories"
      :operators="operators"
      @close="editOpen = false"
      @save="handleSave"
    />
  </div>
</template>
