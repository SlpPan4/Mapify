/**
 * Local notifications via the Notification API. Permission is requested
 * lazily on first use; everything fails silently so the app never breaks
 * in browsers without notification support.
 */
export function useNotify() {
  async function notify(title: string, body?: string) {
    if (typeof window === 'undefined' || !('Notification' in window)) return
    try {
      if (Notification.permission === 'default') {
        await Notification.requestPermission()
      }
      if (Notification.permission === 'granted') {
        new Notification(title, { body, icon: '/icons/icon-192.png' })
      }
    } catch {
      // Notifications are best-effort.
    }
  }
  return { notify }
}
