using Metroidvania.Serialization.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.Serialization
{
    [CreateAssetMenu(fileName = "new Data Manager", menuName = "Scriptables/Serialization/Data Manager")]
    public class DataManager : ScriptableObject
    {
        public GameDataAsset defaultGameDataAsset;

        private readonly DataHandler s_dataHandler = new FileDataHandler();
        public DataHandler dataHandler => s_dataHandler;

        private GameData _gameData;
        private readonly List<IDataPersistance> _persistenceObjects = new List<IDataPersistance>();

        public int selectedUserId { get; private set; }

        public void Initialize()
        {
            DisposeDataInstance();
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;

            Application.quitting += ApplicationQuit;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            _persistenceObjects.Clear();
            foreach (GameObject root in scene.GetRootGameObjects())
                _persistenceObjects.AddRange(root.GetComponentsInChildren<IDataPersistance>());
            DeserializePersistances();
        }

        private void OnSceneUnload(Scene scene)
        {
            SerializePersistances();
        }

        private void ApplicationQuit()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
            if (selectedUserId != -1)
                SerializePersistances();
        }

        public void DisposeDataInstance()
        {
            ChangeSelectedUser(-1);
        }

        public void ChangeSelectedUser(int newUserId)
        {
            selectedUserId = newUserId;
            if (selectedUserId != -1)
                _gameData = s_dataHandler.Deserialize(newUserId) ?? defaultGameDataAsset.GenerateNew(newUserId);
            else _gameData = null;
        }

        public void DeserializePersistances()
        {
            if (selectedUserId == -1) return;

            foreach (IDataPersistance persistanceObject in _persistenceObjects)
                persistanceObject.LoadData(_gameData);
        }

        public void SerializePersistances()
        {
            if (selectedUserId == -1) return;

            foreach (IDataPersistance persistanceObject in _persistenceObjects)
                persistanceObject.SaveData(_gameData);

            _gameData.lastSerialization = DateTime.Now.ToBinary();

            s_dataHandler.Serialize(_gameData);
        }
    }
}