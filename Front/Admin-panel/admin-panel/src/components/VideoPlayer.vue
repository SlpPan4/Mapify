<script setup>
import { computed } from 'vue'

const props = defineProps({
  url: {
    type: String,
    required: true,
  },
})

const platform = computed(() => detectPlatform(props.url))
const embedUrl = computed(() => getEmbedUrl(props.url, platform.value))
const videoId = computed(() => extractVideoId(props.url, platform.value))

function detectPlatform(url) {
  if (!url) return 'unknown'
  const lower = url.toLowerCase()
  if (lower.includes('youtube.com') || lower.includes('youtu.be')) return 'youtube'
  if (lower.includes('tiktok.com')) return 'tiktok'
  if (lower.includes('instagram.com')) return 'instagram'
  if (/\.(mp4|webm|ogg|mov)(\?.*)?$/i.test(lower)) return 'direct'
  return 'unknown'
}

function extractVideoId(url, platform) {
  if (!url) return null
  try {
    const u = new URL(url)
    if (platform === 'youtube') {
      if (u.hostname.includes('youtu.be')) {
        return u.pathname.slice(1)
      }
      return u.searchParams.get('v') || u.pathname.split('/').pop()
    }
    if (platform === 'tiktok') {
      const match = url.match(/\/video\/(\d+)/)
      return match?.[1] || u.pathname.split('/').pop()
    }
    if (platform === 'instagram') {
      const match = url.match(/\/(p|reel|reels|tv)\/([^/]+)/)
      return match?.[2] || u.pathname.split('/').filter(Boolean).pop()
    }
  } catch {
    return null
  }
  return null
}

function getEmbedUrl(url, platform) {
  const id = extractVideoId(url, platform)
  if (!id) return null
  if (platform === 'youtube') return `https://www.youtube.com/embed/${id}`
  if (platform === 'tiktok') return `https://www.tiktok.com/embed/v2/${id}`
  if (platform === 'instagram') return `https://www.instagram.com/p/${id}/embed`
  return url
}
</script>

<template>
  <div class="w-full rounded overflow-hidden border border-[#2A2A2E] bg-[#0B0B0C]">
    <div v-if="platform === 'youtube' && embedUrl" class="relative w-full aspect-video">
      <iframe
        :src="embedUrl"
        class="absolute inset-0 w-full h-full"
        frameborder="0"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowfullscreen
      ></iframe>
    </div>

    <div v-else-if="platform === 'tiktok' && embedUrl" class="relative w-full min-h-[500px]">
      <iframe
        :src="embedUrl"
        class="absolute inset-0 w-full h-full"
        frameborder="0"
        allow="fullscreen"
        scrolling="no"
      ></iframe>
    </div>

    <div v-else-if="platform === 'instagram' && embedUrl" class="relative w-full min-h-[500px]">
      <iframe
        :src="embedUrl"
        class="absolute inset-0 w-full h-full"
        frameborder="0"
        scrolling="no"
        allowtransparency="true"
      ></iframe>
    </div>

    <div v-else-if="platform === 'direct'" class="relative w-full aspect-video">
      <video
        :src="url"
        controls
        class="absolute inset-0 w-full h-full"
      ></video>
    </div>

    <div v-else class="flex flex-col items-center justify-center py-12 px-4 text-center gap-3">
      <div class="w-12 h-12 rounded-full bg-[rgba(220,38,38,0.15)] flex items-center justify-center">
        <svg class="w-6 h-6 text-[#DC2626]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M3 8a2 2 0 012-2h8a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2V8z" />
        </svg>
      </div>
      <p class="text-[#9CA3AF] text-sm">Unsupported video URL</p>
      <a
        :href="url"
        target="_blank"
        rel="noopener noreferrer"
        class="text-[#60A5FA] text-xs hover:text-blue-300 transition-colors truncate max-w-full"
      >
        {{ url }}
      </a>
    </div>
  </div>
</template>
