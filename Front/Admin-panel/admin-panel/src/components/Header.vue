<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const props = defineProps({
  notifications: {
    type: Array,
    default: () => [],
  },
})
const emit = defineEmits(['refresh'])

const notifOpen = ref(false)

const pageTitles = {
  '/dashboard': 'Dashboard',
  '/pending': 'Pending Submissions',
  '/strats': 'Strats',
  '/categories': 'Categories',
  '/operators': 'Operators',
  '/maps': 'Maps',
}

const title = computed(() => {
  if (route.name === 'Strat Detail') {
    return route.params.name || 'Strat Detail'
  }
  return pageTitles[route.path] || 'Mapify Admin'
})
</script>

<template>
  <header class="h-14 border-b border-[#2A2A2E] bg-[#141416] flex items-center px-6 gap-4 sticky top-0 z-40">
    <h1 class="text-white font-semibold text-[15px] tracking-tight shrink-0 mr-2">{{ title }}</h1>

    <div class="flex items-center gap-2 ml-auto">
      <button
        @click="$emit('refresh')"
        class="w-8 h-8 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-all border border-transparent hover:border-[#2A2A2E]"
        title="Refresh"
      >
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
        </svg>
      </button>

      <div class="relative">
        <button
          @click="notifOpen = !notifOpen"
          class="w-8 h-8 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-all border border-transparent hover:border-[#2A2A2E] relative"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
          </svg>
          <span
            v-if="notifications.length > 0"
            class="absolute top-1.5 right-1.5 w-1.5 h-1.5 bg-[#DC2626] rounded-full ring-1 ring-[#141416]"
          />
        </button>

        <div v-if="notifOpen" class="fixed inset-0 z-40" @click="notifOpen = false"></div>
        <div
          v-if="notifOpen"
          class="absolute right-0 top-10 w-72 bg-[#1C1C1F] border border-[#2A2A2E] rounded shadow-2xl z-50 animate-fade-in"
        >
          <div class="px-4 py-3 border-b border-[#2A2A2E]">
            <span class="text-sm font-semibold text-white">Notifications</span>
          </div>
          <div
            v-for="(n, i) in notifications"
            :key="i"
            class="px-4 py-3 border-b border-[#2A2A2E] last:border-b-0 hover:bg-[#141416] transition-colors cursor-pointer"
          >
            <p class="text-sm text-white leading-snug">{{ n.text }}</p>
            <p class="text-[11px] text-[#9CA3AF] mt-1 font-mono">{{ n.time }}</p>
          </div>
        </div>
      </div>

      <div class="w-px h-5 bg-[#2A2A2E]" />

      <div class="w-7 h-7 rounded bg-[#DC2626] flex items-center justify-center cursor-pointer hover:bg-[#EF4444] transition-colors">
        <span class="text-white text-[11px] font-bold">MX</span>
      </div>
    </div>
  </header>
</template>
