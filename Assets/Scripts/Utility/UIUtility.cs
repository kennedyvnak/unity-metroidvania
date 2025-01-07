using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metroidvania.UI
{
    public static class UIUtility
    {
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
                ToggleEvents(false);
            }
            group.gameObject.SetActive(true);
            return DOVirtual.Float(group.alpha, active ? 1 : 0, duration, (a) => group.alpha = a).OnComplete(() =>
            {
                group.blocksRaycasts = active;
                if (disableUIEvents)
                {
                    s_EventsBlocks.Remove(group);
                    if (s_EventsBlocks.Count == 0)
                        ToggleEvents(true);
                }
                group.gameObject.SetActive(active);
                onComplete?.Invoke();
            }).SetUpdate(true).SetEase(active ? Ease.OutSine : Ease.InSine);
        }

        public static void ToggleEvents(bool enabled)
        {
            eventSystem.enabled = enabled;
        }
    }
}
