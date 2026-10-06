import { onMounted, onUnmounted } from 'vue'

interface WakeLockSentinelLike {
  release(): Promise<void>
}
type WakeLockNavigator = Omit<Navigator, 'wakeLock'> & {
  wakeLock?: {
    request(type: 'screen'): Promise<WakeLockSentinel>
  }
}

/**
 * Keeps the screen awake while the calling component is mounted (the
 * strategy view), so the plan stays visible during a round. The lock is
 * re-requested when the tab becomes visible again.
 */
export function useWakeLock() {
  let sentinel: WakeLockSentinelLike | null = null

  async function request() {
    const nav = navigator as WakeLockNavigator
    if (!nav.wakeLock || sentinel) return
    try {
      sentinel = await nav.wakeLock.request('screen')
    } catch {
      // Wake lock is best-effort (e.g. denied in low battery mode).
    }
  }

  function release() {
    if (!sentinel) return
    void sentinel.release().catch(() => {})
    sentinel = null
  }

  function onVisibilityChange() {
    if (document.visibilityState === 'visible') {
      void request()
    }
  }

  onMounted(() => {
    void request()
    document.addEventListener('visibilitychange', onVisibilityChange)
  })

  onUnmounted(() => {
    document.removeEventListener('visibilitychange', onVisibilityChange)
    release()
  })
}
