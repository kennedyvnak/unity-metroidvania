using UnityEngine;

namespace Metroidvania.UI.Menus {
    public abstract class CanvasMenuBase : MonoBehaviour {
        [SerializeField] protected GameObject firstSelected;

        public bool menuEnabled { get; protected set; }

        public void SetFirstSelected() {
            UIUtility.eventSystem.SetSelectedGameObject(firstSelected);
        }
    }
}