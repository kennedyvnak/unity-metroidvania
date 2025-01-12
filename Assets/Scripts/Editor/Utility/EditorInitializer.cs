using System.Collections.Generic;
using Metroidvania.InputSystem;
using Metroidvania.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MetroidvaniaEditor.Settings
{
    public static class EditorInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (EditorSceneManager.GetActiveScene().buildIndex == 0 || EditorSceneManager.sceneCount == 0)
                return;

            AsyncOperationHandle<IList<ScriptableObject>> scriptableSingletonsHandle =
                Addressables.LoadAssetsAsync<ScriptableObject>("Scriptable Singleton", singleton =>
                {
                    if (singleton is IInitializableSingleton initializableSingleton)
                        initializableSingleton.Initialize();
                });


            AsyncOperationHandle<IList<GameObject>> persistentSingletonsHandle =
                Addressables.LoadAssetsAsync<GameObject>("Persistent Singleton", persistentSingleton => GameObject.Instantiate(persistentSingleton));

            scriptableSingletonsHandle.WaitForCompletion();
            persistentSingletonsHandle.WaitForCompletion();

            InputReader.instance.EnableGameplayInput();
        }
    }
}
