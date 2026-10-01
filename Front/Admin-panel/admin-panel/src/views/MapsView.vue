<script setup>
import { ref, computed, onMounted } from 'vue'
import MapFormModal from '../components/MapFormModal.vue'
import { useToast } from '../composables/useToast.js'
import { getMaps, createMap, updateMap, deleteMap } from '../api/maps.js'
import { createBombsite, updateBombsite, deleteBombsite } from '../api/bombsites.js'

const { addToast } = useToast()

const maps = ref([])
const search = ref('')
const loading = ref(false)
const modal = ref({ open: false, mode: 'create', map: null })

const rows = computed(() => {
  return maps.value.filter((m) => {
    if (search.value && !m.name.toLowerCase().includes(search.value.toLowerCase())) return false
    return true
  })
})

async function loadMaps() {
  loading.value = true
  try {
    const list = await getMaps()
    maps.value = list || []
  } catch (error) {
    addToast(error.message || 'Failed to load maps', 'error')
  } finally {
    loading.value = false
  }
}

async function handleDelete(id) {
  if (!confirm('Are you sure you want to delete this map?')) return
  try {
    await deleteMap(id)
    addToast('Map deleted', 'success')
    await loadMaps()
  } catch (error) {
    addToast(error.message || 'Failed to delete map', 'error')
  }
}

async function handleSave(form) {
  try {
    let mapId
    if (modal.value.mode === 'create') {
      const created = await createMap(form.name)
      mapId = created?.id ?? created?.mapId
    } else {
      mapId = modal.value.map.id
      await updateMap(mapId, form.name)
    }

    const original = form.originalBombsites || []
    const current = form.bombsites || []

    for (const b of original) {
      if (!current.some((c) => c.id === b.id)) {
        await deleteBombsite(mapId, b.id)
      }
    }
    for (const b of current) {
      if (!b.id) {
        await createBombsite(mapId, b.name)
      } else {
        const orig = original.find((o) => o.id === b.id)
        if (orig && orig.name !== b.name) {
          await updateBombsite(mapId, b.id, b.name)
        }
      }
    }

    addToast(modal.value.mode === 'create' ? 'Map created' : 'Map updated', 'success')
    modal.value = { open: false, mode: 'create', map: null }
    await loadMaps()
  } catch (error) {
    addToast(error.message || 'Failed to save map', 'error')
  }
}

onMounted(() => {
  loadMaps()
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
          placeholder="Search maps…"
          class="w-full bg-[#1C1C1F] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] pl-8 pr-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
        />
      </div>
      <div class="flex-1"></div>
      <button
        @click="modal = { open: true, mode: 'create', map: null }"
        class="flex items-center gap-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
      >
        <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
        </svg>
        Add Map
      </button>
    </div>

    <div class="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
      <div v-if="loading" class="py-16 text-center text-[#9CA3AF]">Loading…</div>
      <div v-else-if="rows.length === 0" class="flex flex-col items-center justify-center py-16 gap-3">
        <svg class="w-10 h-10 text-[#2A2A2E]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1">
          <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
          <path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
        <p class="text-[#9CA3AF] text-sm">No maps found</p>
      </div>
      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-[#2A2A2E]">
            <th v-for="h in ['ID', 'Name', 'Actions']" :key="h" class="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5 whitespace-nowrap">
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
            <td class="px-5 py-3.5"><span class="text-[#9CA3AF] text-xs font-mono">MAP-{{ row.id }}</span></td>
            <td class="px-5 py-3.5"><span class="text-white text-sm font-semibold">{{ row.name }}</span></td>
            <td class="px-5 py-3.5">
              <div class="flex items-center gap-1.5">
                <button
                  @click="modal = { open: true, mode: 'edit', map: row }"
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

    <MapFormModal
      :open="modal.open"
      :mode="modal.mode"
      :map="modal.map"
      @close="modal = { open: false, mode: 'create', map: null }"
      @save="handleSave"
    />
  </div>
</template>
