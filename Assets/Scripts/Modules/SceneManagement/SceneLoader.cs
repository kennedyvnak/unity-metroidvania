using DG.Tweening;
using Metroidvania.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Metroidvania.SceneManagement
{
    public class SceneLoader : SingletonPersistent<SceneLoader>
    {
        public struct SceneTransitionData
        {
            public const string UseGameDataKey = "k_UseGameData";
            public static readonly SceneTransitionData UseGameData = FromSpawnPoint(UseGameDataKey);

            public const string MainMenuGameKey = "k_MainMenu";
            public static readonly SceneTransitionData MainMenu = FromSpawnPoint(MainMenuGameKey);

            public const string GameOverKey = "k_GameOver";
            public static SceneTransitionData GameOver = FromSpawnPoint(GameOverKey);

#if UNITY_EDITOR
            public const string EditorInitializationKey = "k_EditorInitialization";
            public static readonly SceneTransitionData EditorInitialization = FromSpawnPoint(EditorInitializationKey);
#endif

            public string spawnPoint;
            public GameData gameData;
            public SceneChannel currentScene;

            public static SceneTransitionData FromSpawnPoint(string spawnPoint)
            {
                return new SceneTransitionData() { spawnPoint = spawnPoint };
            }
        }

        public struct SceneUnloadData
        {
            public GameData gameData;
            public SceneChannel currentScene;
            public SceneChannel nextScene;
        }

        [SerializeField] private AssetReferenceSceneChannel m_mainMenuRef;

        [Header("Transition")]
        [SerializeField] private GameObject m_loadScreenPrefab;

        [Header("Events")]
        [SerializeField] private SceneEventChannel m_sceneLoaded;
        [SerializeField] private SceneEventChannel m_beforeSceneUnload;

        private GameObject _loadScreenObj;

        public SceneChannel activeScene { get; private set; }
        private AsyncOperationHandle<SceneChannel> _sceneChannelAssetHandle;

        private List<ISceneTransistor> _sceneTransistors = new List<ISceneTransistor>();

        private void Start()
        {
            _loadScreenObj = Instantiate(m_loadScreenPrefab, FadeScreen.instance.canvas.transform);
            _loadScreenObj.SetActive(false);
        }

        private void OnApplicationQuit()
        {
            OnUnloadScene(null);
        }

        public void LoadMainMenu()
        {
            LoadScene(m_mainMenuRef, SceneTransitionData.MainMenu);
        }

        public Coroutine LoadScene(AssetReferenceSceneChannel channelRef, SceneTransitionData transitionData)
        {
            return StartCoroutine(DOSceneLoadWithTransition(channelRef, transitionData));
        }

        public Coroutine LoadSceneWithoutTransition(AssetReferenceSceneChannel channelRef, SceneTransitionData transitionData)
        {
            return StartCoroutine(DoSceneLoad(channelRef, transitionData));
        }

        private IEnumerator DOSceneLoadWithTransition(AssetReferenceSceneChannel channelRef, SceneTransitionData transitionData)
        {
            _loadScreenObj.SetActive(true);
            yield return FadeScreen.instance.DOFadeIn().WaitForCompletion();
            yield return DoSceneLoad(channelRef, transitionData);
            yield return FadeScreen.instance.DOFadeOut().WaitForCompletion();
            _loadScreenObj.SetActive(false);
        }

        private IEnumerator DoSceneLoad(AssetReferenceSceneChannel sceneRef, SceneTransitionData transitionData)
        {
            SceneChannel scene = null;
            var op = sceneRef.LoadAssetAsync<SceneChannel>();
            yield return op;
            scene = op.Result;

            if (activeScene?.operation.IsValid() == true)
                OnUnloadScene(scene);

            if (_sceneChannelAssetHandle.IsValid())
                Addressables.ReleaseInstance(_sceneChannelAssetHandle);

            activeScene = scene;
            transitionData.currentScene = activeScene;
            transitionData.gameData = DataManager.instance.gameData;

            AsyncOperationHandle<SceneInstance> handle = scene.sceneReference.LoadSceneAsync();
            scene.operation = handle;

            handle.Completed += (op) =>
            {
                foreach (GameObject root in op.Result.Scene.GetRootGameObjects())
                    _sceneTransistors.AddRange(root.GetComponentsInChildren<ISceneTransistor>(true));

                _sceneTransistors.ForEach(sceneTransistor => sceneTransistor.OnSceneTransition(transitionData));

                m_sceneLoaded?.Raise(scene);
            };

            while (!handle.IsDone) yield return null;
        }

        private void OnUnloadScene(SceneChannel nextScene)
        {
            SceneUnloadData unloadData = new SceneUnloadData()
            {
                gameData = DataManager.instance.gameData,
                currentScene = activeScene,
                nextScene = nextScene,
            };
            _sceneTransistors.ForEach(sceneTransistor => sceneTransistor.BeforeUnload(unloadData));

            m_beforeSceneUnload?.Raise(activeScene);

            _sceneTransistors.Clear();
        }
    }
}
