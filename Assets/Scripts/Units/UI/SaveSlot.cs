using Metroidvania.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.Serialization.Menus {
    public class SaveSlot : MonoBehaviour {
        [Header("User")]
        [SerializeField] private int m_userId;

        [Header("Content")]
        [SerializeField] private GameObject m_noDataContent;
        [SerializeField] private GameObject m_hasDataContent;
        [SerializeField] private Button m_button;

        public Button button => m_button;

        private GameData _data;

        public void SetData(GameData data) {
            _data = data;
            if (data == null) {
                m_hasDataContent.SetActive(false);
                m_noDataContent.SetActive(true);
            } else {
                m_noDataContent.SetActive(false);
                m_hasDataContent.SetActive(true);
            }
        }

        public GameData GetData() => _data;

        public int GetUserId() => m_userId;

        public void DeleteData() {
            if (_data == null)
                return;

            DataManager.instance.DeleteUser(GetUserId());
            SetData(null);
            UIUtility.eventSystem.SetSelectedGameObject(button.gameObject);
        }
    }
}