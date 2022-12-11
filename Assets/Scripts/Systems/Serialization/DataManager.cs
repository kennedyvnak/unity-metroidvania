using Metroidvania.SceneManagement;
using Metroidvania.Serialization.Handlers;
using Metroidvania.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.Serialization {
    /// <summary>
    /// Singleton that handle all data in game
    /// </summary>
    [CreateAssetMenu(fileName = "new Data Manager", menuName = "Scriptables/Serialization/Data Manager")]
    public class DataManager : ScriptableSingleton<DataManager>, IInitializableSingleton {
#if UNITY_EDITOR
        // Game data used in games that was initialized without a save load
        private GameData _editorPlaceholderData;
#endif

        public GameDataAsset defaultGameDataAsset;
        public SceneEventChannel sceneUnloadedChannel;

        public GameDataEventChannel onDeserializeChannel;
        public GameDataEventChannel onSerializeChannel;

        private readonly DataHandler s_dataHandler = new FileDataHandler();
        public DataHandler dataHandler => s_dataHandler;

        private GameData _gameData;

        public GameData gameData {
            get {
                if (_gameData != null)
                    return _gameData;
#if UNITY_EDITOR
                return _editorPlaceholderData;
#else
                return null;
#endif
            }
        }

        public void DeleteUser(int userId) {
            s_dataHandler.DeleteUser(userId);
        }

        public int selectedUserId { get; private set; }

        public void Initialize() {
            DisposeDataInstance();
            SceneManager.sceneLoaded += OnSceneLoad;
            sceneUnloadedChannel.OnEventRaise += BeforeSceneUnload;
#if UNITY_EDITOR
            _editorPlaceholderData = defaultGameDataAsset.GenerateNew(1 << 8);
#endif
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
            onDeserializeChannel?.Raise(gameData);
        }

        private void BeforeSceneUnload(SceneChannel scene) {
            onSerializeChannel?.Raise(gameData);
            SerializeData();
        }

        public void SerializeData() {
            _gameData.lastSerialization = System.DateTime.Now.ToBinary();
            s_dataHandler.Serialize(_gameData);
        }

        public void DisposeDataInstance() {
            ChangeSelectedUser(-1);
        }

        public GameData ChangeSelectedUser(int newUserId) {
            selectedUserId = newUserId;
            _gameData = selectedUserId != -1 ? s_dataHandler.Deserialize(newUserId) ?? defaultGameDataAsset.GenerateNew(newUserId) : null;
            return _gameData;
        }
    }
}