<script setup lang="ts">
import { onMounted } from 'vue'
import SubmissionForm from '@/components/SubmissionForm.vue'
import { useReferenceStore } from '@/stores/reference'

const reference = useReferenceStore()

onMounted(() => {
  void reference.load()
})
</script>

<template>
  <div class="container">
    <h1 class="page-title">Submit for review</h1>
    <p class="page-subtitle">
      Share a strategy or propose a category. Submissions appear publicly after moderator
      approval.
    </p>

    <div v-if="reference.error" class="form-error">
      {{ reference.error }}
      <button type="button" class="btn" style="margin-left: 10px" @click="reference.load(true)">
        Retry
      </button>
    </div>

    <SubmissionForm v-else mode="both" />
  </div>
</template>
