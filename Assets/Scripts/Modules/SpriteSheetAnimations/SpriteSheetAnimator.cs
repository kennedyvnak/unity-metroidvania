using UnityEngine;

namespace Metroidvania.Animations {
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSheetAnimator : MonoBehaviour {
        private SpriteRenderer _renderer;

        [SerializeField] private SpriteSheetAnimationsBundle m_AnimationsBundle;
        public SpriteSheetAnimationsBundle animationsBundle { get => m_AnimationsBundle; set => m_AnimationsBundle = value; }

        private SpriteSheetAnimation currentSheet { get; set; }

        private float elapsedTime { get; set; }

        private void Awake() {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            if (currentSheet == null)
                return;

            elapsedTime += Time.deltaTime;

            int spriteIndex = currentSheet.loop
                ? Mathf.FloorToInt(elapsedTime * currentSheet.frameRate % currentSheet.sheet.Length)
                : Mathf.Clamp(Mathf.FloorToInt(elapsedTime * currentSheet.frameRate), 0, currentSheet.sheet.Length - 1);

            _renderer.sprite = currentSheet.sheet[spriteIndex];
        }

        public void SetSheet(SpriteSheetAnimation animation) {
            currentSheet = animation;
            elapsedTime = 0;
        }

        public void SetSheet(int animHash) => SetSheet(m_AnimationsBundle.GetAnimation(animHash));
    }
}