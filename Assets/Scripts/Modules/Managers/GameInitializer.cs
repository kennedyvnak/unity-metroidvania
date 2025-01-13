using Metroidvania.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Metroidvania.Settings
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private AssetReferenceSceneChannel m_mainMenuSceneRef;

        private IEnumerator Start()
        {
            if (!m_mainMenuSceneRef.RuntimeKeyIsValid())
            {
                Debug.LogError("Error on game initialization. Exiting the application.");
                Application.Quit();
            }

            AsyncOperationHandle<IList<ScriptableObject>> scriptableSingletonsHandle =
                Addressables.LoadAssetsAsync<ScriptableObject>("Scriptable Singleton", singleton =>
                {
                    if (singleton is IInitializableSingleton initializableSingleton)
                        initializableSingleton.Initialize();

                    var type = singleton.GetType();
                    var setInstanceMethod = type.BaseType.GetMethod("SetInstance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    setInstanceMethod.Invoke(null, new object[] { singleton });
                });

            AsyncOperationHandle<IList<GameObject>> persistentSingletonsHandle =
                Addressables.LoadAssetsAsync<GameObject>("Persistent Singleton", persistentSingleton => Instantiate(persistentSingleton));

            yield return persistentSingletonsHandle;
            yield return scriptableSingletonsHandle;

            yield return SceneLoader.instance.LoadSceneWithoutTransition(m_mainMenuSceneRef, SceneLoader.SceneTransitionData.MainMenu);
        }
    }

    public interface IInitializableSingleton
    {
        void Initialize();
    }
}
