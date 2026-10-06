import type { StratDetail } from '@/types'

const DB_NAME = 'mapify-offline'
const DB_VERSION = 1
const STORE_NAME = 'strats'

export function isOfflineStorageAvailable(): boolean {
  return typeof indexedDB !== 'undefined'
}

function openDb(): Promise<IDBDatabase> {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION)
    request.onupgradeneeded = () => {
      const db = request.result
      if (!db.objectStoreNames.contains(STORE_NAME)) {
        db.createObjectStore(STORE_NAME, { keyPath: 'id' })
      }
    }
    request.onsuccess = () => resolve(request.result)
    request.onerror = () => reject(request.error ?? new Error('Failed to open offline database'))
  })
}

async function run<T>(
  mode: IDBTransactionMode,
  action: (store: IDBObjectStore) => IDBRequest,
): Promise<T> {
  const db = await openDb()
  try {
    return await new Promise<T>((resolve, reject) => {
      const tx = db.transaction(STORE_NAME, mode)
      const request = action(tx.objectStore(STORE_NAME))
      request.onsuccess = () => resolve(request.result as T)
      request.onerror = () => reject(request.error ?? new Error('Offline storage request failed'))
      tx.onerror = () => reject(tx.error ?? new Error('Offline storage transaction failed'))
    })
  } finally {
    db.close()
  }
}

export async function saveStratToDb(strat: StratDetail): Promise<void> {
  await run<IDBValidKey>('readwrite', (store) => store.put(strat))
}

export function getStratFromDb(id: number): Promise<StratDetail | undefined> {
  return run<StratDetail | undefined>('readonly', (store) => store.get(id))
}

export function listStratsFromDb(): Promise<StratDetail[]> {
  return run<StratDetail[]>('readonly', (store) => store.getAll())
}

export async function removeStratFromDb(id: number): Promise<void> {
  await run<undefined>('readwrite', (store) => store.delete(id))
}
