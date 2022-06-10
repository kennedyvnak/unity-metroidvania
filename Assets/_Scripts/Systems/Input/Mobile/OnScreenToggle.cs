using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Metroidvania.InputSystem.Mobile
{
    [AddComponentMenu("Input/On-Screen Toggle")]
    public class OnScreenToggle : OnScreenControl, IPointerDownHandler
    {
        private bool _active;

        private void Start()
        {
            _active = m_defaultActiveValue;
            Toggle();
        }

        public void OnPointerDown(PointerEventData data)
        {
            Toggle();
        }

        public void Toggle()
        {
            SendValueToControl(_active ? 1f : 0f);
            m_onToggle?.Invoke(_active);
            _active = !_active;
        }

        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_controlPath;

        [SerializeField]
        private bool m_defaultActiveValue;

        [SerializeField]
        private UnityEvent<bool> m_onToggle;

        public UnityEvent<bool> OnToggle => m_onToggle;

        protected override string controlPathInternal
        {
            get => m_controlPath;
            set => m_controlPath = value;
        }
    }
}
