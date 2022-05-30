using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania
{
    public static class GameManager
    {
        public static System.Action GamePaused;
        public static System.Action GameResumed;

        public static bool gameIsPaused { get; private set; }

        public static void PauseGame()
        {
            if (gameIsPaused) return;
            InputReader.instance.EnableMenuInput();
            Time.timeScale = 0;
            gameIsPaused = true;
            GamePaused?.Invoke();
        }

        public static void ResumeGame()
        {
            if (!gameIsPaused) return;
            InputReader.instance.EnableGameplayInput();
            Time.timeScale = 1;
            gameIsPaused = false;
            GameResumed?.Invoke();
        }
    }
}