<script setup>
const props = defineProps({
  side: {
    type: String,
    required: true,
    validator: (value) => ['Attack', 'Defense'].includes(value),
  },
})
</script>

<template>
  <span
    class="inline-flex items-center px-2 py-0.5 rounded text-[11px] font-semibold font-mono tracking-wide border"
    :class="side === 'Attack'
      ? 'bg-[rgba(220,38,38,0.15)] text-[#EF4444] border-[rgba(220,38,38,0.25)]'
      : 'bg-[rgba(37,99,235,0.15)] text-[#60A5FA] border-[rgba(37,99,235,0.25)]'"
  >
    <span class="mr-1">{{ side === 'Attack' ? '⚔' : '🛡' }}</span>
    {{ side.toUpperCase() }}
  </span>
</template>
