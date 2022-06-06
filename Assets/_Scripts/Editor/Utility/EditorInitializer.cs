using System;
using Metroidvania.InputSystem;
using Metroidvania.Serialization;
using Metroidvania.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MetroidvaniaEditor.Settings
{
    public static class EditorInitializer
    {
        static string oldSceneRef;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (EditorSceneManager.GetActiveScene().buildIndex == 0 || EditorSceneManager.sceneCount == 0)
                return;

            GameInitializer.InitializationFinish += GameInitializationFinished;
            oldSceneRef = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private static void GameInitializationFinished()
        {
            InputReader.instance.EnableGameplayInput();
            Addressables.LoadSceneAsync(new AssetReference(AssetDatabase.AssetPathToGUID(oldSceneRef)));
            GameInitializer.InitializationFinish -= GameInitializationFinished;
        }
    }
}