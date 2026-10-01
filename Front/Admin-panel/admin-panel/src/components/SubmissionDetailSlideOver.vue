<script setup>
import { computed } from 'vue'
import SideBadge from './SideBadge.vue'
import StatusBadge from './StatusBadge.vue'
import VideoPlayer from './VideoPlayer.vue'

const props = defineProps({
  submission: Object,
  categories: Array,
  operators: Array,
  maps: Array,
})

const emit = defineEmits(['close', 'approve', 'reject'])

const isOpen = computed(() => !!props.submission)

const mapName = computed(() => {
  if (!props.submission?.mapId) return '—'
  const map = props.maps?.find((m) => m.id === props.submission.mapId)
  return map?.name || props.submission.mapId
})

const categoryNames = computed(() => {
  if (!props.submission?.categoryIds?.length) return []
  return props.submission.categoryIds
    .map((id) => props.categories?.find((c) => c.id === id)?.name)
    .filter(Boolean)
})

const operatorNames = computed(() => {
  if (!props.submission?.operatorIds?.length) return []
  return props.submission.operatorIds
    .map((id) => props.operators?.find((o) => o.id === id)?.name)
    .filter(Boolean)
})

function formatDate(iso) {
  return new Date(iso).toLocaleString('en-US', { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' })
}

function handleApprove() {
  emit('approve', props.submission.id)
  emit('close')
}

function handleReject() {
  emit('reject', props.submission.id)
  emit('close')
}
</script>

<template>
  <div v-if="isOpen">
    <div class="fixed inset-x-0 top-14 bottom-0 z-[100] bg-black/70 backdrop-blur-[2px]" @click="$emit('close')"></div>

    <div class="fixed right-0 top-14 bottom-0 w-full max-w-[520px] bg-[#141416] border-l border-[#2A2A2E] z-[110] animate-slide-right flex flex-col shadow-2xl">
      <div class="px-6 py-4 border-b border-[#2A2A2E] flex items-start justify-between gap-4 shrink-0">
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 mb-1">
            <span class="text-[#9CA3AF] text-xs font-mono">SUB-{{ submission.id }}</span>
            <StatusBadge status="pending" />
          </div>
          <h2 class="text-white font-bold text-lg leading-tight">{{ submission.name }}</h2>
        </div>
        <button
          @click="$emit('close')"
          class="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors shrink-0 mt-0.5"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <div class="flex-1 min-h-0 overflow-y-auto px-6 py-5 space-y-6">
        <div class="flex items-center gap-3 flex-wrap">
          <SideBadge :side="submission.side" />
          <div class="flex items-center gap-1.5">
            <svg class="w-3.5 h-3.5 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
            </svg>
            <span class="text-[#9CA3AF] text-sm">{{ mapName }}</span>
          </div>
          <div class="flex items-center gap-1.5">
            <svg class="w-3.5 h-3.5 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span class="text-[#9CA3AF] text-xs font-mono">Submitted {{ formatDate(submission.submittedAt) }}</span>
          </div>
        </div>

        <div>
          <div class="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Description</div>
          <p class="text-white/90 text-sm leading-relaxed bg-[#1C1C1F] border border-[#2A2A2E] rounded p-4">
            {{ submission.description || 'No description provided.' }}
          </p>
        </div>

        <div v-if="submission.videoUrl">
          <div class="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Video Reference</div>
          <VideoPlayer :url="submission.videoUrl" />
          <a
            :href="submission.videoUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="flex items-center gap-2 text-[#60A5FA] text-sm hover:text-blue-300 transition-colors mt-3"
          >
            <svg class="w-3.5 h-3.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
            </svg>
            <span class="truncate">{{ submission.videoUrl }}</span>
          </a>
        </div>

        <div v-if="categoryNames.length > 0">
          <div class="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Categories</div>
          <div class="flex flex-wrap gap-1.5">
            <span v-for="c in categoryNames" :key="c" class="px-2.5 py-1 bg-[#1C1C1F] border border-[#2A2A2E] rounded text-xs text-[#9CA3AF] font-mono">
              {{ c }}
            </span>
          </div>
        </div>

        <div v-if="operatorNames.length > 0">
          <div class="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Operators</div>
          <div class="flex flex-wrap gap-1.5">
            <span
              v-for="op in operatorNames"
              :key="op"
              class="px-2.5 py-1 rounded text-xs font-semibold font-mono border"
              :class="submission.side === 'Attack'
                ? 'bg-[rgba(220,38,38,0.1)] border-[rgba(220,38,38,0.2)] text-[#EF4444]'
                : 'bg-[rgba(37,99,235,0.1)] border-[rgba(37,99,235,0.2)] text-[#60A5FA]'"
            >
              {{ op }}
            </span>
          </div>
        </div>
      </div>

      <div class="px-6 py-4 border-t border-[#2A2A2E] flex items-center gap-3 shrink-0">
        <button
          @click="handleApprove"
          class="flex-1 flex items-center justify-center gap-2 py-2.5 bg-[#DC2626] hover:bg-[#EF4444] text-white font-semibold text-sm rounded transition-colors"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
          </svg>
          Approve
        </button>
        <button
          @click="handleReject"
          class="flex-1 flex items-center justify-center gap-2 py-2.5 bg-[#1C1C1F] hover:bg-[#242428] text-[#EF4444] font-semibold text-sm rounded border border-[#DC2626] transition-colors"
        >
          <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
          Reject
        </button>
      </div>
    </div>
  </div>
</template>
