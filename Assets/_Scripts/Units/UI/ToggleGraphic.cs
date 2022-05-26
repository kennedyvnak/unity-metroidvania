using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI
{
    public class ToggleGraphic : MonoBehaviour
    {
        [Header("UI Fields")]
        [SerializeField] private Toggle m_toggle;
        [SerializeField] private Image m_display;
        
        [Header("Textures")]
        [SerializeField] private Sprite m_onSprite;
        [SerializeField] private Sprite m_offSprite;

        public Toggle toggle => m_toggle;
        public Image display => m_display;
        public Sprite onSprite => m_onSprite;
        public Sprite offSprite => m_offSprite;

        private void Awake()
        {
            UpdateGraphic();
        }

        private void OnEnable()
        {
            m_toggle.onValueChanged.AddListener(ValueChangedHandle);
        }

        private void OnDisable()
        {
            m_toggle.onValueChanged.RemoveListener(ValueChangedHandle);
        }

        private void ValueChangedHandle(bool isOn) => UpdateGraphic();

        public void UpdateGraphic()
        {
            var isOn = toggle.isOn;
            display.sprite = isOn ? onSprite : offSprite;
        }
    }
}