<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import OfflineNotice from '@/components/OfflineNotice.vue'
import QrShare from '@/components/QrShare.vue'
import VideoPlayer from '@/components/VideoPlayer.vue'
import { getStrat } from '@/api/client'
import { useNotify } from '@/composables/useNotify'
import { useVibration } from '@/composables/useVibration'
import { useWakeLock } from '@/composables/useWakeLock'
import { useOfflineStore } from '@/stores/offline'
import { deriveSide, type StratDetail } from '@/types'

const route = useRoute()
const offline = useOfflineStore()
const { notify } = useNotify()
const { pulse } = useVibration()

// Keep the screen awake while the plan is open during a round.
useWakeLock()

const strat = ref<StratDetail | null>(null)
const loading = ref(true)
const loadFailed = ref(false)
const fromOfflineCopy = ref(false)

const stratId = computed(() => Number(route.params.id))
const side = computed(() => (strat.value ? deriveSide(strat.value) : null))

function initials(name: string): string {
  return name
    .split(/\s+/)
    .map((part) => part[0] ?? '')
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

async function load() {
  loading.value = true
  loadFailed.value = false
  strat.value = null
  fromOfflineCopy.value = false

  if (offline.isOnline) {
    try {
      const detail = await getStrat(stratId.value)
      strat.value = detail
      loading.value = false
      // Fully opening a strategy keeps a copy on this device (Rule 16.1).
      const wasSaved = offline.isSaved(detail.id)
      const saved = await offline.saveStrat(detail)
      if (saved) {
        pulse()
        if (!wasSaved) {
          void notify('Mapify', `"${detail.name}" is saved for offline use.`)
        }
      }
      return
    } catch {
      // Fall through to the local copy below.
    }
  }

  const localCopy = await offline.getStrat(stratId.value)
  if (localCopy) {
    strat.value = localCopy
    fromOfflineCopy.value = true
  } else {
    loadFailed.value = true
  }
  loading.value = false
}

watch(stratId, load, { immediate: true })
</script>

<template>
  <div class="container">
    <div v-if="loading" class="stack" style="margin-top: 24px">
      <div class="skeleton" style="min-height: 220px"></div>
      <div class="skeleton"></div>
    </div>

    <template v-else-if="strat">
      <h1 class="page-title">{{ strat.name }}</h1>
      <p v-if="fromOfflineCopy" class="page-subtitle">
        Showing the copy saved on this device.
      </p>

      <div class="detail-meta">
        <span class="tag">{{ strat.map.name }}</span>
        <span v-if="strat.bombsite" class="tag">{{ strat.bombsite.name }}</span>
        <span
          v-if="side"
          class="badge"
          :class="side === 'Attack' ? 'badge-attack' : 'badge-defense'"
        >
          {{ side }}
        </span>
        <span v-for="category in strat.categories" :key="`c-${category.id}`" class="tag">
          {{ category.name }}
        </span>
        <span
          v-for="operator in strat.operators"
          :key="`o-${operator.id}`"
          class="op-badge"
          :class="operator.side.toLowerCase()"
          :title="operator.name"
        >
          {{ initials(operator.name) }}
        </span>
      </div>

      <VideoPlayer v-if="strat.videoUrl" :url="strat.videoUrl" />

      <div class="card">
        <h2 style="margin-top: 0">The plan</h2>
        <p v-if="strat.description" style="white-space: pre-wrap; margin-bottom: 0">
          {{ strat.description }}
        </p>
        <p v-else class="muted" style="margin-bottom: 0">
          No written plan was added for this strategy.
        </p>
      </div>

      <div class="detail-actions">
        <QrShare label="Share this strategy" />
      </div>
    </template>

    <div v-else-if="loadFailed" style="margin-top: 24px">
      <OfflineNotice
        v-if="!offline.isOnline"
        message="This strategy was never opened before, so no offline copy exists on this device."
      />
      <div v-else class="form-error">
        This strategy could not be loaded. It may have been removed.
        <button type="button" class="btn" style="margin-left: 10px" @click="load">Retry</button>
      </div>
    </div>
  </div>
</template>
