using System.Collections;
using DG.Tweening;
using Metroidvania.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.UI.Menus
{
    public class GamePauseMenu : GameplayMenuInstance
    {
        [SerializeField] private CanvasGroup titleGroup;
        [SerializeField] private OptionsMenu optionsMenu;

        private void Awake()
        {
            optionsMenu.OnMenuDisable += ActiveMenu;
            InputReader.instance.ReturnEvent += PerformReturn;
        }

        private void OnDestroy()
        {
            InputReader.instance.ReturnEvent -= PerformReturn;
        }

        public void ActiveMenu()
        {
            menuEnabled = true;
            titleGroup.DOFade(true, UIUtility.TransitionTime);
        }

        public void Resume()
        {
            StartCoroutine(channel.UnloadMenuInstance());
        }

        public void OpenOptions()
        {
            menuEnabled = false;
            titleGroup.DOFade(false, UIUtility.TransitionTime, optionsMenu.ActiveMenu);
        }

        public void ExitGameplay()
        {
            GameManager.ResumeGame();
            SceneManager.LoadScene("MainMenu");
        }

        public void PerformReturn()
        {
            if (menuEnabled)
                Resume();
            else if (optionsMenu.menuEnabled)
                optionsMenu.DesactiveMenu();
        }

        public override IEnumerator InitOperation(GameplayMenuChannel channel)
        {
            this.channel = channel;
            menuEnabled = true;
            var t = titleGroup.DOFade(true, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
            SetFirstSelected();
        }

        public override IEnumerator ReleaseOperation()
        {
            var t = titleGroup.DOFade(false, UIUtility.TransitionTime);
            yield return t.WaitForCompletion();
        }
    }
}