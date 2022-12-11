using UnityEngine;

namespace Metroidvania.Animations {
    public class Test : MonoBehaviour {
        public SpriteSheetAnimator animator;

        [ContextMenu("Set Idle")]
        public void setIdle() => setAnim("Idle");

        [ContextMenu("Set Walk")]
        public void setWalk() => setAnim("Run");

        [ContextMenu("Set Hurt")]
        public void setHurt() => setAnim("Hurt");

        [ContextMenu("Set Die")]
        public void setDie() => setAnim("Die");

        public void setAnim(string key) {
            animator.SetSheet(animator.animationsBundle.GetAnimation(key));
        }
    }
}