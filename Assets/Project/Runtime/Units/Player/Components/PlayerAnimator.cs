using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle animations</summary>
    public class PlayerAnimator : PlayerComponent
    {
        // Animations names constants
        public const string IdleAnimKey = "Idle";
        public const string RunAnimKey = "Run";

        public const string JumpAnimKey = "Jump";
        public const string FallAnimKey = "Fall";

        public const string DashAnimKey = "Dash";
        public const string RollAnimKey = "Roll";

        public const string SlideStartAnimKey = "Slide-Start";
        public const string SlideAnimKey = "Slide";
        public const string SlideEndAnimKey = "Slide-End";

        public const string WallHandAnimKey = "Wall-Hang";
        public const string WallSlideAnimKey = "Wall-Slide";
        public const string WallClimbAnimKey = "Wall-Climb";

        public const string CrouchAnimKey = "Crouch";
        public const string CrouchWalkAnimKey = "Crouch-Walk";
        public const string CrouchTransitionAnimKey = "Crouch-Transition";
        public const string CrouchAttackAnimKey = "Crouch-Attack";

        public const string AttackOneAnimKey = "Attack-One";
        public const string AttackTwoAnimKey = "Attack-Two";

        public const string HurtAnimKey = "Hurt";
        public const string DeathAnimKey = "Death";

        // Animation behaviours
        public readonly SpriteRenderer graphic;
        public readonly Animator machine;

        public PlayerAnimator(PlayerController target) : base(target)
        {
            machine = target.gfxGameObject.GetComponent<Animator>();
            graphic = target.gfxGameObject.GetComponent<SpriteRenderer>();
        }

        /// <summary>The animation which is running</summary>
        private string currentAnimation { get; set; }

        /// <summary>Switch the running animation in the animator</summary>
        /// <param name="animationKey">The new animation key</param>
        /// <param name="force">If true, don't do animation check</param>
        public void SwitchAnimation(string animationKey, bool force = false)
        {
            if (!force && currentAnimation == animationKey)
                return;

            machine.Play(animationKey);
            currentAnimation = animationKey;
        }

        /// <summary>Flips the player if needed </summary>
        public void FlipCheck()
        {
            if (ShouldFlip())
                Flip();
        }

        /// <summary>Horizontally flips the player</summary>
        public void Flip()
        {
            target.facingDirection *= -1;
            target.transform.localScale = new Vector2(target.facingDirection, 1);
        }

        /// <returns>True if the player needs flip</returns>
        public bool ShouldFlip()
        {
            var velocity = target.rb.velocity.x;
            return (velocity > 0 && target.facingDirection == -1) || (velocity < 0 && target.facingDirection == 1);
        }
    }
}