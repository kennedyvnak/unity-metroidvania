using DG.Tweening;
using Metroidvania.Audio;
using Metroidvania.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Metroidvania.UI.Menus
{
    public class OptionsMenu : CanvasMenuBase, IMenuScreen
    {
        [System.Serializable]
        public struct Panel
        {
            public GameObject group;
            public LocalizedString panelName;
        }

        [SerializeField] private CanvasGroup m_canvasGroup;

        [Header("Options")]
        [SerializeField] private LocalizeStringEvent m_headerLabel;
        [SerializeField] private Panel[] m_panels;
        [SerializeField] private Button m_returnButton;

        [Header("Audio Settings")]
        [SerializeField] private Slider m_masterVolumeSlider;
        [SerializeField] private Slider m_musicsVolumeSlider;
        [SerializeField] private Slider m_sfxVolumeSlider;

        [Header("Quality Settings")]
        [SerializeField] private TextMeshProUGUI m_resolutionText;
        [SerializeField] private LocalizeStringEvent m_qualityEvent;
        [SerializeField] private Toggle m_fullScreenToggle;

        [Header("Events")]
        [SerializeField] private IntEventChannel m_resolutionChangedChannel;
        [SerializeField] private IntEventChannel m_qualityChangedChannel;
        [SerializeField] private BoolEventChannel m_fullScreenChangedChannel;

        public event System.Action OnMenuDisable;

        private int _activeGroupIndex;

        private void Awake()
        {
            m_masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.masterFieldName, 1));
            m_musicsVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.musicsFieldName, 1));
            m_sfxVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GameAudioSettings.instance.sfxFieldName, 1));

            m_masterVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetMasterField);
            m_musicsVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetMusicsField);
            m_sfxVolumeSlider.onValueChanged.AddListener(GameAudioSettings.instance.SetSfxField);

            m_fullScreenToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(GameGraphicsSettings.instance.fullScreenPrefsKey, GameGraphicsSettings.instance.defaultFullScreenValue ? 1 : 0) == 1);

            ResolutionChangedHandler(PlayerPrefs.GetInt(GameGraphicsSettings.instance.resolutionPrefsKey, GameGraphicsSettings.instance.defaultResolutionIndex));
            QualityChangedHandler(PlayerPrefs.GetInt(GameGraphicsSettings.instance.qualityPrefsKey, GameGraphicsSettings.instance.defaultQualityIndex));

            m_resolutionChangedChannel.OnEventRaise += ResolutionChangedHandler;
            m_qualityChangedChannel.OnEventRaise += QualityChangedHandler;
        }

        private void OnDestroy()
        {
            m_masterVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMasterField);
            m_musicsVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMusicsField);
            m_sfxVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetSfxField);

            m_resolutionChangedChannel.OnEventRaise -= ResolutionChangedHandler;
            m_qualityChangedChannel.OnEventRaise -= QualityChangedHandler;
        }

        public void MoveLeft()
        {
            int panel = _activeGroupIndex - 1;
            if (panel < 0)
            {
                panel = m_panels.Length - 1;
            }
            ActivatePanel(panel);
        }

        public void MoveRight()
        {
            int panel = _activeGroupIndex + 1;
            if (panel >= m_panels.Length)
            {
                panel = 0;
            }
            ActivatePanel(panel);
        }

        private void ActivatePanel(int panel)
        {
            m_panels[_activeGroupIndex].group.SetActive(false);
            _activeGroupIndex = panel;
            m_panels[_activeGroupIndex].group.SetActive(true);

            m_headerLabel.StringReference = m_panels[_activeGroupIndex].panelName;
        }

        private void ResolutionChangedHandler(int idx)
        {
            Vector2Int resolution = GameGraphicsSettings.instance.screenResolutions[idx];
            m_resolutionText.text = $"{resolution.x} x {resolution.y}";
        }

        private void QualityChangedHandler(int idx)
        {
            m_qualityEvent.StringReference = GameGraphicsSettings.instance.qualitiesNames[idx];
        }

        public void ActiveMenu()
        {
            _activeGroupIndex = 0;
            m_panels[_activeGroupIndex].group.SetActive(true);
            menuEnabled = true;
            m_canvasGroup.FadeGroup(true, Helpers.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu()
        {
            menuEnabled = false;
            m_canvasGroup.FadeGroup(false, Helpers.TransitionTime, () =>
            {
                m_panels[_activeGroupIndex].group.SetActive(false);
                OnMenuDisable?.Invoke();
            });
        }

        public Tweener FadeMenu(bool enabled)
        {
            return m_canvasGroup.FadeGroup(enabled, Helpers.TransitionTime, SetFirstSelected);
        }
    }
}
