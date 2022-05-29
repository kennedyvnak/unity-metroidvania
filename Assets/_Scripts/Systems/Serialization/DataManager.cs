using Metroidvania.Serialization.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.Serialization
{
    public static class DataManager
    {
        public const string DefaultDataPath = "Data/Default Game Data";

        private static GameDataAsset _defaultGameDataAsset;
        public static GameDataAsset defaultGameDataAsset
        {
            get
            {
                if (_defaultGameDataAsset != null) return _defaultGameDataAsset;
                return _defaultGameDataAsset = UnityEngine.Resources.Load<GameDataAsset>(DefaultDataPath);
            }
        }

        private static readonly DataHandler s_dataHandler = new FileDataHandler();
        public static DataHandler dataHandler => s_dataHandler;

        private static GameData _gameData;
        private static readonly List<IDataPersistance> _persistenceObjects = new List<IDataPersistance>();

        public static int selectedUserId { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadInstance()
        {
            DisposeDataInstance();
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;

            Application.quitting += ApplicationQuit;
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            _persistenceObjects.Clear();
            foreach (GameObject root in scene.GetRootGameObjects())
                _persistenceObjects.AddRange(root.GetComponentsInChildren<IDataPersistance>());
            DeserializePersistances();
        }

        private static void OnSceneUnload(Scene scene)
        {
            SerializePersistances();
        }

        private static void ApplicationQuit()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
            if (selectedUserId != -1)
                SerializePersistances();
        }

        public static void DisposeDataInstance()
        {
            ChangeSelectedUser(-1);
        }

        public static void ChangeSelectedUser(int newUserId)
        {
            selectedUserId = newUserId;
            if (selectedUserId != -1)
                _gameData = s_dataHandler.Deserialize(newUserId) ?? defaultGameDataAsset.GenerateNew(newUserId);
            else _gameData = null;
        }

        public static void DeserializePersistances()
        {
            foreach (IDataPersistance persistanceObject in _persistenceObjects)
                persistanceObject.LoadData(_gameData);
        }

        public static void SerializePersistances()
        {
            foreach (IDataPersistance persistanceObject in _persistenceObjects)
                persistanceObject.SaveData(_gameData);

            _gameData.lastSerialization = DateTime.Now.ToBinary();

            s_dataHandler.Serialize(_gameData);
        }
    }
}