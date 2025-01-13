using DG.Tweening;
using Metroidvania.UI;
using UnityEngine;

namespace Metroidvania
{
    public class CharacterStatusBar : Singleton<CharacterStatusBar>
    {
        [SerializeField] private SliderValueAnimator m_lifeSlider;

        private CharacterAttribute<float> _life;

        public void ConnectLife(CharacterAttribute<float> life)
        {
            _life = life;

            life.OnValueChanged += LifeChanged;
            life.OnLevelChanged += LifeLevelChanged;
        }

        public void SetLife(float life)
        {
            m_lifeSlider.slider.DOKill();
            m_lifeSlider.slider.value = life;
        }

        private void LifeChanged(float value)
        {
            m_lifeSlider.Animate(value);
        }

        private void LifeLevelChanged(int level)
        {
            m_lifeSlider.slider.maxValue = _life.maxValue;
        }
    }
}
