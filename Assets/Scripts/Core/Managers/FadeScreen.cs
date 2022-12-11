using DG.Tweening;
using Metroidvania.UI;
using UnityEngine;

namespace Metroidvania {
    public class FadeScreen : SingletonPersistent<FadeScreen> {
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private CanvasGroup m_canvasGroup;

        public Canvas canvas => m_canvas;
        public CanvasGroup canvasGroup => m_canvasGroup;

        public Tweener DOFadeIn(float fadeDuration = UIUtility.TransitionTime) {
            return DOFade(fadeDuration, true);
        }

        public Tweener DOFadeOut(float fadeDuration = UIUtility.TransitionTime) {
            return DOFade(fadeDuration, false);
        }

        private Tweener DOFade(float duration, bool active) {
            return canvasGroup.FadeGroup(active, duration);
        }
    }
}