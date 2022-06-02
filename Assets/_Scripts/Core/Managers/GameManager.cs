using Metroidvania.Events;
using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        [SerializeField] private VoidEventChannel m_gamePausedChannel;

        [SerializeField] private VoidEventChannel m_gameResumedChannel;

        public bool gameIsPaused { get; private set; }

        public void Initialize()
        {
            var go = gameObject;
            if (instance)
            {
                Destroy(go);
                return;
            }
            instance = this;
            go.name = "[GameManager]";
            DontDestroyOnLoad(go);
        }

        public void PauseGame()
        {
            if (gameIsPaused) return;
            InputReader.instance.EnableMenuInput();
            Time.timeScale = 0;
            gameIsPaused = true;
            m_gamePausedChannel?.Raise();
        }

        public void ResumeGame()
        {
            if (!gameIsPaused) return;
            InputReader.instance.EnableGameplayInput();
            Time.timeScale = 1;
            gameIsPaused = false;
            m_gameResumedChannel?.Raise();
        }
    }
}