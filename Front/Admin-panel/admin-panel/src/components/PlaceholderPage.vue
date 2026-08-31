<script setup>
const props = defineProps({
  title: String,
  description: String,
  icon: String,
})
</script>

<template>
  <div class="animate-fade-in flex flex-col items-center justify-center py-24 gap-5">
    <div class="w-16 h-16 rounded-full bg-[#1C1C1F] border border-[#2A2A2E] flex items-center justify-center text-[#9CA3AF]" v-html="icon"></div>
    <div class="text-center max-w-xs">
      <h2 class="text-white font-semibold text-base mb-1.5">{{ title }}</h2>
      <p class="text-[#9CA3AF] text-sm leading-relaxed">{{ description }}</p>
    </div>
    <button class="mt-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors">
      Coming Soon
    </button>
  </div>
</template>
