<script setup lang="ts">
import { computed, ref } from 'vue'
import { useDraftStore } from '@/stores/draft'
import { useReferenceStore } from '@/stores/reference'
import { SIDES, type Operator, type Side } from '@/types'

const emit = defineEmits<{
  done: []
}>()

const draft = useDraftStore()
const reference = useReferenceStore()

// Resume the wizard at the furthest completed step.
const step = ref(draft.mapId === null ? 1 : draft.bombsiteId === null ? 2 : 3)

const steps = ['Map', 'Bombsite', 'Sketch']

const selectedMap = computed(() => reference.mapById(draft.mapId))
const bombsites = computed(() => selectedMap.value?.bombsites ?? [])
const sideOperators = computed(() =>
  draft.side ? reference.operatorsForSide(draft.side) : [],
)

type Tool = { type: 'operator'; operatorId: number } | { type: 'note' } | null
const selectedTool = ref<Tool>(null)
const pendingNote = ref<{ x: number; y: number; text: string } | null>(null)

function pickMap(mapId: number) {
  draft.setMap(mapId)
  step.value = 2
}

function pickBombsite(bombsiteId: number) {
  draft.setBombsite(bombsiteId)
  step.value = 3
}

function pickSide(side: Side) {
  draft.setSide(draft.side === side ? null : side)
  selectedTool.value = null
}

function pickOperator(operator: Operator) {
  selectedTool.value =
    selectedTool.value?.type === 'operator' && selectedTool.value.operatorId === operator.id
      ? null
      : { type: 'operator', operatorId: operator.id }
}

function pickNoteTool() {
  selectedTool.value = selectedTool.value?.type === 'note' ? null : { type: 'note' }
}

function operatorById(id: number): Operator | undefined {
  return reference.operators.find((o) => o.id === id)
}

function initials(name: string): string {
  return name
    .split(/\s+/)
    .map((part) => part[0] ?? '')
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

function canvasPosition(event: MouseEvent): { x: number; y: number } {
  const rect = (event.currentTarget as HTMLElement).getBoundingClientRect()
  const x = Math.min(100, Math.max(0, ((event.clientX - rect.left) / rect.width) * 100))
  const y = Math.min(100, Math.max(0, ((event.clientY - rect.top) / rect.height) * 100))
  return { x: Math.round(x * 10) / 10, y: Math.round(y * 10) / 10 }
}

function onCanvasClick(event: MouseEvent) {
  if (!selectedTool.value || !draft.side) return
  const pos = canvasPosition(event)
  if (selectedTool.value.type === 'operator') {
    draft.placeOperator(selectedTool.value.operatorId, pos.x, pos.y)
  } else {
    pendingNote.value = { ...pos, text: '' }
  }
}

function commitNote() {
  if (!pendingNote.value) return
  const text = pendingNote.value.text.trim()
  if (text) {
    draft.placeNote(text, pendingNote.value.x, pendingNote.value.y)
  }
  pendingNote.value = null
}
</script>

<template>
  <div>
    <div class="wizard-steps">
      <span
        v-for="(label, index) in steps"
        :key="label"
        class="wizard-step"
        :class="{ current: step === index + 1 }"
      >
        {{ index + 1 }}. {{ label }}
      </span>
    </div>

    <!-- Step 1: official map -->
    <div v-if="step === 1">
      <p class="muted">Pick the official map your strategy is played on.</p>
      <div class="map-grid">
        <button
          v-for="map in reference.maps"
          :key="map.id"
          type="button"
          class="map-tile"
          :class="{ active: draft.mapId === map.id }"
          @click="pickMap(map.id)"
        >
          {{ map.name }}
        </button>
      </div>
    </div>

    <!-- Step 2: bombsite of that map -->
    <div v-else-if="step === 2">
      <p class="muted">
        Pick the bombsite on <strong>{{ selectedMap?.name }}</strong> the strategy applies to.
      </p>
      <div class="map-grid">
        <button
          v-for="site in bombsites"
          :key="site.id"
          type="button"
          class="map-tile"
          @click="pickBombsite(site.id)"
        >
          {{ site.name }}
        </button>
      </div>
      <button type="button" class="btn btn-ghost" @click="step = 1">Back to maps</button>
    </div>

    <!-- Step 3: side + sketch -->
    <div v-else>
      <p class="muted">
        Sketch on <strong>{{ selectedMap?.name }}</strong> —
        <strong>{{ selectedMap?.bombsites.find((s) => s.id === draft.bombsiteId)?.name }}</strong>.
        Choose the side, then tap an operator or the note tool and tap the layout to place it.
        Tap a placed item to remove it.
      </p>

      <div class="filter-group" style="max-width: 260px; margin-bottom: 12px">
        <label id="draft-side-label">Side</label>
        <div class="side-toggle" role="group" aria-labelledby="draft-side-label">
          <button
            v-for="side in SIDES"
            :key="side"
            type="button"
            :class="draft.side === side ? `active-${side.toLowerCase()}` : ''"
            @click="pickSide(side)"
          >
            {{ side }}
          </button>
        </div>
      </div>

      <template v-if="draft.side">
        <div class="palette">
          <button
            v-for="operator in sideOperators"
            :key="operator.id"
            type="button"
            class="op-badge"
            :class="[
              operator.side.toLowerCase(),
              {
                selected:
                  selectedTool?.type === 'operator' && selectedTool.operatorId === operator.id,
              },
            ]"
            :title="operator.name"
            @click="pickOperator(operator)"
          >
            {{ initials(operator.name) }}
          </button>
          <button
            type="button"
            class="btn"
            :class="{ selected: selectedTool?.type === 'note' }"
            style="padding: 2px 12px; font-size: 0.8rem"
            @click="pickNoteTool"
          >
            + Note
          </button>
        </div>

        <div class="sketch-canvas" @click="onCanvasClick">
          <span
            v-for="(item, index) in draft.sketch.operators"
            :key="`op-${index}`"
            class="sketch-item op-badge"
            :class="draft.side?.toLowerCase()"
            :style="{ left: `${item.x}%`, top: `${item.y}%` }"
            :title="`${operatorById(item.operatorId)?.name ?? 'Operator'} — click to remove`"
            @click.stop="draft.removeSketchOperator(index)"
          >
            {{ initials(operatorById(item.operatorId)?.name ?? '?') }}
          </span>
          <span
            v-for="(note, index) in draft.sketch.notes"
            :key="`note-${index}`"
            class="sketch-note"
            :style="{ left: `${note.x}%`, top: `${note.y}%` }"
            :title="`${note.text} — click to remove`"
            @click.stop="draft.removeSketchNote(index)"
          >
            {{ note.text }}
          </span>
        </div>

        <div v-if="pendingNote" class="form-field" style="margin-top: 12px">
          <label for="note-text">Note text</label>
          <input
            id="note-text"
            v-model="pendingNote.text"
            class="input"
            type="text"
            placeholder="e.g. rush through balcony"
            @keyup.enter="commitNote"
          />
          <div style="margin-top: 8px; display: flex; gap: 8px">
            <button type="button" class="btn btn-primary" @click="commitNote">Place note</button>
            <button type="button" class="btn btn-ghost" @click="pendingNote = null">Cancel</button>
          </div>
        </div>

        <div style="margin-top: 16px; display: flex; gap: 10px; flex-wrap: wrap">
          <button type="button" class="btn btn-ghost" @click="step = 2">Back to bombsites</button>
          <button type="button" class="btn btn-primary" @click="emit('done')">
            Continue to submission
          </button>
        </div>
      </template>
      <p v-else class="muted">Pick a side to unlock the operator palette.</p>
    </div>
  </div>
</template>
