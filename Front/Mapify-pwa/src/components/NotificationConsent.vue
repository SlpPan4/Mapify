<script setup lang="ts">
import { ref } from 'vue'

const CONSENT_KEY = 'mapify.notify-consent'

const supported = typeof window !== 'undefined' && 'Notification' in window

// One-time consent card: the browser permission prompt is only ever triggered
// from the Enable button here. "Not now" is remembered so the app never nags,
// and denial stays silent — offline saving works either way.
const visible = ref(
  supported &&
    Notification.permission === 'default' &&
    localStorage.getItem(CONSENT_KEY) === null,
)

async function enable() {
  try {
    localStorage.setItem(CONSENT_KEY, 'enabled')
  } catch {
    // Best-effort.
  }
  visible.value = false
  try {
    await Notification.requestPermission()
  } catch {
    // Denied or unavailable: notifications stay off, everything else works.
  }
}

function notNow() {
  try {
    localStorage.setItem(CONSENT_KEY, 'dismissed')
  } catch {
    // Best-effort.
  }
  visible.value = false
}
</script>

<template>
  <div v-if="visible" class="card consent-banner" role="dialog" aria-label="Notification consent">
    <p style="margin-top: 0">
      Mapify can send a local notification when a strategy is saved for offline use or a
      submission is sent for review.
    </p>
    <div style="display: flex; gap: 8px; justify-content: flex-end">
      <button type="button" class="btn btn-ghost" @click="notNow">Not now</button>
      <button type="button" class="btn btn-primary" @click="enable">Enable</button>
    </div>
  </div>
</template>
