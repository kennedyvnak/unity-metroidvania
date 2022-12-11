using Metroidvania.RuntimeFields;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI {
    [RequireComponent(typeof(Slider))]
    public class SliderValueAnimator : MonoBehaviour {
        [SerializeField] private float m_animationScale = 1;

        private Coroutine _animationCoroutine;
        private Slider _slider;

        public Slider slider {
            get {
                if (!_slider)
                    _slider = GetComponent<Slider>();
                return _slider;
            }
        }

        public void SetValue(float value, RuntimeFieldSetMode setMode) {
            this.EnsureStopCoroutine(ref _animationCoroutine);

            switch (setMode) {
                case RuntimeFieldSetMode.Update:
                    _animationCoroutine = StartCoroutine(DOAnimation(value));
                    break;
                case RuntimeFieldSetMode.Setup:
                    slider.value = value;
                    break;
            }
        }

        private IEnumerator DOAnimation(float targetValue) {
            float startValue = slider.value;
            float time = 0f;
            while (slider.value != targetValue) {
                time += Time.deltaTime * m_animationScale;
                float normalizedProgress = Mathf.Clamp01(time);
                slider.value = Mathf.Lerp(startValue, targetValue, normalizedProgress);
                yield return null;
            }
        }
    }
}