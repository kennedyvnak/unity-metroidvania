namespace Metroidvania.Animations
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Sprite Sheet Animation Bundle", menuName = "Scriptables/Sheet Animation Bundle")]
    public class SpriteSheetAnimationsBundle : ScriptableObject
    {
        public SpriteSheetAnimation[] animations;

        public SpriteSheetAnimation GetAnimation(string animationName)
        {
            return GetAnimation(Animator.StringToHash(animationName));
        }

        public SpriteSheetAnimation GetAnimation(int animationHash)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                SpriteSheetAnimation animation = animations[i];
                if (animation.hash == animationHash)
                    return animation;
            }

            return null;
        }
    }
}