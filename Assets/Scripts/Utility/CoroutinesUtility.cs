using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania {
    /// <summary>Utility class for Unity Coroutines</summary>
    public static class CoroutinesUtility {
        private static readonly Dictionary<float, WaitForSeconds> k_YieldSecondsWait = new Dictionary<float, WaitForSeconds>();

        /// <summary>Use this method to get a cached <see cref="WaitForSeconds"/></summary>
        /// <param name="time">The yield time</param>
        /// <returns>A cached <see cref="WaitForSeconds"/></returns>
        public static WaitForSeconds GetYieldSeconds(float time) {
            if (!k_YieldSecondsWait.TryGetValue(time, out WaitForSeconds seconds))
                k_YieldSecondsWait[time] = seconds = new WaitForSeconds(time);
            return seconds;
        }

        private static readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame GetWaitForEndOfFrame() {
            return _waitForEndOfFrame;
        }

        /// <summary>Stop the coroutine in behaviour and make its reference null</summary>
        /// <param name="behaviour">The MonoBehaviour running the coroutine</param>
        /// <param name="coroutine">The coroutine</param>
        public static void EnsureStopCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine) {
            if (coroutine == null)
                return;
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}