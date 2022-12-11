using Metroidvania.Events;
using UnityEngine;
using UnityEngine.Localization;

namespace Metroidvania.Audio {
    public class GameGraphicsSettings : ScriptableSingleton<GameGraphicsSettings> {
        [Header("Resolution")] public string resolutionPrefsKey = "Selected-Resolution";
        public int defaultResolutionIndex;
        public Vector2Int[] screenResolutions;

        [Header("Quality")] public string qualityPrefsKey = "Selected-Quality";
        public int defaultQualityIndex;
        public LocalizedString[] qualitiesNames;

        [Header("Full Screen")] public string fullScreenPrefsKey = "Is-Full-Screen";
        public bool defaultFullScreenValue;

        [Header("Events")]
        public IntEventChannel changeResolutionChannel;
        public IntEventChannel changeQualityChannel;
        public BoolEventChannel changeFullScreenChannel;

        public void SetResolution(int idx, bool setPrefs) {
            idx = Mathf.Clamp(idx, 0, screenResolutions.Length);
            Vector2Int resolution = screenResolutions[idx];
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
            if (setPrefs)
                PlayerPrefs.SetInt(resolutionPrefsKey, idx);
            changeResolutionChannel?.Raise(idx);
        }

        public void SetQuality(int idx, bool setPrefs) {
            idx = Mathf.Clamp(idx, 0, qualitiesNames.Length);
            QualitySettings.SetQualityLevel(idx);
            if (setPrefs)
                PlayerPrefs.SetInt(qualityPrefsKey, idx);
            changeQualityChannel?.Raise(idx);
        }

        public void SetFullScreen(bool isFullScreen, bool setPrefs) {
            Screen.fullScreen = isFullScreen;
            if (setPrefs)
                PlayerPrefs.SetInt(fullScreenPrefsKey, isFullScreen ? 1 : 0);
            changeFullScreenChannel?.Raise(isFullScreen);
        }

        public void ToggleFullScreen(bool isFullScreen) => SetFullScreen(isFullScreen, true);

        public void StepResolution() => SetResolution((PlayerPrefs.GetInt(resolutionPrefsKey, defaultResolutionIndex) + 1) % screenResolutions.Length, true);

        public void StepQuality() => SetQuality((PlayerPrefs.GetInt(qualityPrefsKey, defaultQualityIndex) + 1) % qualitiesNames.Length, true);

#if UNITY_EDITOR
        private void OnValidate() {
            if (qualitiesNames.Length != QualitySettings.names.Length)
                GameDebugger.LogError($"{nameof(qualitiesNames)} length isn't equal the built-in quality names length.");
        }
#endif
    }
}