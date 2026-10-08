/**
 * Local notifications via the Notification API. Everything fails silently so
 * the app never breaks in browsers without notification support or with the
 * permission denied. The browser permission prompt is only triggered from the
 * consent card (NotificationConsent.vue) — never from here.
 */
export function useNotify() {
  async function notify(title: string, body?: string) {
    if (typeof window === 'undefined' || !('Notification' in window)) return
    try {
      if (Notification.permission === 'granted') {
        new Notification(title, { body, icon: '/icons/icon-192.png' })
      }
    } catch {
      // Notifications are best-effort.
    }
  }
  return { notify }
}
