<script setup>
import { ref, watch, computed } from 'vue'

const props = defineProps({
  open: Boolean,
  mode: {
    type: String,
    default: 'create',
  },
  strat: Object,
  maps: Array,
  categories: Array,
  operators: Array,
})

const emit = defineEmits(['close', 'save'])

const defaultForm = {
  name: '',
  side: 'Attack',
  mapId: null,
  description: '',
  videoUrl: '',
  categoryIds: [],
  operatorIds: [],
}

const form = ref({ ...defaultForm })

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    if (props.strat) {
      form.value = {
        name: props.strat.name || '',
        side: props.strat.side || 'Attack',
        mapId: props.strat.map?.id ?? props.strat.mapId ?? props.maps[0]?.id ?? null,
        description: props.strat.description || '',
        videoUrl: props.strat.videoUrl || '',
        categoryIds: props.strat.categories?.map((c) => c.id) || [],
        operatorIds: props.strat.operators?.map((o) => o.id) || [],
      }
    } else {
      form.value = {
        ...defaultForm,
        mapId: props.maps[0]?.id || null,
      }
    }
  }
})

const filteredCategories = computed(() =>
  props.categories?.filter((c) => c.side === form.value.side) || []
)

const filteredOperators = computed(() =>
  props.operators?.filter((o) => o.side === form.value.side) || []
)

function toggleCategory(id) {
  const set = new Set(form.value.categoryIds)
  set.has(id) ? set.delete(id) : set.add(id)
  form.value.categoryIds = Array.from(set)
}

function toggleOperator(id) {
  const set = new Set(form.value.operatorIds)
  set.has(id) ? set.delete(id) : set.add(id)
  form.value.operatorIds = Array.from(set)
}

function handleSubmit() {
  emit('save', { ...form.value })
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
        class="bg-[#141416] border border-[#2A2A2E] rounded shadow-2xl w-full max-w-[600px] max-h-[85vh] flex flex-col animate-slide-up pointer-events-auto"
        @click.stop
      >
        <div class="px-6 py-4 border-b border-[#2A2A2E] flex items-center justify-between shrink-0">
          <div>
            <h2 class="text-white font-bold text-base">{{ mode === 'create' ? 'Create Strat' : 'Edit Strat' }}</h2>
            <p v-if="strat?.id" class="text-[#9CA3AF] text-xs font-mono mt-0.5">STR-{{ strat.id }}</p>
          </div>
          <button @click="handleClose" class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="flex-1 overflow-y-auto px-6 py-5 space-y-5">
          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Strat Name <span class="text-[#DC2626]">*</span></label>
            <input
              v-model="form.name"
              required
              type="text"
              placeholder="e.g. Clubhouse Garage Double Breach"
              class="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
            />
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Side <span class="text-[#DC2626]">*</span></label>
            <div class="flex gap-2">
              <button
                v-for="s in ['Attack', 'Defense']"
                :key="s"
                type="button"
                @click="form.side = s; form.operatorIds = []"
                class="flex-1 py-2 rounded text-sm font-semibold border transition-colors"
                :class="form.side === s
                  ? s === 'Attack'
                    ? 'bg-[rgba(220,38,38,0.15)] border-[#DC2626] text-[#EF4444]'
                    : 'bg-[rgba(37,99,235,0.15)] border-[#2563EB] text-[#60A5FA]'
                  : 'bg-[#0B0B0C] border-[#2A2A2E] text-[#9CA3AF] hover:text-white'"
              >
                {{ s === 'Attack' ? '⚔' : '🛡' }} {{ s }}
              </button>
            </div>
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Map <span class="text-[#DC2626]">*</span></label>
            <select
              v-model="form.mapId"
              required
              class="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
            >
              <option v-for="m in maps" :key="m.id" :value="m.id">{{ m.name }}</option>
            </select>
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Description</label>
            <textarea
              v-model="form.description"
              rows="3"
              placeholder="Describe the strategy…"
              class="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors resize-none"
            ></textarea>
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Video URL</label>
            <input
              v-model="form.videoUrl"
              type="url"
              placeholder="https://youtube.com/watch?v=…"
              class="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
            />
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Categories</label>
            <div class="flex flex-wrap gap-1.5 p-3 bg-[#0B0B0C] border border-[#2A2A2E] rounded">
              <button
                v-for="cat in filteredCategories"
                :key="cat.id"
                type="button"
                @click="toggleCategory(cat.id)"
                class="px-2.5 py-1 rounded text-xs font-medium transition-all border"
                :class="form.categoryIds.includes(cat.id)
                  ? 'bg-[#DC2626] text-white border-[#DC2626]'
                  : 'bg-[#1C1C1F] text-[#9CA3AF] border-[#2A2A2E] hover:border-[#DC2626] hover:text-white'"
              >
                {{ cat.name }}
              </button>
            </div>
          </div>

          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Operators ({{ form.side }})</label>
            <div class="flex flex-wrap gap-1.5 p-3 bg-[#0B0B0C] border border-[#2A2A2E] rounded">
              <button
                v-for="op in filteredOperators"
                :key="op.id"
                type="button"
                @click="toggleOperator(op.id)"
                class="px-2.5 py-1 rounded text-xs font-medium transition-all border"
                :class="form.operatorIds.includes(op.id)
                  ? 'bg-[#DC2626] text-white border-[#DC2626]'
                  : 'bg-[#1C1C1F] text-[#9CA3AF] border-[#2A2A2E] hover:border-[#DC2626] hover:text-white'"
              >
                {{ op.name }}
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
            class="px-5 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
          >
            {{ mode === 'create' ? 'Create' : 'Save Changes' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
