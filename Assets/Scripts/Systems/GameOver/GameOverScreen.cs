using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using Metroidvania.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.GameOver {
    public class GameOverScreen : MonoBehaviour {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button leaveButton;

        private void Start() {
            InputReader.instance.EnableMenuInput();
            continueButton.onClick.AddListener(Continue);
            leaveButton.onClick.AddListener(Leave);
            UIUtility.eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }

        private void Continue() {
            DataManager.instance.gameData.LoadCurrentScene();
        }

        private void Leave() {
            SceneLoader.instance.LoadMainMenu();
        }
    }
}
