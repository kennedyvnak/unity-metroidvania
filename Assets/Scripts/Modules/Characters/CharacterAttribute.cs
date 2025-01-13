using System;

namespace Metroidvania
{
    [Serializable]
    public class CharacterAttributeData<T>
    {
        public T startValue, stepPerLevel;
    }

    public class CharacterAttribute<T> where T : IEquatable<T>
    {
        private T _currentValue;
        private int _currentLevel = 0;

        public T currentValue
        {
            get => _currentValue;
            set
            {
                var oldVal = _currentValue;
                _currentValue = value;
                if (!oldVal.Equals(_currentValue))
                {
                    OnValueChanged?.Invoke(_currentValue);
                }
            }
        }
        public int currentLevel
        {
            get => _currentLevel;
            set
            {
                var oldVal = _currentLevel;
                _currentLevel = value;
                if (_currentLevel != oldVal)
                {
                    maxValue = getMaxValue(this);
                    OnLevelChanged?.Invoke(value);
                }
            }
        }

        public T maxValue { get; private set; }

        public readonly CharacterAttributeData<T> data;
        public readonly Func<CharacterAttribute<T>, T> getMaxValue;

        public event Action<T> OnValueChanged;
        public event Action<int> OnLevelChanged;

        public CharacterAttribute(CharacterAttributeData<T> atData, Func<CharacterAttribute<T>, T> getMaxValueFunc)
        {
            data = atData;
            getMaxValue = getMaxValueFunc;

            maxValue = getMaxValueFunc(this);
            _currentValue = maxValue;
        }
    }
}