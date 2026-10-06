import { defineStore } from 'pinia'
import {
  getStratFromDb,
  isOfflineStorageAvailable,
  listStratsFromDb,
  removeStratFromDb,
  saveStratToDb,
} from '@/offline/db'
import type { StratDetail } from '@/types'

export interface SavedStratIndexEntry {
  id: number
  name: string
  mapName: string
}

export const useOfflineStore = defineStore('offline', {
  state: () => ({
    isOnline: typeof navigator === 'undefined' ? true : navigator.onLine,
    savedIndex: [] as SavedStratIndexEntry[],
    storageAvailable: isOfflineStorageAvailable(),
    initialized: false,
  }),
  getters: {
    isSaved: (state) => (id: number) => state.savedIndex.some((s) => s.id === id),
  },
  actions: {
    init() {
      if (this.initialized || typeof window === 'undefined') return
      this.initialized = true
      window.addEventListener('online', () => (this.isOnline = true))
      window.addEventListener('offline', () => (this.isOnline = false))
      void this.refreshIndex()
    },
    async refreshIndex() {
      if (!this.storageAvailable) return
      try {
        const strats = await listStratsFromDb()
        this.savedIndex = strats.map((s) => ({ id: s.id, name: s.name, mapName: s.map.name }))
      } catch {
        // IndexedDB failures must never break the app; offline copies are best-effort.
      }
    },
    async saveStrat(strat: StratDetail): Promise<boolean> {
      if (!this.storageAvailable) return false
      try {
        await saveStratToDb(strat)
        await this.refreshIndex()
        return true
      } catch {
        return false
      }
    },
    async getStrat(id: number): Promise<StratDetail | undefined> {
      if (!this.storageAvailable) return undefined
      try {
        return await getStratFromDb(id)
      } catch {
        return undefined
      }
    },
    async listSaved(): Promise<StratDetail[]> {
      if (!this.storageAvailable) return []
      try {
        return await listStratsFromDb()
      } catch {
        return []
      }
    },
    async removeStrat(id: number) {
      if (!this.storageAvailable) return
      try {
        await removeStratFromDb(id)
        await this.refreshIndex()
      } catch {
        // Best-effort cleanup.
      }
    },
  },
})
