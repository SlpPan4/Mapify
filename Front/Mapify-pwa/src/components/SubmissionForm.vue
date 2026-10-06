<script setup lang="ts">
import { computed, ref } from 'vue'
import OfflineNotice from '@/components/OfflineNotice.vue'
import { submitCategory, submitStrat } from '@/api/client'
import { useNotify } from '@/composables/useNotify'
import { useDraftStore } from '@/stores/draft'
import { useOfflineStore } from '@/stores/offline'
import { useReferenceStore } from '@/stores/reference'
import { SIDES, type Side } from '@/types'

const props = withDefaults(
  defineProps<{
    // SubmitView shows both forms; the constructor flow only needs the strategy form.
    mode?: 'strat' | 'category' | 'both'
  }>(),
  { mode: 'both' },
)

const emit = defineEmits<{
  submitted: []
}>()

const draft = useDraftStore()
const reference = useReferenceStore()
const offline = useOfflineStore()
const { notify } = useNotify()

const sending = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const selectedMap = computed(() => reference.mapById(draft.mapId))
const bombsites = computed(() => selectedMap.value?.bombsites ?? [])

// When a side is chosen the pickers are constrained to it; otherwise everything
// is offered and side-consistency is validated before sending (Rules 10.2/14.3).
const offeredCategories = computed(() => reference.categoriesForSide(draft.side))
const offeredOperators = computed(() => reference.operatorsForSide(draft.side))

function toggleId(list: number[], id: number): number[] {
  return list.includes(id) ? list.filter((x) => x !== id) : [...list, id]
}

function toggleCategory(id: number) {
  draft.categoryIds = toggleId(draft.categoryIds, id)
}

function toggleOperator(id: number) {
  draft.operatorIds = toggleId(draft.operatorIds, id)
}

function onMapChange(event: Event) {
  const value = (event.target as HTMLSelectElement).value
  draft.setMap(value === '' ? null : Number(value))
}

function onBombsiteChange(event: Event) {
  const value = (event.target as HTMLSelectElement).value
  draft.setBombsite(value === '' ? null : Number(value))
}

function onSideChange(event: Event) {
  const value = (event.target as HTMLSelectElement).value
  draft.setSide(value === '' ? null : (value as Side))
}

function validateStrat(): string | null {
  if (!draft.name.trim()) return 'A strategy name is required.'
  if (draft.mapId === null) return 'Please choose an official map.'
  if (!draft.videoUrl.trim()) return 'A video link is required.'
  const sides = new Set<Side>()
  for (const id of draft.categoryIds) {
    const side = reference.categories.find((c) => c.id === id)?.side
    if (side) sides.add(side)
  }
  for (const id of draft.operatorIds) {
    const side = reference.operators.find((o) => o.id === id)?.side
    if (side) sides.add(side)
  }
  if (sides.size > 1) {
    return 'Chosen categories and operators must all belong to the same side.'
  }
  return null
}

async function sendStrat() {
  errorMessage.value = ''
  successMessage.value = ''
  const validationError = validateStrat()
  if (validationError) {
    errorMessage.value = validationError
    return
  }
  sending.value = true
  try {
    await submitStrat({
      name: draft.name.trim(),
      videoUrl: draft.videoUrl.trim(),
      mapId: draft.mapId as number,
      bombsiteId: draft.bombsiteId,
      description: draft.description.trim(),
      categoryIds: draft.categoryIds,
      operatorIds: draft.operatorIds,
    })
    draft.reset()
    successMessage.value =
      'Strategy submitted for review. It will appear in the library after moderator approval.'
    void notify('Mapify', 'Your strategy was submitted for review.')
    emit('submitted')
  } catch (e) {
    errorMessage.value = e instanceof Error ? e.message : 'Submission failed.'
  } finally {
    sending.value = false
  }
}

// Category proposal form (Story 11).
const categoryName = ref('')
const categorySide = ref<Side | ''>('')
const categorySending = ref(false)
const categoryError = ref('')
const categorySuccess = ref('')

async function sendCategory() {
  categoryError.value = ''
  categorySuccess.value = ''
  if (!categoryName.value.trim()) {
    categoryError.value = 'A category name is required.'
    return
  }
  if (!categorySide.value) {
    categoryError.value = 'Please choose a side for the category.'
    return
  }
  categorySending.value = true
  try {
    await submitCategory({
      name: categoryName.value.trim(),
      side: categorySide.value as Side,
    })
    categoryName.value = ''
    categorySide.value = ''
    categorySuccess.value =
      'Category proposed. It becomes selectable after moderator approval.'
    void notify('Mapify', 'Your category proposal was submitted for review.')
    emit('submitted')
  } catch (e) {
    categoryError.value = e instanceof Error ? e.message : 'Submission failed.'
  } finally {
    categorySending.value = false
  }
}
</script>

<template>
  <div class="stack">
    <OfflineNotice
      v-if="!offline.isOnline"
      message="Submissions need an internet connection. The forms are disabled while offline."
    />

    <section v-if="props.mode !== 'category'" class="card">
      <h2 style="margin-top: 0">Submit a strategy</h2>

      <div v-if="errorMessage" class="form-error">{{ errorMessage }}</div>
      <div v-if="successMessage" class="form-success">{{ successMessage }}</div>

      <form @submit.prevent="sendStrat">
        <div class="form-field">
          <label for="sub-name">Name *</label>
          <input id="sub-name" v-model="draft.name" class="input" type="text" />
        </div>

        <div class="form-field">
          <label for="sub-map">Map *</label>
          <select
            id="sub-map"
            class="select"
            :value="draft.mapId ?? ''"
            @change="onMapChange"
          >
            <option value="">Choose a map</option>
            <option v-for="map in reference.maps" :key="map.id" :value="map.id">
              {{ map.name }}
            </option>
          </select>
        </div>

        <div class="form-field">
          <label for="sub-video">Video link *</label>
          <input
            id="sub-video"
            v-model="draft.videoUrl"
            class="input"
            type="text"
            placeholder="https://youtube.com/watch?v=..."
          />
        </div>

        <div class="form-field">
          <label for="sub-bombsite">Bombsite (optional)</label>
          <select
            id="sub-bombsite"
            class="select"
            :disabled="draft.mapId === null"
            :value="draft.bombsiteId ?? ''"
            @change="onBombsiteChange"
          >
            <option value="">No specific site</option>
            <option v-for="site in bombsites" :key="site.id" :value="site.id">
              {{ site.name }}
            </option>
          </select>
        </div>

        <div class="form-field">
          <label for="sub-description">Description (optional)</label>
          <textarea
            id="sub-description"
            v-model="draft.description"
            class="input"
            rows="3"
          ></textarea>
        </div>

        <div class="form-field">
          <label for="sub-side">Side (optional, narrows category/operator choices)</label>
          <select id="sub-side" class="select" :value="draft.side ?? ''" @change="onSideChange">
            <option value="">Any</option>
            <option v-for="side in SIDES" :key="side" :value="side">{{ side }}</option>
          </select>
        </div>

        <div class="form-field">
          <label>Categories (optional)</label>
          <div class="chip-row">
            <button
              v-for="category in offeredCategories"
              :key="category.id"
              type="button"
              class="chip"
              :class="{ selected: draft.categoryIds.includes(category.id) }"
              @click="toggleCategory(category.id)"
            >
              {{ category.name }} ({{ category.side }})
            </button>
          </div>
        </div>

        <div class="form-field">
          <label>Operators (optional)</label>
          <div class="chip-row">
            <button
              v-for="operator in offeredOperators"
              :key="operator.id"
              type="button"
              class="chip"
              :class="{ selected: draft.operatorIds.includes(operator.id) }"
              @click="toggleOperator(operator.id)"
            >
              {{ operator.name }} ({{ operator.side }})
            </button>
          </div>
        </div>

        <button
          type="submit"
          class="btn btn-primary"
          :disabled="sending || !offline.isOnline"
        >
          {{ sending ? 'Sending...' : 'Submit strategy for review' }}
        </button>
      </form>
    </section>

    <section v-if="props.mode !== 'strat'" class="card">
      <h2 style="margin-top: 0">Propose a category</h2>

      <div v-if="categoryError" class="form-error">{{ categoryError }}</div>
      <div v-if="categorySuccess" class="form-success">{{ categorySuccess }}</div>

      <form @submit.prevent="sendCategory">
        <div class="form-field">
          <label for="cat-name">Name *</label>
          <input
            id="cat-name"
            v-model="categoryName"
            class="input"
            type="text"
            placeholder="e.g. Shield clear"
          />
        </div>

        <div class="form-field">
          <label for="cat-side">Side *</label>
          <select id="cat-side" v-model="categorySide" class="select">
            <option value="">Choose a side</option>
            <option v-for="side in SIDES" :key="side" :value="side">{{ side }}</option>
          </select>
        </div>

        <button
          type="submit"
          class="btn btn-primary"
          :disabled="categorySending || !offline.isOnline"
        >
          {{ categorySending ? 'Sending...' : 'Propose category' }}
        </button>
      </form>
    </section>
  </div>
</template>
