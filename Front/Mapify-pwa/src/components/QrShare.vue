<script setup lang="ts">
import { nextTick, ref } from 'vue'
import QRCode from 'qrcode'

const props = withDefaults(
  defineProps<{
    url?: string
    label?: string
  }>(),
  { url: '', label: 'Share via QR' },
)

const open = ref(false)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const renderedUrl = ref('')

async function openModal() {
  renderedUrl.value = props.url || window.location.href
  open.value = true
  await nextTick()
  if (canvasRef.value) {
    try {
      await QRCode.toCanvas(canvasRef.value, renderedUrl.value, {
        width: 240,
        margin: 1,
        color: { dark: '#0f1115', light: '#ffffff' },
      })
    } catch {
      // If rendering fails the modal still shows the plain URL.
    }
  }
}
</script>

<template>
  <button type="button" class="btn" @click="openModal">{{ props.label }}</button>

  <div v-if="open" class="modal-backdrop" @click.self="open = false">
    <div class="modal" role="dialog" aria-modal="true" aria-label="Share via QR code">
      <h2 style="margin-top: 0">Share this view</h2>
      <canvas ref="canvasRef"></canvas>
      <p class="muted" style="word-break: break-all; font-size: 0.85rem">{{ renderedUrl }}</p>
      <button type="button" class="btn" @click="open = false">Close</button>
    </div>
  </div>
</template>
