using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.RuntimeFields.Watchers {
    public class RuntimeFloatWatcher : MonoBehaviour {
        [SerializeField] private RuntimeFloatField m_field;
        [SerializeField] private bool m_updateOnEnable = true;

        [Space]
        [SerializeField] private UnityEvent<float, RuntimeFieldSetMode> m_valueChanged;

        public UnityEvent<float, RuntimeFieldSetMode> valueChanged => m_valueChanged;

        private void Start() {
        }

        private void OnEnable() {
            if (m_field) {
                m_field.ValueChanged += ValueChanged;
                if (m_updateOnEnable)
                    m_valueChanged?.Invoke(m_field.value, RuntimeFieldSetMode.Setup);
            }
        }

        private void OnDisable() {
            if (m_field)
                m_field.ValueChanged -= ValueChanged;
        }

        private void ValueChanged(float newValue, RuntimeFieldSetMode setMode) {
            m_valueChanged?.Invoke(newValue, setMode);
        }
    }
}