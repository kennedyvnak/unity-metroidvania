using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;

namespace Metroidvania.Serialization
{
    [System.Serializable]
    public class GameData : System.ICloneable
    {
        public int userId;
        public long lastSerialization;

        public CharacterSafePointData lastCharacterSafePoint;

        public float ch_knight_life;
        public bool ch_knight_died;
        public float ch_archer_life;

        public GameData Clone() => MemberwiseClone() as GameData;
        object System.ICloneable.Clone() => Clone();

        public void LoadCurrentScene()
        {
            InputReader.instance.EnableGameplayInput();
            SceneLoader.instance.LoadScene(new AssetReferenceSceneChannel(lastCharacterSafePoint.sceneGUID), SceneLoader.SceneTransitionData.GameOver);
        }
    }

    [System.Serializable]
    public struct CharacterSafePointData
    {
        public string sceneGUID;
        public SerializableGuid pointGUID;
    }
}
