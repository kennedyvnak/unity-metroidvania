using Metroidvania.InputSystem;
using Metroidvania.UI;
using Metroidvania.UI.Menus;
using UnityEngine;

namespace Metroidvania.Serialization.Menus
{
    public class SaveSlotsMenu : CanvasMenuBase, IMenuScreen
    {
        [SerializeField] private CanvasGroup m_canvasGroup;

        public event System.Action OnMenuDisable;
        private SaveSlot[] _saveSlots;

        private void Start()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>();

            foreach (SaveSlot saveSlot in _saveSlots)
            {
                GameData saveUserData = DataManager.dataHandler.Deserialize(saveSlot.GetUserId());
                saveSlot.SetData(saveUserData);
                saveSlot.button.onClick.AddListener(() => OnSaveSlotClick(saveSlot));
            }
        }

        public void OnSaveSlotClick(SaveSlot saveSlot)
        {
            DataManager.ChangeSelectedUser(saveSlot.GetUserId());
            GameData slotData = saveSlot.GetData();
            if (slotData != null)
                ContinueGame(slotData);
            else
                NewGame(saveSlot.GetUserId());
        }

        private void NewGame(int userId)
        {
            // new game operations
            if (GameDebugger.instance.debugSerialization)
                GameDebugger.Log($"Started a new game at user {userId}");

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level0");
            InputReader.instance.EnableGameplayInput();
        }

        private void ContinueGame(GameData data)
        {
            // load game operations
            if (GameDebugger.instance.debugSerialization)
                GameDebugger.Log($"Continued a game {data.userId}");

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level0");
            InputReader.instance.EnableGameplayInput();
        }

        public void ActiveMenu()
        {
            menuEnabled = true;
            m_canvasGroup.DOFade(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu()
        {
            menuEnabled = false;
            m_canvasGroup.DOFade(false, UIUtility.TransitionTime, () => OnMenuDisable?.Invoke());
        }
    }
}