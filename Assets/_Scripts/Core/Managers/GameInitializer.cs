using System.Collections;
using Metroidvania.Audio;
using Metroidvania.Localization;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

namespace Metroidvania.Settings
{
    public class GameInitializer : MonoBehaviour
    {
#if UNITY_EDITOR
        public static event System.Action InitializationFinish;
#endif

        [Header("Scenes")]
        [SerializeField] private AssetReference m_mainMenuSceneRef;

        [Header("Settings")]
        [SerializeField] private AssetReference m_dataManager;
        [SerializeField] private AssetReference m_audioSettings;
        [SerializeField] private AssetReference m_localizationSettings;

        [Header("Managers")]
        [SerializeField] private AssetReference m_gameManager;
        [SerializeField] private AssetReference m_sceneLoader;

        //
        private IEnumerator Start()
        {
            if (!m_mainMenuSceneRef.RuntimeKeyIsValid() || !m_dataManager.RuntimeKeyIsValid()
               || !m_audioSettings.RuntimeKeyIsValid() || !m_localizationSettings.RuntimeKeyIsValid()
               || !m_gameManager.RuntimeKeyIsValid() || !m_sceneLoader.RuntimeKeyIsValid())
            {
                Debug.LogError("Error on game initialization. Exiting the application.");
#if UNITY_EDITOR
                Debug.Break();
#else
                Application.Quit();
#endif
            }

            var audioSettingsHandle = m_audioSettings.LoadAssetAsync<GameAudioSettings>();
            yield return audioSettingsHandle;
            audioSettingsHandle.Result.LoadPrefs();

            var localizationSettingsHandle = m_localizationSettings.LoadAssetAsync<GameLocalizationSettings>();
            yield return LocalizationSettings.InitializationOperation;
            yield return localizationSettingsHandle;
            localizationSettingsHandle.Result.LoadPrefs();

            var dataManagerHandle = m_dataManager.LoadAssetAsync<DataManager>();
            yield return dataManagerHandle;
            dataManagerHandle.Result.Initialize();

            var gameManagerHandle = m_gameManager.InstantiateAsync();
            yield return gameManagerHandle;
            gameManagerHandle.Result.GetComponent<GameManager>().Initialize();

            var sceneLoaderHandle = m_sceneLoader.InstantiateAsync();
            yield return sceneLoaderHandle;
            sceneLoaderHandle.Result.GetComponent<SceneLoader>().Initialize();

            if (InitializationFinish != null)
            {
                InitializationFinish.Invoke();
                yield break;
            }

            yield return Addressables.LoadSceneAsync(m_mainMenuSceneRef);
        }
    }
}
