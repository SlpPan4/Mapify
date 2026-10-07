<script setup lang="ts">
import { computed } from 'vue'
import { useFiltersStore } from '@/stores/filters'
import { useReferenceStore } from '@/stores/reference'
import { SIDES, type Side } from '@/types'

// 'full' shows every control (library browsing); 'match' is the home funnel:
// the map comes from the map grid, so the select is hidden, and the
// remaining controls appear only once a map is chosen.
const props = withDefaults(defineProps<{ variant?: 'full' | 'match' }>(), { variant: 'full' })

const filters = useFiltersStore()
const reference = useReferenceStore()

const maps = computed(() => reference.maps)
const selectedMap = computed(() => reference.mapById(filters.mapId))
const bombsites = computed(() => selectedMap.value?.bombsites ?? [])
const categories = computed(() => reference.categoriesForSide(filters.side))
const operators = computed(() => reference.operatorsForSide(filters.side))
const showAdvanced = computed(() => props.variant === 'full' || filters.mapId !== null)

function numberOrNull(event: Event): number | null {
  const value = (event.target as HTMLSelectElement).value
  return value === '' ? null : Number(value)
}

function onSide(side: Side) {
  filters.setSide(filters.side === side ? null : side)
}
</script>

<template>
  <div class="filter-bar" :class="`filter-bar-${props.variant}`">
    <div class="filter-group">
      <label id="side-label">Side</label>
      <div class="side-toggle" role="group" aria-labelledby="side-label">
        <button
          v-for="side in SIDES"
          :key="side"
          type="button"
          :class="filters.side === side ? `active-${side.toLowerCase()}` : ''"
          @click="onSide(side)"
        >
          {{ side }}
        </button>
      </div>
    </div>

    <div v-if="props.variant === 'full'" class="filter-group">
      <label for="filter-map">Map</label>
      <select
        id="filter-map"
        class="select"
        :value="filters.mapId ?? ''"
        @change="filters.setMap(numberOrNull($event))"
      >
        <option value="">All maps</option>
        <option v-for="map in maps" :key="map.id" :value="map.id">{{ map.name }}</option>
      </select>
    </div>

    <template v-if="showAdvanced">
      <div class="filter-group">
        <label for="filter-bombsite">Bombsite</label>
        <select
          id="filter-bombsite"
          class="select"
          :disabled="filters.mapId === null"
          :value="filters.bombsiteId ?? ''"
          @change="filters.setBombsite(numberOrNull($event))"
        >
          <option value="">
            {{ filters.mapId === null ? 'Pick a map first' : 'All sites' }}
          </option>
          <option v-for="site in bombsites" :key="site.id" :value="site.id">{{ site.name }}</option>
        </select>
      </div>

      <div class="filter-group">
        <label for="filter-category">Category</label>
        <select
          id="filter-category"
          class="select"
          :value="filters.categoryId ?? ''"
          @change="filters.setCategory(numberOrNull($event))"
        >
          <option value="">All categories</option>
          <option v-for="category in categories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label for="filter-operator">Operator</label>
        <select
          id="filter-operator"
          class="select"
          :value="filters.operatorId ?? ''"
          @change="filters.setOperator(numberOrNull($event))"
        >
          <option value="">All operators</option>
          <option v-for="operator in operators" :key="operator.id" :value="operator.id">
            {{ operator.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label for="filter-q">Search by name</label>
        <input
          id="filter-q"
          class="input"
          type="search"
          placeholder="e.g. rush"
          :value="filters.q"
          @input="filters.setQuery(($event.target as HTMLInputElement).value)"
        />
      </div>
    </template>

    <div class="filter-group" style="flex: 0 0 auto">
      <button
        type="button"
        class="btn btn-ghost"
        :disabled="!filters.hasActive"
        @click="filters.clearAll()"
      >
        Clear all
      </button>
    </div>
  </div>
</template>
