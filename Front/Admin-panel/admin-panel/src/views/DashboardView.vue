<script setup>
import { ref, onMounted, computed } from 'vue'
import SideBadge from '../components/SideBadge.vue'
import StatusBadge from '../components/StatusBadge.vue'
import { getPendingStratSubmissions, getPendingCategorySubmissions } from '../api/submissions.js'
import { getStrats } from '../api/strats.js'
import { getCategories } from '../api/categories.js'
import { getMaps } from '../api/maps.js'

const stats = ref({
  pendingStrats: 0,
  pendingCategories: 0,
  totalStrats: 0,
  totalCategories: 0,
})
const recentActivity = ref([])
const loading = ref(false)

const systemStatus = [
  { label: 'API Status', value: 'Operational', ok: true },
  { label: 'Database', value: 'Healthy', ok: true },
  { label: 'Last Sync', value: 'Just now', ok: true },
]

function formatTime(iso) {
  const d = new Date(iso)
  return d.toLocaleString('en-US', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })
}

async function loadDashboard() {
  loading.value = true
  try {
    const [pendingStrats, pendingCategories, strats, categories, mapList] = await Promise.all([
      getPendingStratSubmissions(),
      getPendingCategorySubmissions(),
      getStrats(),
      getCategories(),
      getMaps(),
    ])

    maps.value = mapList || []

    stats.value = {
      pendingStrats: pendingStrats?.length || 0,
      pendingCategories: pendingCategories?.length || 0,
      totalStrats: strats?.length || 0,
      totalCategories: categories?.length || 0,
    }

    const stratSubs = (pendingStrats || []).map((s) => ({
      id: `SUB-${s.id}`,
      name: s.name,
      type: 'Strat',
      side: deriveSide(s, categories || []),
      map: s.mapId ? findMapName(s.mapId) : '—',
      submittedBy: 'User',
      submittedAt: s.submittedAt,
      status: 'pending',
    }))

    const catSubs = (pendingCategories || []).map((c) => ({
      id: `SUB-C${c.id}`,
      name: c.name,
      type: 'Category',
      side: c.side,
      map: '—',
      submittedBy: 'User',
      submittedAt: c.submittedAt,
      status: 'pending',
    }))

    recentActivity.value = [...stratSubs, ...catSubs]
      .sort((a, b) => new Date(b.submittedAt) - new Date(a.submittedAt))
      .slice(0, 6)
  } catch (error) {
    console.error('Failed to load dashboard:', error)
  } finally {
    loading.value = false
  }
}

function deriveSide(submission, categories) {
  if (!submission.categoryIds?.length) return 'Attack'
  const cat = categories.find((c) => c.id === submission.categoryIds[0])
  return cat?.side || 'Attack'
}

const maps = ref([])

function findMapName(mapId) {
  const map = maps.value.find((m) => m.id === mapId)
  return map?.name || mapId
}

onMounted(() => {
  loadDashboard()
})
</script>

<template>
  <div class="animate-fade-in space-y-6">
    <div class="grid grid-cols-4 gap-4">
      <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 flex flex-col gap-3 glow-red-hover transition-all duration-200 group cursor-default">
        <span class="text-[#9CA3AF] text-xs font-medium tracking-wider uppercase">Pending Strats</span>
        <div class="flex items-end gap-2">
          <span class="text-4xl font-bold tabular-nums tracking-tight text-[#DC2626] group-hover:drop-shadow-[0_0_12px_rgba(220,38,38,0.5)] transition-all duration-200">
            {{ stats.pendingStrats.toLocaleString() }}
          </span>
        </div>
        <span class="text-[#9CA3AF] text-[11px] font-mono">Awaiting review</span>
      </div>

      <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 flex flex-col gap-3 glow-red-hover transition-all duration-200 group cursor-default">
        <span class="text-[#9CA3AF] text-xs font-medium tracking-wider uppercase">Pending Categories</span>
        <div class="flex items-end gap-2">
          <span class="text-4xl font-bold tabular-nums tracking-tight text-[#DC2626] group-hover:drop-shadow-[0_0_12px_rgba(220,38,38,0.5)] transition-all duration-200">
            {{ stats.pendingCategories.toLocaleString() }}
          </span>
        </div>
        <span class="text-[#9CA3AF] text-[11px] font-mono">Awaiting review</span>
      </div>

      <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 flex flex-col gap-3 glow-red-hover transition-all duration-200 group cursor-default">
        <span class="text-[#9CA3AF] text-xs font-medium tracking-wider uppercase">Total Strats</span>
        <div class="flex items-end gap-2">
          <span class="text-4xl font-bold tabular-nums tracking-tight text-white transition-all duration-200">
            {{ stats.totalStrats.toLocaleString() }}
          </span>
        </div>
        <span class="text-[#9CA3AF] text-[11px] font-mono">All time published</span>
      </div>

      <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 flex flex-col gap-3 glow-red-hover transition-all duration-200 group cursor-default">
        <span class="text-[#9CA3AF] text-xs font-medium tracking-wider uppercase">Total Categories</span>
        <div class="flex items-end gap-2">
          <span class="text-4xl font-bold tabular-nums tracking-tight text-white transition-all duration-200">
            {{ stats.totalCategories.toLocaleString() }}
          </span>
        </div>
        <span class="text-[#9CA3AF] text-[11px] font-mono">Active categories</span>
      </div>
    </div>

    <div class="flex items-center gap-3">
      <span class="text-[#9CA3AF] text-xs font-mono tracking-wider uppercase">Quick Actions</span>
      <div class="flex-1 h-px bg-[#2A2A2E]" />
      <router-link
        to="/pending"
        class="flex items-center gap-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
      >
        <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
        </svg>
        Review Pending
      </router-link>
    </div>

    <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
      <div class="px-5 py-3.5 border-b border-[#2A2A2E] flex items-center justify-between">
        <h2 class="text-white font-semibold text-sm">Recent Activity</h2>
        <span class="text-[#9CA3AF] text-[11px] font-mono">{{ recentActivity.length }} entries</span>
      </div>
      <table class="w-full">
        <thead>
          <tr class="border-b border-[#2A2A2E]">
            <th v-for="h in ['ID', 'Name', 'Type', 'Side', 'Map', 'Submitted By', 'Date', 'Status']" :key="h" class="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5">
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(row, i) in recentActivity"
            :key="row.id"
            class="table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors"
            :class="i % 2 !== 0 ? 'bg-[rgba(255,255,255,0.01)]' : ''"
          >
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono">{{ row.id }}</span></td>
            <td class="px-5 py-3.5"><span class="text-white text-sm font-medium">{{ row.name }}</span></td>
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono">{{ row.type }}</span></td>
            <td class="px-5 py-3.5"><SideBadge :side="row.side" /></td>
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-sm">{{ row.map }}</span></td>
            <td class="px-5 py-3.5"><span class="text-white text-sm">{{ row.submittedBy }}</span></td>
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">{{ formatTime(row.submittedAt) }}</span></td>
            <td class="px-5 py-3.5"><StatusBadge :status="row.status" /></td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="grid grid-cols-3 gap-4">
      <div v-for="s in systemStatus" :key="s.label" class="bg-[#1C1C1F] border border-[#2A2A2E] rounded px-4 py-3 flex items-center justify-between">
        <span class="text-[#9CA3AF] text-xs font-mono uppercase tracking-wider">{{ s.label }}</span>
        <div class="flex items-center gap-2">
          <div class="w-1.5 h-1.5 rounded-full" :class="s.ok ? 'bg-[#16A34A]' : 'bg-[#DC2626]'" />
          <span class="text-xs font-medium" :class="s.ok ? 'text-[#4ADE80]' : 'text-[#EF4444]'">{{ s.value }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
