using Metroidvania.InputSystem;
using Metroidvania.UI.Menus;
using UnityEngine;

namespace Metroidvania {
    public class GamePauseManager : MonoBehaviour {
        [SerializeField] private GameplayMenuChannel m_pauseMenuChannel;

        private void OnEnable() {
            InputReader.instance.PauseEvent += PauseGame;
            m_pauseMenuChannel.MenuReleased += ResumeGame;
        }

        private void OnDisable() {
            InputReader.instance.PauseEvent -= PauseGame;
            m_pauseMenuChannel.MenuReleased -= ResumeGame;
        }

        private void PauseGame() {
            GameManager.instance.PauseGame();
            StartCoroutine(m_pauseMenuChannel.LoadMenu());
        }

        private void ResumeGame() {
            GameManager.instance.ResumeGame();
        }
    }
}