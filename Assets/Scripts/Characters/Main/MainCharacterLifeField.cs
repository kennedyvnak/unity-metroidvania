using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.Characters
{
    [CreateAssetMenu(menuName = "Scriptables/Characters/Life Field")]
    public class MainCharacterLifeField : ScriptableObject
    {
        public float maxLife;

        [System.NonSerialized] private float _currentLife;

        public float currentLife
        {
            get => _currentLife;
            set => SetLife(value, RuntimeFields.RuntimeFieldSetMode.Update);
        }

        public event UnityAction<float, RuntimeFields.RuntimeFieldSetMode> LifeChanged;
        public event UnityAction<float> MaxLifeChanged;

        private void OnEnable()
        {
            _currentLife = maxLife;
        }

        public void SetLife(float life, RuntimeFields.RuntimeFieldSetMode setMode)
        {
            _currentLife = life;
            LifeChanged?.Invoke(_currentLife, setMode);
        }

        public void SetMaxLife(float max)
        {
            maxLife = max;
            MaxLifeChanged?.Invoke(max);
        }
    }
}