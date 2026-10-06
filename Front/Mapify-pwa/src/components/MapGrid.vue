<script setup lang="ts">
import { useFiltersStore } from '@/stores/filters'
import { useReferenceStore } from '@/stores/reference'
import { useVibration } from '@/composables/useVibration'

const filters = useFiltersStore()
const reference = useReferenceStore()
const { pulse } = useVibration()

function toggleMap(mapId: number) {
  pulse()
  filters.setMap(filters.mapId === mapId ? null : mapId)
}
</script>

<template>
  <div class="map-grid">
    <button
      v-for="map in reference.maps"
      :key="map.id"
      type="button"
      class="map-tile"
      :class="{ active: filters.mapId === map.id }"
      @click="toggleMap(map.id)"
    >
      {{ map.name }}
    </button>
  </div>
</template>
