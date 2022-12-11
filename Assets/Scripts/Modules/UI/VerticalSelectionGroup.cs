using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania {
    public class VerticalSelectionGroup : MonoBehaviour {
        [SerializeField] private bool m_updateOnStart;
        [SerializeField] private Selectable[] m_selectables;

        public Selectable[] selectables => m_selectables;
        public bool updateOnStart => m_updateOnStart;

        private void Start() {
            if (m_updateOnStart)
                UpdateNavigation();
        }

        public void UpdateNavigation() {
            if (!IsValid())
                return;

            int lastIndex = m_selectables.Length - 1;
            for (int i = 0; i < m_selectables.Length; i++) {
                Selectable selectable = m_selectables[i];
                Selectable up = m_selectables[i == 0 ? lastIndex : i - 1];
                Selectable down = m_selectables[i == lastIndex ? 0 : i + 1];

                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnDown = down;
                navigation.selectOnUp = up;
                selectable.navigation = navigation;
            }
        }

        public bool IsValid() => m_selectables.Length >= 2;
    }
}
