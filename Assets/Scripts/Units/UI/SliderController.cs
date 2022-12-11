using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI {
    public class SliderController : MonoBehaviour {
        [SerializeField] private Slider m_slider;

        [SerializeField] private Button m_leftButton;
        [SerializeField] private Button m_rightButton;

        private void Start() {
            m_leftButton.onClick.AddListener(StepValueToLeft);
            m_rightButton.onClick.AddListener(StepValueToRight);
        }

        public void StepValueToLeft() {
            float v = GetNormalizedSliderValue();
            m_slider.value = v - (m_slider.maxValue * 0.1f);
        }

        public void StepValueToRight() {
            float v = GetNormalizedSliderValue();
            m_slider.value = v + (m_slider.maxValue * 0.1f);
        }

        private float GetNormalizedSliderValue() {
            return Mathf.Round(m_slider.normalizedValue * (m_slider.maxValue * 10f)) * 0.1f;
        }
    }
}