using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metroidvania.TMPUtility {
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_HyperlinksOpener : MonoBehaviour, IPointerClickHandler {
        public TMP_Text text { get; private set; }

        private void Awake() {
            text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, CameraUtility.mainCamera);
            if (linkIndex != -1) {
                TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];

                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}
