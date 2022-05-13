using System.Collections;
using Metroidvania.Combat;
using UnityEngine;

namespace Metroidvania.Player.States
{
    public abstract class PlayerStateBase
    {
        public readonly PlayerStateMachine machine;

        protected PlayerStateBase(PlayerStateMachine machine)
        {
            this.machine = machine;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void LogicUpdate()
        {
        }
    }

    public abstract class PlayerAttackStateBase : PlayerStateBase
    {
        protected readonly Collider2D[] hits = new Collider2D[8];
        protected float elapsedTime;
        protected bool triggered;

        protected PlayerAttackStateBase(PlayerStateMachine machine) : base(machine)
        {
        }

        protected abstract PlayerAttackStateBase nextAttackState { get; }
        protected abstract PlayerDataChannel.Attack attackData { get; }

        public override void Enter()
        {
            elapsedTime = 0;
            triggered = false;
            machine.target.SetHorizontalVelocity(0);
        }

        public override void LogicUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (machine.EnterFallState() != null)
                return;

            if (!triggered && elapsedTime >= attackData.triggerTime)
            {
                triggered = true;
                TriggerAttack();
            }

            if (elapsedTime >= attackData.duration)
            {
                if (machine.target.input.virtualAttacking && nextAttackState != null)
                {
                    nextAttackState.SetActive();
                    return;
                }

                machine.EnterIdleState();
            }
        }

        protected virtual void TriggerAttack()
        {
            machine.target.rb.MovePosition(machine.target.rb.position +
                                           new Vector2(attackData.horizontalMoveOffset * machine.target.facingDirection,
                                               0));

            var hitCount = Physics2D.OverlapBoxNonAlloc(
                machine.target.rb.position + attackData.triggerCollider.center * machine.target.transform.localScale,
                attackData.triggerCollider.size, 0, hits, machine.target.data.hittableLayer);
            for (var i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
                if (hit.TryGetComponent<IHittableTarget>(out var hittableTarget))
                    hittableTarget.OnTakeHit(new PlayerHitData(attackData.damage, attackData.force));
            }
        }
    }

    public abstract class PlayerCrouchStateBase : PlayerStateBase
    {
        protected float elapsedTime;
        protected bool quittingAnim;

        public bool shouldMakeTransition;
        protected bool swappedAnim;

        protected PlayerCrouchStateBase(PlayerStateMachine machine) : base(machine)
        {
        }

        protected IEnumerator PerformCrouchExitAnim(PlayerStateBase nextState)
        {
            quittingAnim = true;
            machine.target.animator.SwitchAnimation(PlayerAnimator.CrouchTransitionAnimKey);
            yield return CoroutinesUtility.GetYieldSeconds(machine.target.data.crouchTransitionTime);
            nextState.SetActive();
        }
    }

    public class PlayerAttackOneState : PlayerAttackStateBase
    {
        public PlayerAttackOneState(PlayerStateMachine machine) : base(machine)
        {
        }

        protected override PlayerAttackStateBase nextAttackState => machine.attackTwoState;
        protected override PlayerDataChannel.Attack attackData => machine.target.data.attackOne;

        public override void Enter()
        {
            base.Enter();
            machine.target.animator.SwitchAnimation(PlayerAnimator.AttackOneAnimKey);
        }
    }

    public class PlayerAttackTwoState : PlayerAttackStateBase
    {
        public PlayerAttackTwoState(PlayerStateMachine machine) : base(machine)
        {
        }

        protected override PlayerAttackStateBase nextAttackState => machine.attackOneState;
        protected override PlayerDataChannel.Attack attackData => machine.target.data.attackTwo;

        public override void Enter()
        {
            base.Enter();
            machine.target.animator.SwitchAnimation(PlayerAnimator.AttackTwoAnimKey);
        }
    }

    public class PlayerCrouchAttackState : PlayerAttackStateBase
    {
        public PlayerCrouchAttackState(PlayerStateMachine machine) : base(machine)
        {
        }

        protected override PlayerAttackStateBase nextAttackState => null;
        protected override PlayerDataChannel.Attack attackData => machine.target.data.crouchAttack;

        public override void Enter()
        {
            base.Enter();
            machine.target.animator.SwitchAnimation(PlayerAnimator.CrouchAttackAnimKey);
        }
    }

    public class PlayerCrouchState : PlayerCrouchStateBase
    {
        public PlayerCrouchState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.SetHorizontalVelocity(0);
            elapsedTime = 0;
            quittingAnim = false;

            swappedAnim = !shouldMakeTransition;

            machine.target.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchAnimKey);
        }

        public override void LogicUpdate()
        {
            if (quittingAnim) return;

            elapsedTime += Time.deltaTime;

            if (!swappedAnim && elapsedTime > machine.target.data.crouchTransitionTime)
            {
                swappedAnim = true;
                machine.target.animator.SwitchAnimation(PlayerAnimator.CrouchAnimKey);
            }

            if (machine.EnterJump() != null)
                return;

            if (machine.EnterFallState() != null)
                return;

            if (machine.target.input.horizontalMove != 0)
            {
                machine.crouchWalkState.shouldMakeTransition = false;
                machine.crouchWalkState.SetActive();
                return;
            }

            if (machine.target.input.virtualCrouching == false)
            {
                machine.target.StartCoroutine(PerformCrouchExitAnim(machine.idleState));
                return;
            }

            if (machine.EnterSlideState() != null)
                return;

            machine.EnterAttackState();
        }
    }

    public class PlayerCrouchWalkState : PlayerCrouchStateBase
    {
        public PlayerCrouchWalkState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            elapsedTime = 0;
            quittingAnim = false;

            swappedAnim = !shouldMakeTransition;

            machine.target.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchWalkAnimKey);
        }

        public override void LogicUpdate()
        {
            if (quittingAnim)
            {
                machine.target.MoveHorizontalAxes(machine.target.data.crouchSpeed);
                return;
            }

            elapsedTime += Time.deltaTime;

            if (!swappedAnim && elapsedTime > machine.target.data.crouchTransitionTime)
            {
                swappedAnim = true;
                machine.target.animator.SwitchAnimation(PlayerAnimator.CrouchWalkAnimKey);
            }

            if (machine.EnterJump() != null)
                return;

            if (machine.EnterFallState() != null)
                return;

            if (machine.target.input.horizontalMove == 0)
            {
                machine.crouchState.shouldMakeTransition = false;
                machine.crouchState.SetActive();
                return;
            }

            if (machine.target.input.virtualCrouching == false)
            {
                machine.target.StartCoroutine(PerformCrouchExitAnim(machine.runState));
                return;
            }

            if (machine.EnterSlideState() != null)
                return;

            if (machine.EnterAttackState() != null)
                return;

            machine.target.MoveHorizontalAxes(machine.target.data.crouchSpeed);
        }
    }

    public class PlayerDeathState : PlayerStateBase
    {
        public PlayerDeathState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.DeathAnimKey);
        }
    }

    public class PlayerFallState : PlayerStateBase
    {
        public PlayerFallState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.FallAnimKey);
        }

        public override void LogicUpdate()
        {
            if (machine.target.collisions.isGrounded)
            {
                machine.EnterIdleState();
                return;
            }

            machine.target.MoveHorizontalAxes(machine.target.data.moveSpeed);
        }
    }

    public class PlayerHurtState : PlayerStateBase
    {
        private float _elapsedTime;

        public PlayerHurtState(PlayerStateMachine machine) : base(machine)
        {
        }

        public Vector2 knockbackForce { get; set; }

        public override void Enter()
        {
            _elapsedTime = 0;
            machine.target.rb.AddForce(knockbackForce, ForceMode2D.Impulse);
            machine.target.animator.SwitchAnimation(PlayerAnimator.HurtAnimKey);
        }

        public override void LogicUpdate()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= machine.target.data.hurtTime)
                machine.EnterIdleState();
        }
    }

    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.IdleAnimKey);
            machine.target.SetHorizontalVelocity(0);
        }

        public override void LogicUpdate()
        {
            if (machine.EnterFallState() != null)
                return;

            if (machine.target.input.horizontalMove != 0)
            {
                machine.runState.SetActive();
                return;
            }

            if (machine.EnterCrouchState() != null)
                return;

            if (machine.EnterJump() != null)
                return;

            if (machine.EnterRollState() != null)
                return;

            machine.EnterAttackState();
        }
    }

    public class PlayerJumpState : PlayerStateBase
    {
        private float _elapsedTime;
        private bool _jumpCanceled;

        public PlayerJumpState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.JumpAnimKey);
            _elapsedTime = 0;
            _jumpCanceled = false;
            machine.target.data.inputReader.JumpCanceledEvent += HandleJumpCancel;
        }

        public override void Exit()
        {
            machine.target.data.inputReader.JumpCanceledEvent -= HandleJumpCancel;
        }

        public override void LogicUpdate()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > machine.target.data.jumpDuration)
            {
                machine.EnterIdleState();
                return;
            }

            if (_jumpCanceled)
            {
                machine.target.rb.velocity = new Vector2(machine.target.rb.velocity.x, .15f);
                machine.EnterIdleState();
                return;
            }

            var horizontalSpeed = machine.target.input.horizontalMove * machine.target.data.moveSpeed;
            var verticalSpeed =
                machine.target.data.jumpCurve.Evaluate(_elapsedTime / machine.target.data.jumpDuration) *
                machine.target.data.jumpSpeed;

            machine.target.rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
            machine.target.animator.FlipCheck();
        }

        private void HandleJumpCancel()
        {
            _jumpCanceled = true;
        }
    }

    public class PlayerRunState : PlayerStateBase
    {
        public PlayerRunState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.RunAnimKey);
        }

        public override void LogicUpdate()
        {
            if (machine.EnterFallState() != null)
                return;

            if (machine.target.input.horizontalMove == 0)
            {
                machine.idleState.SetActive();
                return;
            }

            if (machine.EnterCrouchState() != null)
                return;

            if (machine.EnterJump() != null)
                return;

            if (machine.EnterRollState() != null)
                return;

            if (machine.EnterAttackState() != null)
                return;

            machine.target.MoveHorizontalAxes(machine.target.data.moveSpeed);
        }
    }

    public class PlayerRollState : PlayerStateBase
    {
        private float _elapsedTime;

        public PlayerRollState(PlayerStateMachine machine) : base(machine)
        {
        }

        public bool isInCooldown { get; private set; }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.RollAnimKey);
            _elapsedTime = 0;
            isInCooldown = true;
        }

        public override void LogicUpdate()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= machine.target.data.rollDuration)
            {
                machine.idleState.SetActive();
                return;
            }

            var speed = machine.target.data.rollSpeed * machine.target.facingDirection;
            machine.target.SetHorizontalVelocity(
                machine.target.data.rollCurve.Evaluate(_elapsedTime / machine.target.data.rollDuration) * speed);
        }

        public override void Exit()
        {
            machine.target.StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return CoroutinesUtility.GetYieldSeconds(machine.target.data.rollCooldown);
            isInCooldown = false;
        }
    }

    public class PlayerSlideState : PlayerStateBase
    {
        private float _elapsedTime;
        private bool _quittingAnim;

        public PlayerSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

        public bool isInCooldown { get; private set; }

        public override void Enter()
        {
            isInCooldown = true;
            _elapsedTime = 0;
            _quittingAnim = false;
            machine.target.animator.SwitchAnimation(PlayerAnimator.SlideAnimKey);
        }

        public override void LogicUpdate()
        {
            _elapsedTime += Time.deltaTime;

            if (!_quittingAnim &&
                _elapsedTime >= machine.target.data.slideDuration - machine.target.data.slideTransitionTime)
            {
                machine.target.animator.SwitchAnimation(PlayerAnimator.SlideEndAnimKey);
                _quittingAnim = true;
            }

            if (_elapsedTime >= machine.target.data.slideDuration)
            {
                machine.crouchState.shouldMakeTransition = false;
                machine.crouchState.SetActive();
                return;
            }

            if (machine.EnterFallState() != null)
                return;

            var speed = machine.target.data.slideCurve.Evaluate(_elapsedTime / machine.target.data.slideDuration) *
                        machine.target.data.slideSpeed * machine.target.facingDirection;
            machine.target.SetHorizontalVelocity(speed);
        }

        public override void Exit()
        {
            machine.target.StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return CoroutinesUtility.GetYieldSeconds(machine.target.data.slideCooldown);
            isInCooldown = false;
        }
    }

    public class PlayerWallClimbState : PlayerStateBase
    {
        public PlayerWallClimbState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.WallClimbAnimKey);
        }
    }

    public class PlayerWallHandState : PlayerStateBase
    {
        public PlayerWallHandState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.WallHandAnimKey);
        }
    }

    public class PlayerWallSlideState : PlayerStateBase
    {
        public PlayerWallSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.target.animator.SwitchAnimation(PlayerAnimator.WallSlideAnimKey);
        }

        public override void LogicUpdate()
        {
            if (machine.target.collisions.isGrounded)
            {
                machine.idleState.SetActive();
                return;
            }

            if (machine.target.input.horizontalMove == 0)
            {
                machine.EnterIdleState();
                return;
            }

            machine.target.rb.velocity = new Vector2(0, -machine.target.data.wallSlideSpeed);
        }
    }
}