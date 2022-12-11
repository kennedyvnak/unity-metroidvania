using Metroidvania.UI;
using Metroidvania.UI.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.Credits {
    public class CreditsMenu : CanvasMenuBase, IMenuScreen {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private CreditsAsset m_asset;

        [SerializeField] private TextMeshProUGUI m_creditsText;

        private ScrollRect _scrollRect;

        public event System.Action OnMenuDisable;

        private void Awake() {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void Start() {
            m_creditsText.text = m_asset.GenerateText();
        }

        public void ActiveMenu() {
            menuEnabled = true;
            m_canvasGroup.FadeGroup(true, UIUtility.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu() {
            menuEnabled = false;
            m_canvasGroup.FadeGroup(false, UIUtility.TransitionTime, () => OnMenuDisable?.Invoke());
        }
    }
}