<script setup>
const props = defineProps({
  status: {
    type: String,
    default: 'pending',
  },
})

const map = {
  pending: { bg: 'bg-[rgba(234,179,8,0.12)] border-[rgba(234,179,8,0.25)]', text: 'text-yellow-400', label: 'PENDING' },
  approved: { bg: 'bg-[rgba(22,163,74,0.12)] border-[rgba(22,163,74,0.25)]', text: 'text-[#4ADE80]', label: 'APPROVED' },
  rejected: { bg: 'bg-[rgba(220,38,38,0.12)] border-[rgba(220,38,38,0.25)]', text: 'text-[#EF4444]', label: 'REJECTED' },
}

const s = map[props.status] || map.pending
</script>

<template>
  <span
    class="inline-flex items-center px-2 py-0.5 rounded text-[10px] font-bold font-mono tracking-wider border"
    :class="[s.bg, s.text]"
  >
    {{ s.label }}
  </span>
</template>
