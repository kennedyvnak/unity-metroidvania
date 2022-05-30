using System.Collections;
using DG.Tweening;
using Metroidvania.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.UI.Menus
{
    public class GamePauseMenu : GameplayMenuInstance
    {
        [SerializeField] private CanvasGroup m_titleGroup;
        [SerializeField] private OptionsMenu m_optionsMenu;

        public IMenuScreen activeScreen { get; private set; }

        private void Awake()
        {
            m_optionsMenu.OnMenuDisable += ActiveMenu;
            InputReader.instance.ReturnEvent += PerformReturn;
        }

        private void OnDestroy()
        {
            InputReader.instance.ReturnEvent -= PerformReturn;
        }

        public void ActiveMenu()
        {
            menuEnabled = true;
            m_titleGroup.DOFade(true, UIUtility.TransitionTime);
        }

        public void Resume()
        {
            StartCoroutine(channel.UnloadMenuInstance());
        }

        public void OpenOptions()
        {
            SwitchToScreen(m_optionsMenu);
        }

        public void ExitGameplay()
        {
            GameManager.ResumeGame();
            SceneManager.LoadScene("MainMenu");
        }

        public void SwitchToScreen(IMenuScreen screen)
        {
            m_titleGroup.DOFade(false, UIUtility.TransitionTime, screen.ActiveMenu);
            menuEnabled = false;
            activeScreen = screen;
        }

        public void PerformReturn()
        {
            if (menuEnabled)
                Resume();
            else if (activeScreen != null)
                activeScreen.DesactiveMenu();
        }

        public override IEnumerator InitOperation(GameplayMenuChannel channel)
        {
            this.channel = channel;
            menuEnabled = true;
            var t = m_titleGroup.DOFade(true, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
            SetFirstSelected();
        }

        public override IEnumerator ReleaseOperation()
        {
            var t = m_titleGroup.DOFade(false, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
        }
    }
}