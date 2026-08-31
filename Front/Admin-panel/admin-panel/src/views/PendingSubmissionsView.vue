<script setup>
import { ref, computed, onMounted } from 'vue'
import SideBadge from '../components/SideBadge.vue'
import StatusBadge from '../components/StatusBadge.vue'
import SubmissionDetailSlideOver from '../components/SubmissionDetailSlideOver.vue'
import { useToast } from '../composables/useToast.js'
import {
  getPendingStratSubmissions,
  getPendingCategorySubmissions,
  approveStratSubmission,
  rejectStratSubmission,
  approveCategorySubmission,
  rejectCategorySubmission,
} from '../api/submissions.js'
import { getCategories } from '../api/categories.js'
import { getOperators } from '../api/operators.js'
import { getMaps } from '../api/maps.js'

const { addToast } = useToast()

const activeTab = ref('strat')
const selected = ref(new Set())
const sideFilter = ref('all')
const search = ref('')
const detail = ref(null)

const stratSubmissions = ref([])
const categorySubmissions = ref([])
const categories = ref([])
const operators = ref([])
const maps = ref([])
const loading = ref(false)

const pendingStrats = computed(() => stratSubmissions.value)
const pendingCategories = computed(() => categorySubmissions.value)

const rows = computed(() => {
  const source = activeTab.value === 'strat' ? pendingStrats.value : pendingCategories.value
  return source.filter((s) => {
    if (sideFilter.value !== 'all' && s.side !== sideFilter.value) return false
    if (search.value && !s.name.toLowerCase().includes(search.value.toLowerCase())) return false
    return true
  })
})

const allIds = computed(() => rows.value.map((r) => r.id))
const allChecked = computed(() => allIds.value.length > 0 && allIds.value.every((id) => selected.value.has(id)))

function enrichSubmissions(list, type) {
  return list.map((s) => ({
    ...s,
    type,
    side: type === 'strat' ? deriveStratSide(s) : s.side,
    mapId: type === 'strat' ? s.mapId : null,
  }))
}

function deriveStratSide(submission) {
  if (!submission.categoryIds?.length) return 'Attack'
  const cat = categories.value.find((c) => c.id === submission.categoryIds[0])
  return cat?.side || 'Attack'
}

async function loadData() {
  loading.value = true
  try {
    const [strats, cats, catsList, opsList, mapsList] = await Promise.all([
      getPendingStratSubmissions(),
      getPendingCategorySubmissions(),
      getCategories(),
      getOperators(),
      getMaps(),
    ])
    categories.value = catsList || []
    operators.value = opsList || []
    maps.value = mapsList || []
    stratSubmissions.value = enrichSubmissions(strats || [], 'strat')
    categorySubmissions.value = enrichSubmissions(cats || [], 'category')
  } catch (error) {
    addToast(error.message || 'Failed to load submissions', 'error')
  } finally {
    loading.value = false
  }
}

function toggleAll() {
  const newSelected = new Set(selected.value)
  if (allChecked.value) {
    allIds.value.forEach((id) => newSelected.delete(id))
  } else {
    allIds.value.forEach((id) => newSelected.add(id))
  }
  selected.value = newSelected
}

function toggleRow(id) {
  const newSelected = new Set(selected.value)
  newSelected.has(id) ? newSelected.delete(id) : newSelected.add(id)
  selected.value = newSelected
}

async function approve(id) {
  try {
    if (activeTab.value === 'strat') {
      await approveStratSubmission(id)
    } else {
      await approveCategorySubmission(id)
    }
    addToast('Submission approved', 'success')
    await loadData()
  } catch (error) {
    addToast(error.message || 'Failed to approve', 'error')
  }
}

async function reject(id) {
  try {
    if (activeTab.value === 'strat') {
      await rejectStratSubmission(id)
    } else {
      await rejectCategorySubmission(id)
    }
    addToast('Submission rejected', 'success')
    await loadData()
  } catch (error) {
    addToast(error.message || 'Failed to reject', 'error')
  }
}

async function bulkApprove() {
  for (const id of selected.value) {
    await approve(id)
  }
  selected.value = new Set()
}

async function bulkReject() {
  for (const id of selected.value) {
    await reject(id)
  }
  selected.value = new Set()
}

function switchTab(tab) {
  activeTab.value = tab
  selected.value = new Set()
  search.value = ''
  sideFilter.value = 'all'
}

function formatDate(iso) {
  return new Date(iso).toLocaleString('en-US', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })
}

function mapName(mapId) {
  if (!mapId) return '—'
  const map = maps.value.find((m) => m.id === mapId)
  return map?.name || mapId
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="animate-fade-in space-y-5">
    <div class="flex items-center border-b border-[#2A2A2E] gap-1">
      <button
        v-for="[tab, label, count] in [['strat', 'Strat Submissions', pendingStrats.length], ['category', 'Category Submissions', pendingCategories.length]]"
        :key="tab"
        @click="switchTab(tab)"
        class="flex items-center gap-2 px-4 py-2.5 text-sm font-medium border-b-2 -mb-px transition-colors"
        :class="activeTab === tab
          ? 'border-[#DC2626] text-white'
          : 'border-transparent text-[#9CA3AF] hover:text-white'"
      >
        {{ label }}
        <span
          v-if="count > 0"
          class="text-[10px] font-bold font-mono px-1.5 py-0.5 rounded-full"
          :class="activeTab === tab ? 'bg-[#DC2626] text-white' : 'bg-[#1C1C1F] text-[#9CA3AF] border border-[#2A2A2E]'"
        >
          {{ count }}
        </span>
      </button>
    </div>

    <div class="flex items-center gap-3 flex-wrap">
      <div class="relative flex-1 min-w-[200px] max-w-xs">
        <svg class="w-3.5 h-3.5 text-[#9CA3AF] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          v-model="search"
          type="text"
          placeholder="Filter by name…"
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
          {{ f === 'all' ? 'All Sides' : f }}
        </button>
      </div>

      <div class="flex-1"></div>

      <div v-if="selected.size > 0" class="flex items-center gap-2 animate-fade-in">
        <span class="text-[#9CA3AF] text-xs font-mono">{{ selected.size }} selected</span>
        <button @click="bulkApprove" class="flex items-center gap-1.5 px-3 py-1.5 bg-[#16A34A] hover:bg-green-500 text-white text-xs font-semibold rounded transition-colors">
          <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg>
          Approve All
        </button>
        <button @click="bulkReject" class="flex items-center gap-1.5 px-3 py-1.5 bg-[#DC2626] hover:bg-[#EF4444] text-white text-xs font-semibold rounded transition-colors">
          <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="3"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>
          Reject All
        </button>
        <button @click="selected = new Set()" class="text-[#9CA3AF] hover:text-white text-xs transition-colors">Clear</button>
      </div>
    </div>

    <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
      <div v-if="rows.length === 0" class="flex flex-col items-center justify-center py-20 gap-4">
        <div class="w-14 h-14 rounded-full bg-[#141416] border border-[#2A2A2E] flex items-center justify-center">
          <svg class="w-6 h-6 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
        </div>
        <div class="text-center">
          <p class="text-white font-medium text-sm">No pending submissions</p>
          <p class="text-[#9CA3AF] text-xs mt-1">All caught up — nothing awaiting review here.</p>
        </div>
      </div>

      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-[#2A2A2E]">
            <th class="px-4 py-2.5 w-10">
              <input
                type="checkbox"
                :checked="allChecked"
                @change="toggleAll"
                class="w-3.5 h-3.5 accent-[#DC2626] cursor-pointer"
              />
            </th>
            <th v-for="h in ['ID', 'Name', 'Side', 'Map', 'Submitted At', 'Status', 'Actions']" :key="h" class="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-4 py-2.5 whitespace-nowrap">
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(row, i) in rows"
            :key="row.id"
            class="table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors"
            :class="selected.has(row.id) ? 'bg-[rgba(220,38,38,0.06)]' : i % 2 !== 0 ? 'bg-[rgba(255,255,255,0.01)]' : ''"
          >
            <td class="px-4 py-3.5">
              <input
                type="checkbox"
                :checked="selected.has(row.id)"
                @change="toggleRow(row.id)"
                class="w-3.5 h-3.5 accent-[#DC2626] cursor-pointer"
              />
            </td>
            <td class="px-4 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono">SUB-{{ activeTab === 'strat' ? '' : 'C' }}{{ row.id }}</span></td>
            <td class="px-4 py-3.5"><span class="text-white text-sm font-medium">{{ row.name }}</span></td>
            <td class="px-4 py-3.5"><SideBadge :side="row.side" /></td>
            <td class="px-4 py-3.5"><span class="text-[#9CA3AF] text-sm">{{ mapName(row.mapId) }}</span></td>
            <td class="px-4 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">{{ formatDate(row.submittedAt) }}</span></td>
            <td class="px-4 py-3.5"><StatusBadge status="pending" /></td>
            <td class="px-4 py-3.5">
              <div class="flex items-center gap-1.5">
                <button
                  @click="detail = row"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                  title="View"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                </button>
                <button
                  @click="approve(row.id)"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#16A34A] hover:text-white hover:bg-[#16A34A] border border-transparent hover:border-[#16A34A] transition-all"
                  title="Approve"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
                  </svg>
                </button>
                <button
                  @click="reject(row.id)"
                  class="w-7 h-7 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all"
                  title="Reject"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <SubmissionDetailSlideOver
      :submission="detail"
      :categories="categories"
      :operators="operators"
      :maps="maps"
      @close="detail = null"
      @approve="approve"
      @reject="reject"
    />
  </div>
</template>
