using System.Collections;
using UnityEngine;

namespace Metroidvania.Animations {
    [RequireComponent(typeof(SpriteSheetAnimator))]
    public class SpriteSheetAnimationsBundleLoopTest : MonoBehaviour {
        private SpriteSheetAnimator _animator;

        private void Awake() {
            _animator = GetComponent<SpriteSheetAnimator>();
        }

        private IEnumerator Start() {
            while (enabled) {
                foreach (SpriteSheetAnimation animation in _animator.animationsBundle.animations) {
                    _animator.SetSheet(animation);
                    yield return CoroutinesUtility.GetYieldSeconds(animation.duration);
                }
            }
        }
    }
}
