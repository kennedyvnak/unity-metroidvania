using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;

namespace Metroidvania.Serialization {
    public partial class GameData {
        public void LoadCurrentScene() {
            InputReader.instance.EnableGameplayInput();
            SceneLoader.instance.LoadScene(new AssetReferenceSceneChannel(lastCharacterSafePoint.sceneGUID), SceneLoader.SceneTransitionData.GameOver);
        }
    }
}