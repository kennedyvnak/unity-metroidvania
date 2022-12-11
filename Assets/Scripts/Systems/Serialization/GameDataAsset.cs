using UnityEngine;

namespace Metroidvania.Serialization {
    public class GameDataAsset : ScriptableObject {
        [SerializeField] private GameData m_gameData;

        public GameData gameData { get => m_gameData; set => m_gameData = value; }

        public void LoadFromJson(string json) {
            m_gameData = JsonUtility.FromJson<GameData>(json);
        }

        public string ToJson() => JsonUtility.ToJson(m_gameData);

        public GameData GenerateNew(int userId) {
            GameData gameData = this.gameData.Clone();
            gameData.userId = userId;
            gameData.lastSerialization = 0;
            return gameData;
        }
    }
}