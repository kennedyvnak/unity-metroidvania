using DG.Tweening;
using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Metroidvania.UI.Menus {
    public class GamePauseMenu : GameplayMenuInstance {
        [SerializeField] private CanvasGroup m_titleGroup;
        [SerializeField] private OptionsMenu m_optionsMenu;

        public IMenuScreen activeScreen { get; private set; }

        private void Awake() {
            m_optionsMenu.OnMenuDisable += ActiveMenu;
            InputReader.instance.MenuCloseEvent += PerformMenuClose;
        }

        private void OnDestroy() {
            InputReader.instance.MenuCloseEvent -= PerformMenuClose;
        }

        public void ActiveMenu() {
            menuEnabled = true;
            m_titleGroup.FadeGroup(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void Resume() {
            StartCoroutine(channel.UnloadMenuInstance());
        }

        public void OpenOptions() {
            SwitchToScreen(m_optionsMenu);
        }

        public void ExitGameplay() {
            GameManager.instance.ResumeGame();
            SceneLoader.instance.LoadMainMenu();
        }

        public void SwitchToScreen(IMenuScreen screen) {
            m_titleGroup.FadeGroup(false, UIUtility.TransitionTime, screen.ActiveMenu);
            menuEnabled = false;
            activeScreen = screen;
        }

        public void PerformMenuClose() {
            if (menuEnabled)
                Resume();
            else if (activeScreen != null)
                activeScreen.DesactiveMenu();
        }

        public override IEnumerator InitOperation(GameplayMenuChannel channel) {
            this.channel = channel;
            menuEnabled = true;
            Tweener t = m_titleGroup.FadeGroup(true, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
            SetFirstSelected();
        }

        public override IEnumerator ReleaseOperation() {
            Tweener t = m_titleGroup.FadeGroup(false, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
        }
    }
}