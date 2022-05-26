using UnityEngine;
using UnityEngine.Localization;

namespace Metroidvania.Audio
{
    [ResourceObjectPath("Settings/Graphics Settings")]
    public class GameGraphicsSettings : ScriptableSingleton<GameGraphicsSettings>
    {
        [Header("Resolution")] public string resolutionPrefsKey = "Selected-Resolution";
        public int defaultResolutionIndex;
        public Vector2Int[] screenResolutions;

        public static event System.Action<int> ResolutionChanged;

        [Header("Quality")] public string qualityPrefsKey = "Selected-Quality";
        public int defaultQualityIndex;
        public LocalizedString[] qualitiesNames;

        public static event System.Action<int> QualityChanged;

        [Header("Full Screen")] public string fullScreenPrefsKey = "Is-Full-Screen";
        public bool defaultFullScreenValue;

        public static event System.Action<bool> FullScreenChanged;

        public void SetResolution(int idx, bool setPrefs)
        {
            idx = Mathf.Clamp(idx, 0, screenResolutions.Length);
            var resolution = screenResolutions[idx];
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
            if (setPrefs)
                PlayerPrefs.SetInt(resolutionPrefsKey, idx);
            ResolutionChanged?.Invoke(idx);
        }

        public void SetQuality(int idx, bool setPrefs)
        {
            idx = Mathf.Clamp(idx, 0, qualitiesNames.Length);
            QualitySettings.SetQualityLevel(idx);
            if (setPrefs)
                PlayerPrefs.SetInt(qualityPrefsKey, idx);
            QualityChanged?.Invoke(idx);
        }

        public void SetFullScreen(bool isFullScreen, bool setPrefs)
        {
            Screen.fullScreen = isFullScreen;
            if (setPrefs)
                PlayerPrefs.SetInt(fullScreenPrefsKey, isFullScreen ? 1 : 0);
            FullScreenChanged?.Invoke(isFullScreen);
        }

        public void ToggleFullScreen(bool isFullScreen) => SetFullScreen(isFullScreen, true);

        public void StepResolution() => SetResolution((PlayerPrefs.GetInt(resolutionPrefsKey, defaultResolutionIndex) + 1) % screenResolutions.Length, true);

        public void StepQuality() => SetQuality((PlayerPrefs.GetInt(qualityPrefsKey, defaultQualityIndex) + 1) % qualitiesNames.Length, true);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (qualitiesNames.Length != QualitySettings.names.Length)
                Debug.LogError($"{nameof(qualitiesNames)} lenght isn't equal the built-in quality names lenght.");
        }
#endif
    }
}