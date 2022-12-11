using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.RuntimeFields {
    /// <summary>Update = the value is updated in runtime; Setup = the value is updated by a saved data.</summary>
    public enum RuntimeFieldSetMode { Update, Setup }

    public class RuntimeField<T> : ScriptableObject {
        [SerializeField] private T m_defaultValue;

        [System.NonSerialized] private T _value;

        public T defaultValue => m_defaultValue;

        public T value {
            get => _value;
            set => SetValue(value, RuntimeFieldSetMode.Update);
        }

        public void SetValue(T value, RuntimeFieldSetMode setMode) {
            _value = value;
            ValueChanged?.Invoke(_value, setMode);
        }

        public event UnityAction<T, RuntimeFieldSetMode> ValueChanged;

        protected virtual void OnEnable() {
            _value = m_defaultValue;
        }

        protected void RaiseEvent(T newValue, RuntimeFieldSetMode setMode) => ValueChanged?.Invoke(newValue, setMode);
    }
}