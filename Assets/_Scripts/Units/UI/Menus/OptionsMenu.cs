using Metroidvania.Audio;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI.Menus
{
    public class OptionsMenu : CanvasMenuBase
    {
        [SerializeField] private CanvasGroup m_canvasGroup;

        [Header("Audio Settings")]
        [SerializeField] private Slider m_masterVolumeSlider;
        [SerializeField] private Slider m_musicsVolumeSlider;
        [SerializeField] private Slider m_sfxVolumeSlider;

        [Header("Quality Settings")]
        [SerializeField] private TextMeshProUGUI m_resolutionText;
        [SerializeField] private LocalizeStringEvent m_qualityEvent;
        [SerializeField] private ToggleGraphic m_fullScreenToggle;

        public event System.Action OnMenuDisable;

        private void Awake()
        {
            m_masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.masterFieldName, 1));
            m_musicsVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.musicsFieldName, 1));
            m_sfxVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.sfxFieldName, 1));

            m_masterVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetMasterField);
            m_musicsVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetMusicsField);
            m_sfxVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetSfxField);

            m_fullScreenToggle.toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(GameGraphicsSettings.instance.fullScreenPrefsKey, GameGraphicsSettings.instance.defaultFullScreenValue ? 1 : 0) == 1);

            ResolutionChangedHandler(PlayerPrefs.GetInt(GameGraphicsSettings.instance.resolutionPrefsKey, GameGraphicsSettings.instance.defaultResolutionIndex));
            QualityChangedHandler(PlayerPrefs.GetInt(GameGraphicsSettings.instance.qualityPrefsKey, GameGraphicsSettings.instance.defaultQualityIndex));

            GameGraphicsSettings.ResolutionChanged += ResolutionChangedHandler;
            GameGraphicsSettings.QualityChanged += QualityChangedHandler;
        }

        private void OnDestroy()
        {
            m_masterVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMasterField);
            m_musicsVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMusicsField);
            m_sfxVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetSfxField);

            GameGraphicsSettings.ResolutionChanged -= ResolutionChangedHandler;
            GameGraphicsSettings.QualityChanged -= QualityChangedHandler;
        }

        private void ResolutionChangedHandler(int idx)
        {
            var resolution = GameGraphicsSettings.instance.screenResolutions[idx];
            m_resolutionText.text = $"{resolution.x} x {resolution.y}";
        }

        private void QualityChangedHandler(int idx)
        {
            m_qualityEvent.StringReference = GameGraphicsSettings.instance.qualitiesNames[idx];
        }

        public void ActiveMenu()
        {
            m_canvasGroup.DOFade(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu()
        {
            m_canvasGroup.DOFade(false, UIUtility.TransitionTime, () => OnMenuDisable?.Invoke());
        }
    }
}