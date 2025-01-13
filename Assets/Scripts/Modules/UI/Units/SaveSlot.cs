using Metroidvania.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Metroidvania.Serialization.Menus
{
    public class SaveSlot : MonoBehaviour
    {
        [Header("User")]
        [SerializeField] private int m_userId;

        [Header("Content")]
        [SerializeField] private LocalizedString m_noDataContent;
        [SerializeField] private LocalizedString m_hasDataContent;
        [SerializeField] private LocalizeStringEvent m_text;
        [SerializeField] private Button m_button, m_deleteButton;

        public Button button => m_button;

        private GameData _data;

        public void SetData(GameData data)
        {
            _data = data;
            if (data == null)
            {
                m_text.StringReference = m_noDataContent;
                m_deleteButton.gameObject.SetActive(false);
            }
            else
            {
                m_text.StringReference = m_hasDataContent;
                m_deleteButton.gameObject.SetActive(true);
            }
        }

        public GameData GetData() => _data;

        public int GetUserId() => m_userId;

        public void DeleteData()
        {
            if (_data == null)
                return;

            DataManager.instance.DeleteUser(GetUserId());
            SetData(null);
            Helpers.eventSystem.SetSelectedGameObject(button.gameObject);
        }
    }
}
