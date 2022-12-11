using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using Metroidvania.Settings;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MetroidvaniaEditor.Settings {
    public static class EditorInitializer {
        private static string oldScenePath;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            if (EditorSceneManager.GetActiveScene().buildIndex == 0 || EditorSceneManager.sceneCount == 0)
                return;

            GameInitializer.InitializationFinish += GameInitializationFinished;
            oldScenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private static void GameInitializationFinished() {
            InputReader.instance.EnableGameplayInput();
            DataManager.instance.ChangeSelectedUser(1 << 8);

            // Find all scene channels assets
            string[] channelsGUID = AssetDatabase.FindAssets($"t:{typeof(SceneChannel).Name}");
            string oldSceneGUID = AssetDatabase.AssetPathToGUID(oldScenePath);
            foreach (string channelGUID in channelsGUID) {
                // Load scene channel and compare if chanel.scene.guid equals oldScene.guid
                SceneChannel channel = AssetDatabase.LoadAssetAtPath<SceneChannel>(AssetDatabase.GUIDToAssetPath(channelGUID));
                if (channel.sceneReference.AssetGUID.Equals(oldSceneGUID)) {
                    SceneLoader.instance.LoadSceneWithoutTransition(new AssetReferenceSceneChannel(channelGUID), SceneLoader.SceneTransitionData.EditorInitialization);
                    return;
                }
            }

            // If no scene channel is loaded, load the scene without using SceneLoader
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings.FindAssetEntry(oldSceneGUID) != null)
                Addressables.LoadSceneAsync(new AssetReference(oldSceneGUID));
            else
                SceneManager.LoadScene(AssetDatabase.GUIDToAssetPath(oldSceneGUID), LoadSceneMode.Single);

            GameInitializer.InitializationFinish -= GameInitializationFinished;
        }
    }
}