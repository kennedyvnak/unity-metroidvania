using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerAnimator : PlayerComponent
    {
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
        public readonly SpriteRenderer graphic;

        public readonly Animator machine;

        public PlayerAnimator(PlayerController target) : base(target)
        {
            machine = target.gfxGameObject.GetComponent<Animator>();
            graphic = target.gfxGameObject.GetComponent<SpriteRenderer>();
        }

        private string currentAnimation { get; set; }

        public void SwitchAnimation(string animationKey)
        {
            if (currentAnimation == animationKey)
                return;

            machine.Play(animationKey);
            currentAnimation = animationKey;
        }

        public void FlipCheck()
        {
            if (ShouldFlip())
                Flip();
        }

        public void Flip()
        {
            target.facingDirection *= -1;
            target.transform.localScale = new Vector2(target.facingDirection, 1);
        }

        public bool ShouldFlip()
        {
            var velocity = target.rb.velocity.x;
            return (velocity > 0 && target.facingDirection == -1) || (velocity < 0 && target.facingDirection == 1);
        }
    }
}