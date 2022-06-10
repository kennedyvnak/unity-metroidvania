using System;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.InputSystem.Mobile
{
    [RequireComponent(typeof(OnScreenToggle))]
    public class MobileToggleInput : MonoBehaviour
    {
        private OnScreenToggle _toggle;
        [SerializeField] private Image m_graphic;

        [SerializeField] private Color m_disabledColor;
        [SerializeField] private Color m_enabledColor;

        private void Awake()
        {
            _toggle = GetComponent<OnScreenToggle>();
            _toggle.OnToggle.AddListener(OnToggle);
        }

        private void OnToggle(bool active)
        {
            m_graphic.color = active ? m_enabledColor : m_disabledColor;
        }
    }
}