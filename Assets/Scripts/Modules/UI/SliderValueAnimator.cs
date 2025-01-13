using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderValueAnimator : MonoBehaviour
    {
        [SerializeField] private Ease m_ease;
        [SerializeField] private float m_duration;

        private Slider _slider;

        public Slider slider
        {
            get
            {
                if (!_slider)
                    _slider = GetComponent<Slider>();
                return _slider;
            }
        }

        public void Animate(float value)
        {
            slider.DOValue(value, m_duration).SetEase(m_ease).SetTarget(slider);
        }
    }
}
