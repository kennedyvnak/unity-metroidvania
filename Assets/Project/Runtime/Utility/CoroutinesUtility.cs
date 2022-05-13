using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania
{
    public static class CoroutinesUtility
    {
        private static readonly Dictionary<float, WaitForSeconds> YieldSecondsWait = new();

        public static WaitForSeconds GetYieldSeconds(float time)
        {
            if (!YieldSecondsWait.TryGetValue(time, out var seconds))
                YieldSecondsWait[time] = seconds = new WaitForSeconds(time);
            return seconds;
        }

        public static void EnsureStopCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine)
        {
            if (coroutine == null) return;
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}