<script setup lang="ts">
import { computed } from 'vue'
import OfflineNotice from '@/components/OfflineNotice.vue'
import { useOfflineStore } from '@/stores/offline'

const props = defineProps<{
  url: string
}>()

const offline = useOfflineStore()

type Embed =
  | { kind: 'iframe'; src: string }
  | { kind: 'video'; src: string }
  | { kind: 'link' }

const embed = computed<Embed>(() => {
  const url = props.url.trim()

  const youtube = url.match(
    /(?:youtube\.com\/(?:watch\?[^#]*v=|shorts\/|embed\/)|youtu\.be\/)([\w-]{6,})/,
  )
  if (youtube?.[1]) {
    return { kind: 'iframe', src: `https://www.youtube.com/embed/${youtube[1]}` }
  }

  const tiktok = url.match(/tiktok\.com\/(?:@[\w.-]+\/)?video\/(\d+)/)
  if (tiktok?.[1]) {
    return { kind: 'iframe', src: `https://www.tiktok.com/embed/v2/${tiktok[1]}` }
  }

  const instagram = url.match(/instagram\.com\/(?:p|reel|reels)\/([\w-]+)/)
  if (instagram?.[1]) {
    return { kind: 'iframe', src: `https://www.instagram.com/p/${instagram[1]}/embed` }
  }

  if (/\.(mp4|webm|ogg|ogv|mov)([?#].*)?$/i.test(url)) {
    return { kind: 'video', src: url }
  }

  return { kind: 'link' }
})

function withProtocol(url: string): string {
  return /^https?:\/\//i.test(url) ? url : `https://${url}`
}
</script>

<template>
  <OfflineNotice
    v-if="!offline.isOnline"
    message="The video demonstration needs an internet connection. The written plan below remains available."
  />
  <div v-else-if="embed.kind === 'iframe'" class="video-wrapper">
    <iframe
      :src="embed.src"
      title="Strategy video"
      allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
      allowfullscreen
    ></iframe>
  </div>
  <div v-else-if="embed.kind === 'video'" class="video-wrapper">
    <video :src="embed.src" controls preload="metadata"></video>
  </div>
  <p v-else>
    <a :href="withProtocol(props.url)" target="_blank" rel="noopener noreferrer">
      Watch the video demonstration externally
    </a>
  </p>
</template>
