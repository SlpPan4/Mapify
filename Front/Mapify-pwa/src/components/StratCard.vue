<script setup lang="ts">
import { computed } from 'vue'
import { useVibration } from '@/composables/useVibration'
import { deriveSide, type StratDetail, type StratSummary } from '@/types'

const props = defineProps<{
  strat: StratSummary | StratDetail
}>()

const { pulse } = useVibration()

const side = computed(() =>
  'side' in props.strat ? props.strat.side : deriveSide(props.strat),
)

function initials(name: string): string {
  return name
    .split(/\s+/)
    .map((part) => part[0] ?? '')
    .join('')
    .slice(0, 2)
    .toUpperCase()
}
</script>

<template>
  <router-link :to="`/strats/${strat.id}`" class="strat-card" @click="pulse()">
    <h3>{{ strat.name }}</h3>
    <div class="strat-meta">
      {{ strat.map.name }}
      <template v-if="strat.bombsite"> · {{ strat.bombsite.name }}</template>
      <span v-if="side" class="badge" :class="side === 'Attack' ? 'badge-attack' : 'badge-defense'" style="margin-left: 6px">
        {{ side }}
      </span>
    </div>
    <div class="tag-row">
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
  </router-link>
</template>
