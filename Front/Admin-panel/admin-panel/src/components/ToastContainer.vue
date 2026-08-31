<script setup>
import { useToast } from '../composables/useToast.js'

const { toasts, removeToast } = useToast()

const icons = {
  success: `<svg class="w-4 h-4 text-[#16A34A] shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" /></svg>`,
  error: `<svg class="w-4 h-4 text-[#DC2626] shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`,
  info: `<svg class="w-4 h-4 text-blue-400 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5"><path stroke-linecap="round" stroke-linejoin="round" d="M13 16h-1v-4h-1m1-4h.01" /></svg>`,
}

const borderColors = {
  success: 'border-l-[#16A34A]',
  error: 'border-l-[#DC2626]',
  info: 'border-l-blue-500',
}
</script>

<template>
  <div class="fixed bottom-6 right-6 z-[200] flex flex-col gap-3 pointer-events-none">
    <div
      v-for="toast in toasts"
      :key="toast.id"
      class="animate-toast pointer-events-auto flex items-center gap-3 px-4 py-3 rounded bg-[#1C1C1F] border border-[#2A2A2E] border-l-2 shadow-2xl min-w-[280px] max-w-[360px]"
      :class="borderColors[toast.type]"
    >
      <div v-html="icons[toast.type]"></div>
      <span class="text-sm text-white font-medium flex-1">{{ toast.message }}</span>
      <button
        @click="removeToast(toast.id)"
        class="text-[#9CA3AF] hover:text-white transition-colors ml-2"
      >
        <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  </div>
</template>
