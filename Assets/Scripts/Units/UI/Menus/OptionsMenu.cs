using DG.Tweening;
using Metroidvania.Audio;
using Metroidvania.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Metroidvania.UI.Menus {
    public class OptionsMenu : CanvasMenuBase, IMenuScreen {
        [System.Serializable]
        public class Panel {
            public CanvasGroup group;
            public GameObject firstSelected;
            public VerticalSelectionGroup selectionGroup;

            public Tween FadeGroup(bool enabled) {
                return group.FadeGroup(enabled, UIUtility.TransitionTime, () => {
                    if (enabled) {
                        selectionGroup.UpdateNavigation();
                        UIUtility.eventSystem.SetSelectedGameObject(firstSelected);
                    }
                });
            }
        }

        [SerializeField] private CanvasGroup m_canvasGroup;

        [Header("Options")]
        [SerializeField] private Panel m_buttonsGroup;
        [SerializeField] private Panel m_settingsPanel;
        [SerializeField] private Panel m_audioPanel;
        [SerializeField] private Panel m_graphicsPanel;
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

        private CanvasGroup _activeGroup;

        private void Awake() {
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

        private void OnDestroy() {
            m_masterVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMasterField);
            m_musicsVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetMusicsField);
            m_sfxVolumeSlider.onValueChanged.RemoveListener(GameAudioSettings.instance.SetSfxField);

            m_resolutionChangedChannel.OnEventRaise -= ResolutionChangedHandler;
            m_qualityChangedChannel.OnEventRaise -= QualityChangedHandler;
        }

        public void OpenSettingsScreen() => OpenScreen(m_settingsPanel);
        public void OpenAudioScreen() => OpenScreen(m_audioPanel);
        public void OpenGraphicsScreen() => OpenScreen(m_graphicsPanel);

        public void OpenScreen(Panel panel) {
            m_returnButton.interactable = false;
            m_buttonsGroup.FadeGroup(false).onComplete += () => {
                panel.FadeGroup(true).onComplete += () => {
                    _activeGroup = panel.group;
                    m_returnButton.interactable = true;
                };
            };
        }

        private void ResolutionChangedHandler(int idx) {
            Vector2Int resolution = GameGraphicsSettings.instance.screenResolutions[idx];
            m_resolutionText.text = $"{resolution.x} x {resolution.y}";
        }

        private void QualityChangedHandler(int idx) {
            m_qualityEvent.StringReference = GameGraphicsSettings.instance.qualitiesNames[idx];
        }

        public void ActiveMenu() {
            menuEnabled = true;
            m_canvasGroup.FadeGroup(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu() {
            if (_activeGroup) {
                _activeGroup.FadeGroup(false, UIUtility.TransitionTime, () => {
                    m_returnButton.interactable = false;
                    m_buttonsGroup.FadeGroup(true).onComplete += () => {
                        _activeGroup = null;
                        m_returnButton.interactable = true;
                    };
                });
            } else {
                menuEnabled = false;
                m_canvasGroup.FadeGroup(false, UIUtility.TransitionTime, () => OnMenuDisable?.Invoke());
            }
        }

        public Tweener FadeMenu(bool enabled) {
            return m_canvasGroup.FadeGroup(enabled, UIUtility.TransitionTime, SetFirstSelected);
        }
    }
}