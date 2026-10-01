<script setup>
import { ref, watch } from 'vue'
import { getMap } from '../api/maps.js'
import { useToast } from '../composables/useToast.js'

const props = defineProps({
  open: Boolean,
  mode: {
    type: String,
    default: 'create',
  },
  map: Object,
})

const emit = defineEmits(['close', 'save'])

const { addToast } = useToast()

const DEFAULT_BOMBSITES = ['Site A', 'Site B', 'Site C', 'Site D', 'All']

const form = ref({ name: '' })
const bombsites = ref([])
const originalBombsites = ref([])
const loading = ref(false)
let rowKey = 0

watch(() => props.open, async (isOpen) => {
  if (!isOpen) return
  if (props.mode === 'edit' && props.map) {
    form.value = { name: props.map.name }
    loading.value = true
    try {
      const detail = await getMap(props.map.id)
      originalBombsites.value = detail?.bombsites || []
      bombsites.value = originalBombsites.value.map((b) => ({ key: ++rowKey, id: b.id, name: b.name }))
    } catch (error) {
      addToast(error.message || 'Failed to load bombsites', 'error')
      originalBombsites.value = []
      bombsites.value = []
    } finally {
      loading.value = false
    }
  } else {
    form.value = { name: '' }
    originalBombsites.value = []
    bombsites.value = DEFAULT_BOMBSITES.map((name) => ({ key: ++rowKey, id: null, name }))
  }
})

function addBombsite() {
  bombsites.value.push({ key: ++rowKey, id: null, name: '' })
}

function removeBombsite(key) {
  bombsites.value = bombsites.value.filter((b) => b.key !== key)
}

function handleSubmit() {
  emit('save', {
    name: form.value.name,
    bombsites: bombsites.value
      .map((b) => ({ id: b.id, name: b.name.trim() }))
      .filter((b) => b.name),
    originalBombsites: originalBombsites.value,
  })
}

function handleClose() {
  emit('close')
}
</script>

<template>
  <div v-if="open">
    <div class="fixed inset-0 z-[100] bg-black/75 backdrop-blur-[2px]" @click="handleClose"></div>
    <div class="fixed inset-0 z-[110] flex items-center justify-center p-6 pointer-events-none">
      <div
        class="bg-[#141416] border border-[#2A2A2E] rounded shadow-2xl w-full max-w-[480px] max-h-[85vh] flex flex-col animate-slide-up pointer-events-auto"
        @click.stop
      >
        <div class="px-6 py-4 border-b border-[#2A2A2E] flex items-center justify-between shrink-0">
          <h2 class="text-white font-bold text-base">{{ mode === 'create' ? 'Add Map' : 'Edit Map' }}</h2>
          <button @click="handleClose" class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="flex-1 min-h-0 overflow-y-auto px-6 py-5 space-y-5">
          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Map Name <span class="text-[#DC2626]">*</span></label>
            <input
              v-model="form.name"
              required
              type="text"
              placeholder="e.g. Clubhouse"
              class="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
            />
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Bombsites</label>
            <div v-if="loading" class="py-4 text-center text-[#9CA3AF] text-sm">Loading…</div>
            <div v-else class="space-y-2">
              <div v-for="b in bombsites" :key="b.key" class="flex items-center gap-2">
                <input
                  v-model="b.name"
                  type="text"
                  placeholder="Bombsite name"
                  class="flex-1 bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
                />
                <button
                  type="button"
                  @click="removeBombsite(b.key)"
                  class="w-8 h-8 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all shrink-0"
                  title="Remove"
                >
                  <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
              <button
                type="button"
                @click="addBombsite"
                class="flex items-center gap-1.5 px-3 py-1.5 bg-[#1C1C1F] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-xs font-medium rounded border border-[#2A2A2E] transition-colors"
              >
                <svg class="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
                </svg>
                Add Bombsite
              </button>
            </div>
          </div>
        </form>

        <div class="px-6 py-4 border-t border-[#2A2A2E] flex items-center justify-end gap-3 shrink-0">
          <button
            type="button"
            @click="handleClose"
            class="px-4 py-2 bg-[#1C1C1F] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-sm font-medium rounded border border-[#2A2A2E] transition-colors"
          >
            Cancel
          </button>
          <button
            type="button"
            @click="handleSubmit"
            :disabled="loading"
            class="px-5 py-2 bg-[#DC2626] hover:bg-[#EF4444] disabled:opacity-50 text-white text-sm font-semibold rounded transition-colors"
          >
            {{ mode === 'create' ? 'Create' : 'Save' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
