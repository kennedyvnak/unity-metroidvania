using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.UI
{
    public static class UIUtility
    {
        private static Canvas s_mainCanvas;
        public static Canvas mainCanvas
        {
            get
            {
                if (s_mainCanvas == null) s_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
                return s_mainCanvas;
            }
        }
        public const float TransitionTime = .333F;

        private static readonly List<CanvasGroup> s_EventsBlocks = new List<CanvasGroup>();
        public static Tweener DOFade(this CanvasGroup group, bool active, float duration, TweenCallback onComplete = null, bool disableUIEvents = true)
        {
            if (disableUIEvents)
                s_EventsBlocks.Add(group);
            return DOVirtual.Float(group.alpha, active ? 1 : 0, duration, (a) => group.alpha = a).OnComplete(() =>
            {
                group.blocksRaycasts = active;
                group.interactable = active;
                if (disableUIEvents)
                    s_EventsBlocks.Remove(group);
                onComplete?.Invoke();
            }).SetUpdate(true);
        }
    }
}