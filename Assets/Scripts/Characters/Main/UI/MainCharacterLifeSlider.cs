using Metroidvania.RuntimeFields;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.Characters {
    [RequireComponent(typeof(Slider))]
    public class MainCharacterLifeSlider : MonoBehaviour {
        private Slider _slider;
        private Coroutine _animationCoroutine;

        [SerializeField] private MainCharacterLifeField m_LifeField;
        [SerializeField] private float m_AnimationScale = 1;

        private void Awake() {
            _slider = GetComponent<Slider>();
        }

        private void OnEnable() {
            _slider.maxValue = m_LifeField.maxLife;
            m_LifeField.LifeChanged += LifeChanged;
            m_LifeField.MaxLifeChanged += MaxLifeChanged;
        }

        private void OnDisable() {
            m_LifeField.LifeChanged -= LifeChanged;
            m_LifeField.MaxLifeChanged -= MaxLifeChanged;
        }

        private void LifeChanged(float life, RuntimeFieldSetMode setMode) {
            this.EnsureStopCoroutine(ref _animationCoroutine);

            switch (setMode) {
                case RuntimeFieldSetMode.Update:
                    _animationCoroutine = StartCoroutine(DOAnimation(life));
                    break;
                case RuntimeFieldSetMode.Setup:
                    _slider.value = life;
                    break;
            }
        }

        private void MaxLifeChanged(float maxLife) {
            _slider.maxValue = maxLife;
        }

        private IEnumerator DOAnimation(float targetValue) {
            float startValue = _slider.value;
            float time = 0f;
            while (_slider.value != targetValue) {
                time += Time.deltaTime * m_AnimationScale;
                float normalizedProgress = Mathf.Clamp01(time);
                _slider.value = Mathf.Lerp(startValue, targetValue, normalizedProgress);
                yield return null;
            }
        }
    }
}