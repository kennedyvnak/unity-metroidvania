using System.Collections;
using Metroidvania.Combat;
using UnityEngine;

namespace Metroidvania.Player.States
{
    /// <summary>Base classes for all player states</summary>
    public abstract class PlayerStateBase
    {
        /// <summary>The target machine</summary>
        public readonly PlayerStateMachine machine;

        protected PlayerStateBase(PlayerStateMachine machine)
        {
            this.machine = machine;
        }

        /// <summary>This method should be called when the player switch to this state</summary> 
        public virtual void Enter()
        {
        }

        /// <summary>This method should be called when the player is in this state and switch to another state</summary> 
        public virtual void Exit()
        {
        }

        /// <summary>This method should be called in player.Update()</summary> 
        public virtual void LogicUpdate()
        {
        }
    }

    /// <summary>Class for player attack states</summary>
    public sealed class PlayerAttackState : PlayerStateBase
    {
        /// <summary>
        /// Colliders hit on last trigger. 
        /// Used for allocate hits array only once
        /// </summary>
        private readonly Collider2D[] _hits = new Collider2D[8];

        /// <summary>
        /// The attack data that stores the collision rect, move offset, damage... 
        /// </summary>
        public readonly PlayerDataChannel.Attack attackData;

        /// <summary>
        /// The animation key of this attack
        /// </summary>
        public readonly string animKey;

        /// <summary>
        /// On reach the end and this prop don't is null, switch to this state
        /// </summary>
        public PlayerAttackState nextAttackState;

        /// <summary>
        /// Time elapsed after entering this state.
        /// Used to deal with the attack trigger
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        /// Is the state triggered?
        /// Used to trigger the attack only once
        /// </summary>
        private bool _triggered;

        public PlayerAttackState(PlayerStateMachine machine, PlayerDataChannel.Attack attackData, string animKey) :
            base(machine)
        {
            this.attackData = attackData;
            this.animKey = animKey;
        }

        public override void Enter()
        {
            // Resets all properties
            _elapsedTime = 0;
            _triggered = false;
            machine.target.SetHorizontalVelocity(0);
            machine.target.animator.SwitchAnimation(animKey);
        }

        public override void LogicUpdate()
        {
            _elapsedTime += Time.deltaTime;

            if (machine.EnterFallState() != null)
                return;

            if (!_triggered && _elapsedTime >= attackData.triggerTime)
            {
                _triggered = true;
                TriggerAttack();
            }

            if (_elapsedTime >= attackData.duration)
            {
                if (machine.target.input.virtualAttacking && nextAttackState != null)
                {
                    nextAttackState.SetActive();
                    return;
                }

                machine.EnterIdleState();
            }
        }

        /// <summary>
        /// Triggers the attack, note that method don't set or follow the property <see cref="_triggered"/>
        /// </summary>
        private void TriggerAttack()
        {
            // Moves the player's position by the offset defined in the attack data
            machine.target.rb.MovePosition(machine.target.rb.position +
                                           new Vector2(attackData.horizontalMoveOffset * machine.target.facingDirection,
                                               0));

            // Get the colliders in the attack rect without allocate a new array in the memory
            var hitCount = Physics2D.OverlapBoxNonAlloc(
                machine.target.rb.position + attackData.triggerCollider.center * machine.target.transform.localScale,
                attackData.triggerCollider.size, 0, _hits, machine.target.data.hittableLayer);

            if (hitCount <= 0) return;

            var hitData = new PlayerHitData(attackData.damage, attackData.force, machine.target);
            for (var i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                // If the hit contains an IHittableTarget component, it will call the OnTakeHit method.  
                if (hit.TryGetComponent<IHittableTarget>(out var hittableTarget))
                    hittableTarget.OnTakeHit(hitData);
            }
        }
    }

    /// <summary>Base class for crouch states, excluding the attack state</summary>
    public abstract class PlayerCrouchStateBase : PlayerStateBase
    {
        /// <summary>
        /// Time elapsed after entering this state
        /// Used to deal with the transition animation.
        /// </summary>
        protected float elapsedTime;

        /// <summary>
        /// True when is quitting in transition anim.
        /// Used for not repeat the <see cref="PerformCrouchExitAnim"/> process
        /// </summary>
        protected bool quittingAnim;

        /// <summary>
        /// Used for define if should make transition when enter in the state.
        /// For example, idle to crouch idle should animate, but walking crouch to idle crouch should not
        /// </summary>
        public bool shouldMakeTransition;

        /// <summary>
        /// Must be true if entry transition animation is already ove
        /// </summary>
        protected bool swappedAnim;

        protected PlayerCrouchStateBase(PlayerStateMachine machine) : base(machine)
        {
        }

        /// <summary>A coroutine to perform the exit transition animation and switch the state.</summary>
        /// <param name="nextState">The state that will be active when the animation end</param>
        protected IEnumerator PerformCrouchExitAnim(PlayerStateBase nextState)
        {
            quittingAnim = true;
            machine.target.animator.SwitchAnimation(PlayerAnimator.CrouchTransitionAnimKey);
            yield return CoroutinesUtility.GetYieldSeconds(machine.target.data.crouchTransitionTime);
            nextState.SetActive();
        }
    }

    /// <summary>
    /// Crouch idle state
    /// </summary>
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

    // TODO: Implement this state
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
        /// <summary>
        /// Time elapsed after entering this state
        /// </summary>
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
        /// <summary>
        /// Time elapsed after entering this state
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        /// True when the jump button is up 
        /// </summary>
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
        /// <summary>
        /// Time elapsed after entering this state
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        /// Boolean to controls the slide cooldown
        /// </summary>
        public bool isInCooldown { get; private set; }

        public PlayerRollState(PlayerStateMachine machine) : base(machine)
        {
        }

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
        /// <summary>
        /// Time elapsed after entering this state
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        /// True when the quitting animation is running.
        /// </summary>
        private bool _quittingAnim;

        /// <summary>
        /// Boolean to controls the slide cooldown
        /// </summary>
        public bool isInCooldown { get; private set; }

        public PlayerSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

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

    // TODO: Implement this
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

    // TODO: Implement this state
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

    //TODO: Implement this state
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