<script setup lang="ts">
import { onMounted, ref } from 'vue'
import StratConstructor from '@/components/StratConstructor.vue'
import SubmissionForm from '@/components/SubmissionForm.vue'
import { useReferenceStore } from '@/stores/reference'

const reference = useReferenceStore()

const wizardDone = ref(false)

onMounted(() => {
  void reference.load()
})
</script>

<template>
  <div class="container">
    <h1 class="page-title">Strategy constructor</h1>
    <p class="page-subtitle">
      Sketch your own tactic on an official map, then send it for review.
    </p>

    <div v-if="reference.error" class="form-error">
      {{ reference.error }}
      <button type="button" class="btn" style="margin-left: 10px" @click="reference.load(true)">
        Retry
      </button>
    </div>

    <template v-else>
      <StratConstructor v-if="!wizardDone" @done="wizardDone = true" />
      <div v-else class="stack">
        <button type="button" class="btn btn-ghost" @click="wizardDone = false">
          Back to the sketch
        </button>
        <SubmissionForm mode="strat" />
      </div>
    </template>
  </div>
</template>
