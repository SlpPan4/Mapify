/** Short haptic feedback for key actions; silently no-ops without support. */
export function useVibration() {
  function pulse(pattern: number | number[] = 30) {
    if (typeof navigator !== 'undefined' && 'vibrate' in navigator) {
      navigator.vibrate(pattern)
    }
  }
  return { pulse }
}
