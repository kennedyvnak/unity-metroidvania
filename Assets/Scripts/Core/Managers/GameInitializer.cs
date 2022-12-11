using Metroidvania.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Metroidvania.Settings {
    public class GameInitializer : MonoBehaviour {
#if UNITY_EDITOR
        public static event System.Action InitializationFinish;
#endif

        [Header("Scenes")]
        [SerializeField] private AssetReferenceSceneChannel m_mainMenuSceneRef;

        private IEnumerator Start() {
            if (!m_mainMenuSceneRef.RuntimeKeyIsValid()) {
                Debug.LogError("Error on game initialization. Exiting the application.");
#if UNITY_EDITOR
                Debug.Break();
#else
                Application.Quit();
#endif
            }

            AsyncOperationHandle<IList<ScriptableObject>> scriptableSingletonsHandle =
                Addressables.LoadAssetsAsync<ScriptableObject>("Scriptable Singleton", singleton => {
                    if (singleton is IInitializableSingleton initializableSingleton)
                        initializableSingleton.Initialize();
                });

            yield return scriptableSingletonsHandle;

            AsyncOperationHandle<IList<GameObject>> persistentSingletonsHandle =
                Addressables.LoadAssetsAsync<GameObject>("Persistent Singleton", persistentSingleton => Instantiate(persistentSingleton));

            yield return persistentSingletonsHandle;

#if UNITY_EDITOR
            if (InitializationFinish != null) {
                InitializationFinish.Invoke();
                yield break;
            }
#endif
            // Prevents SceneLoader._progressSlider null exception
            yield return null;
            yield return SceneLoader.instance.LoadSceneWithoutTransition(m_mainMenuSceneRef, SceneLoader.SceneTransitionData.MainMenu);
        }
    }

    public interface IInitializableSingleton {
        void Initialize();
    }
}
