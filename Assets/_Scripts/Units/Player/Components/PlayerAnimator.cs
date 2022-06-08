using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle animations</summary>
    public class PlayerAnimator : PlayerComponent
    {
        // Animations names constants
        public static readonly int IdleAnimKey = Animator.StringToHash("Idle");
        public static readonly int RunAnimKey = Animator.StringToHash("Run");

        public static readonly int JumpAnimKey = Animator.StringToHash("Jump");
        public static readonly int FallAnimKey = Animator.StringToHash("Fall");

        public static readonly int DashAnimKey = Animator.StringToHash("Dash");
        public static readonly int RollAnimKey = Animator.StringToHash("Roll");

        public static readonly int SlideStartAnimKey = Animator.StringToHash("Slide-Start");
        public static readonly int SlideAnimKey = Animator.StringToHash("Slide");
        public static readonly int SlideEndAnimKey = Animator.StringToHash("Slide-End");

        public static readonly int WallHandAnimKey = Animator.StringToHash("Wall-Hang");
        public static readonly int WallSlideAnimKey = Animator.StringToHash("Wall-Slide");
        public static readonly int WallClimbAnimKey = Animator.StringToHash("Wall-Climb");

        public static readonly int CrouchAnimKey = Animator.StringToHash("Crouch");
        public static readonly int CrouchWalkAnimKey = Animator.StringToHash("Crouch-Walk");
        public static readonly int CrouchTransitionAnimKey = Animator.StringToHash("Crouch-Transition");
        public static readonly int CrouchAttackAnimKey = Animator.StringToHash("Crouch-Attack");

        public static readonly int AttackOneAnimKey = Animator.StringToHash("Attack-One");
        public static readonly int AttackTwoAnimKey = Animator.StringToHash("Attack-Two");

        public static readonly int HurtAnimKey = Animator.StringToHash("Hurt");
        public static readonly int DeathAnimKey = Animator.StringToHash("Death");

        // Animation behaviours
        public readonly SpriteRenderer graphic;
        public readonly Animator machine;

        public PlayerAnimator(PlayerController player) : base(player)
        {
            machine = player.gfxGameObject.GetComponent<Animator>();
            graphic = player.gfxGameObject.GetComponent<SpriteRenderer>();
        }

        /// <summary>The animation which is running</summary>
        private int currentAnimation { get; set; }

        /// <summary>Switch the running animation in the animator</summary>
        /// <param name="animationKey">The new animation key</param>
        /// <param name="force">If true, don't do animation check</param>
        public void SwitchAnimation(int animationKey, bool force = false)
        {
            if (!force && currentAnimation == animationKey)
                return;

            machine.Play(animationKey, -1, 0);
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
            player.facingDirection *= -1;
            player.transform.localScale = new Vector2(player.facingDirection, 1);
        }

        /// <returns>True if the player needs flip</returns>
        public bool ShouldFlip()
        {
            var hMove = player.input.horizontalMove;
            return (hMove > 0 && player.facingDirection == -1) || (hMove < 0 && player.facingDirection == 1);
        }
    }
}