using Metroidvania.Serialization.Menus;
using TMPro;
using UnityEngine;

namespace Metroidvania.UI.Menus
{
    // TODO: Add credits menu
    public class MainMenu : CanvasMenuBase
    {
        [Header("Menu Transition")]
        [SerializeField] private CanvasGroup m_mainTitleGroup;
        [SerializeField] private OptionsMenu m_optionsMenu;
        [SerializeField] private SaveSlotsMenu m_saveSlots;

        [SerializeField] private TextMeshProUGUI m_versionText;

        private void Awake()
        {
            m_versionText.text = Application.version;
            m_optionsMenu.OnMenuDisable += ActiveMenu;
            m_saveSlots.OnMenuDisable += ActiveMenu;
        }

        private void Start()
        {
            SetFirstSelected();
        }

        public void ShowOptions()
        {
            m_mainTitleGroup.DOFade(false, UIUtility.TransitionTime, m_optionsMenu.ActiveMenu);
        }

        public void ShowSaveSlots()
        {
            m_mainTitleGroup.DOFade(false, UIUtility.TransitionTime, m_saveSlots.ActiveMenu);
        }

        public void ActiveMenu()
        {
            m_mainTitleGroup.DOFade(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}