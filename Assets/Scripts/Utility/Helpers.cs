using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metroidvania
{
    public static class Helpers
    {
        private static Camera s_mainCamera;
        public static Camera mainCamera
        {
            get
            {
                if (!s_mainCamera)
                    s_mainCamera = Camera.main;
                return s_mainCamera;
            }
        }

        private static CinemachineCamera s_virtualCamera;
        public static CinemachineCamera virtualCamera
        {
            get
            {
                if (!s_virtualCamera)
                    s_virtualCamera = mainCamera.transform.parent.GetComponentInChildren<CinemachineCamera>(true);
                return s_virtualCamera;
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> s_YieldSecondsWait = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetYieldSeconds(float time)
        {
            if (!s_YieldSecondsWait.TryGetValue(time, out WaitForSeconds seconds))
                s_YieldSecondsWait[time] = seconds = new WaitForSeconds(time);
            return seconds;
        }

        private static readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame GetWaitForEndOfFrame()
        {
            return _waitForEndOfFrame;
        }

        public static void EnsureStopCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine)
        {
            if (coroutine == null)
                return;
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            Color gCol = renderer.color;
            renderer.color = new Color(gCol.r, gCol.g, gCol.b, alpha);
        }

        private static Canvas s_MainCanvas;
        public static Canvas mainCanvas
        {
            get
            {
                if (!s_MainCanvas)
                    s_MainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
                return s_MainCanvas;
            }
        }

        private static EventSystem s_EventSystem;
        public static EventSystem eventSystem
        {
            get
            {
                if (!s_EventSystem)
                    s_EventSystem = EventSystem.current;
                return s_EventSystem;
            }
        }

        public const float TransitionTime = .2F;

        private static readonly List<CanvasGroup> s_EventsBlocks = new List<CanvasGroup>();
        public static Tweener FadeGroup(this CanvasGroup group, bool active, float duration, TweenCallback onComplete = null, bool disableUIEvents = true)
        {
            if (disableUIEvents)
            {
                s_EventsBlocks.Add(group);
                ToggleEventSystem(false);
            }
            group.gameObject.SetActive(true);
            return DOVirtual.Float(group.alpha, active ? 1 : 0, duration, (a) => group.alpha = a).OnComplete(() =>
            {
                group.blocksRaycasts = active;
                if (disableUIEvents)
                {
                    s_EventsBlocks.Remove(group);
                    if (s_EventsBlocks.Count == 0)
                        ToggleEventSystem(true);
                }
                group.gameObject.SetActive(active);
                onComplete?.Invoke();
            }).SetUpdate(true).SetEase(active ? Ease.OutSine : Ease.InSine);
        }

        public static void ToggleEventSystem(bool enabled)
        {
            eventSystem.enabled = enabled;
        }
    }
}
