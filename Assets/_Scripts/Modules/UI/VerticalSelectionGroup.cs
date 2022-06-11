using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania
{
    public class VerticalSelectionGroup : MonoBehaviour
    {
        [SerializeField] private bool m_updateOnStart;
        [SerializeField] private Selectable[] m_selectables;

        public Selectable[] selectables => m_selectables;
        public bool updateOnStart => m_updateOnStart;

        private void Start()
        {
            if (m_updateOnStart)
                UpdateNavigation();
        }

        public void UpdateNavigation()
        {
            if (!IsValid()) return;

            var lastIndex = m_selectables.Length - 1;
            for (int i = 0; i < m_selectables.Length; i++)
            {
                var selectable = m_selectables[i];
                var up = m_selectables[i == 0 ? lastIndex : i - 1];
                var down = m_selectables[i == lastIndex ? 0 : i + 1];

                selectable.navigation = new Navigation()
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnDown = down,
                    selectOnUp = up,
                };
            }
        }

        public bool IsValid() => m_selectables.Length >= 2;
    }
}
