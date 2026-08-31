<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import SideBadge from '../components/SideBadge.vue'
import StratFormModal from '../components/StratFormModal.vue'
import { useToast } from '../composables/useToast.js'
import {
  getStratsSummary,
  createStrat,
  updateStrat,
  deleteStrat,
  assignCategoryToStrat,
  removeCategoryFromStrat,
} from '../api/strats.js'
import { assignOperatorToStrat, removeOperatorFromStrat } from '../api/operators.js'
import { getCategories } from '../api/categories.js'
import { getOperators } from '../api/operators.js'
import { getMaps } from '../api/maps.js'

const router = useRouter()
const { addToast } = useToast()

const strats = ref([])
const categories = ref([])
const operators = ref([])
const maps = ref([])
const search = ref('')
const sideFilter = ref('all')
const loading = ref(false)
const modal = ref({ open: false, mode: 'create', strat: null })

const rows = computed(() => {
  return strats.value.filter((s) => {
    if (sideFilter.value !== 'all' && s.side !== sideFilter.value) return false
    if (search.value && !s.name.toLowerCase().includes(search.value.toLowerCase())) return false
    return true
  })
})

async function loadStrats() {
  loading.value = true
  try {
    const [summary, cats, ops, ms] = await Promise.all([
      getStratsSummary(),
      getCategories(),
      getOperators(),
      getMaps(),
    ])
    strats.value = summary || []
    categories.value = cats || []
    operators.value = ops || []
    maps.value = ms || []
  } catch (error) {
    addToast(error.message || 'Failed to load strats', 'error')
  } finally {
    loading.value = false
  }
}

function mapName(mapId) {
  const map = maps.value.find((m) => m.id === mapId)
  return map?.name || mapId
}

async function handleDelete(id) {
  if (!confirm('Are you sure you want to delete this strat?')) return
  try {
    await deleteStrat(id)
    addToast('Strat deleted', 'success')
    await loadStrats()
  } catch (error) {
    addToast(error.message || 'Failed to delete strat', 'error')
  }
}

async function handleSave(form) {
  try {
    const map = maps.value.find((m) => m.id === form.mapId)
    const mapName = map?.name || ''

    const baseData = {
      name: form.name,
      videoUrl: form.videoUrl || '',
      mapName,
      description: form.description,
    }

    let stratId
    if (modal.value.mode === 'create') {
      const result = await createStrat(baseData)
      stratId = result?.stratId
    } else {
      stratId = modal.value.strat.id
      await updateStrat(stratId, baseData)
    }

    if (modal.value.mode === 'edit') {
      const currentCategoryIds = modal.value.strat.categories?.map((c) => c.id) || []
      const currentOperatorIds = modal.value.strat.operators?.map((o) => o.id) || []

      const toRemoveCategories = currentCategoryIds.filter((id) => !form.categoryIds.includes(id))
      const toAddCategories = form.categoryIds.filter((id) => !currentCategoryIds.includes(id))
      const toRemoveOperators = currentOperatorIds.filter((id) => !form.operatorIds.includes(id))
      const toAddOperators = form.operatorIds.filter((id) => !currentOperatorIds.includes(id))

      for (const id of toRemoveCategories) {
        await removeCategoryFromStrat(stratId, id)
      }
      for (const id of toAddCategories) {
        await assignCategoryToStrat(stratId, id)
      }
      for (const id of toRemoveOperators) {
        await removeOperatorFromStrat(id, stratId)
      }
      for (const id of toAddOperators) {
        await assignOperatorToStrat(id, stratId)
      }
    } else {
      for (const id of form.categoryIds) {
        await assignCategoryToStrat(stratId, id)
      }
      for (const id of form.operatorIds) {
        await assignOperatorToStrat(id, stratId)
      }
    }

    addToast(modal.value.mode === 'create' ? 'Strat created' : 'Strat updated', 'success')
    modal.value = { open: false, mode: 'create', strat: null }
    await loadStrats()
  } catch (error) {
    addToast(error.message || 'Failed to save strat', 'error')
  }
}

function openCreateModal() {
  modal.value = { open: true, mode: 'create', strat: null }
}

function openEditModal(strat) {
  modal.value = { open: true, mode: 'edit', strat }
}

function viewStrat(strat) {
  router.push({ name: 'Strat Detail', params: { id: strat.id }, state: { stratName: strat.name } })
}

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

onMounted(() => {
  loadStrats()
})
</script>

<template>
  <div class="animate-fade-in space-y-5">
    <div class="flex items-center gap-3 flex-wrap">
      <div class="relative flex-1 min-w-[180px] max-w-xs">
        <svg class="w-3.5 h-3.5 text-[#9CA3AF] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          v-model="search"
          type="text"
          placeholder="Search strats…"
          class="w-full bg-[#1C1C1F] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] pl-8 pr-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
        />
      </div>
      <div class="flex items-center gap-1 bg-[#1C1C1F] border border-[#2A2A2E] rounded p-1">
        <button
          v-for="f in ['all', 'Attack', 'Defense']"
          :key="f"
          @click="sideFilter = f"
          class="px-3 py-1 text-xs font-medium rounded transition-colors"
          :class="sideFilter === f ? 'bg-[#DC2626] text-white' : 'text-[#9CA3AF] hover:text-white'"
        >
          {{ f === 'all' ? 'All' : f }}
        </button>
      </div>
      <div class="flex-1"></div>
      <button
        @click="openCreateModal"
        class="flex items-center gap-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
      >
        <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
        </svg>
        Add New Strat
      </button>
    </div>

    <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
      <div v-if="loading" class="py-16 text-center text-[#9CA3AF]">Loading…</div>
      <div v-else-if="rows.length === 0" class="flex flex-col items-center justify-center py-16 gap-3">
        <svg class="w-10 h-10 text-[#2A2A2E]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1">
          <path stroke-linecap="round" stroke-linejoin="round" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
        </svg>
        <p class="text-[#9CA3AF] text-sm">No strats found</p>
      </div>
      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-[#2A2A2E]">
            <th v-for="h in ['ID', 'Name', 'Side', 'Map', 'Categories', 'Operators', 'Actions']" :key="h" class="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5 whitespace-nowrap">
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(row, i) in rows"
            :key="row.id"
            class="table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors"
            :class="i % 2 !== 0 ? 'bg-[rgba(255,255,255,0.01)]' : ''"
          >
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono">STR-{{ row.id }}</span></td>
            <td class="px-5 py-3.5"><span class="text-white text-sm font-medium">{{ row.name }}</span></td>
            <td class="px-5 py-3.5"><SideBadge :side="row.side" /></td>
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-sm">{{ row.map.name }}</span></td>
            <td class="px-5 py-3.5">
              <div class="flex flex-wrap gap-1">
                <span v-for="c in row.categories.slice(0, 2)" :key="c.id" class="px-1.5 py-0.5 bg-[#141416] border border-[#2A2A2E] rounded text-[10px] text-[#9CA3AF] font-mono">{{ c.name }}</span>
                <span v-if="row.categories.length > 2" class="text-[#9CA3AF] text-xs">+{{ row.categories.length - 2 }}</span>
              </div>
            </td>
            <td class="px-5 py-3.5">
              <div class="flex flex-wrap gap-1">
                <span v-for="op in row.operators.slice(0, 2)" :key="op.id" class="px-1.5 py-0.5 bg-[#141416] border border-[#2A2A2E] rounded text-[10px] text-white font-mono">{{ op.name }}</span>
                <span v-if="row.operators.length > 2" class="text-[#9CA3AF] text-xs">+{{ row.operators.length - 2 }}</span>
              </div>
            </td>
            <td class="px-5 py-3.5">
              <div class="flex items-center gap-1.5">
                <button
                  @click="viewStrat(row)"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                  title="View"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                </button>
                <button
                  @click="openEditModal(row)"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                  title="Edit"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </button>
                <button
                  @click="handleDelete(row.id)"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all"
                  title="Delete"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <StratFormModal
      :open="modal.open"
      :mode="modal.mode"
      :strat="modal.strat"
      :maps="maps"
      :categories="categories"
      :operators="operators"
      @close="modal = { open: false, mode: 'create', strat: null }"
      @save="handleSave"
    />
  </div>
</template>
