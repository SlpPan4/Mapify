<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  open: Boolean,
  mode: {
    type: String,
    default: 'create',
  },
  category: Object,
})

const emit = defineEmits(['close', 'save'])

const form = ref({ name: '', side: 'Attack' })

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    form.value = props.category
      ? { name: props.category.name, side: props.category.side }
      : { name: '', side: 'Attack' }
  }
})

function handleSubmit() {
  emit('save', { ...form.value })
}
</script>

<template>
  <div v-if="open">
    <div class="fixed inset-0 z-[100] bg-black/75 backdrop-blur-[2px]" @click="$emit('close')"></div>
    <div class="fixed inset-0 z-[110] flex items-center justify-center p-6 pointer-events-none">
      <div
        class="bg-[#141416] border border-[#2A2A2E] rounded shadow-2xl w-full max-w-[420px] flex flex-col animate-slide-up pointer-events-auto"
        @click.stop
      >
        <div class="px-6 py-4 border-b border-[#2A2A2E] flex items-center justify-between">
          <h2 class="text-white font-bold text-base">{{ mode === 'create' ? 'Add Category' : 'Edit Category' }}</h2>
          <button @click="$emit('close')" class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit" class="px-6 py-5 space-y-5">
          <div>
            <label class="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Category Name <span class="text-[#DC2626]">*</span></label>
            <input
              v-model="form.name"
              required
              type="text"
              placeholder="e.g. Vertical Play"
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
                @click="form.side = s"
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
        </form>

        <div class="px-6 py-4 border-t border-[#2A2A2E] flex items-center justify-end gap-3">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 bg-[#1C1C1F] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-sm font-medium rounded border border-[#2A2A2E] transition-colors"
          >
            Cancel
          </button>
          <button
            type="button"
            @click="handleSubmit"
            class="px-5 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
          >
            {{ mode === 'create' ? 'Create' : 'Save' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
