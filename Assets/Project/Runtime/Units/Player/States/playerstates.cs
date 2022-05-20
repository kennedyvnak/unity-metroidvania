using System.Collections;
using Metroidvania.Combat;
using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania.Player.States
{
    /// <summary>Base classes for all player states</summary>
    public abstract class PlayerStateBase
    {
        /// <summary>The entity machine</summary>
        public readonly PlayerStateMachine machine;

        protected PlayerStateBase(PlayerStateMachine machine) => this.machine = machine;

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

    /// <summary>Class for use attack states</summary>
    public class PlayerAttackState : PlayerStateBase
    {
        /// <summary>Colliders hit on last trigger. Used for allocate hits array only once</summary>
        private readonly Collider2D[] _hits = new Collider2D[8];

        /// <summary>The attack data that stores the collision rect, move offset, damage...</summary>
        public readonly PlayerDataChannel.Attack attackData;

        /// <summary>The animation key of this attack</summary>
        public readonly string animKey;

        /// <summary>On reach the end and this prop don't is null, switch to this state</summary>
        public PlayerAttackState nextAttackState;

        /// <summary>Time elapsed after entering this state. Used to deal with the attack trigger</summary>
        private float _elapsedTime;

        /// <summary>Is the state triggered? Used to trigger the attack only once</summary>
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

            // Set player's horizontal velocity to 0 and switch the animation
            machine.player.SetHorizontalVelocity(0);
            machine.player.animator.SwitchAnimation(animKey);
        }

        public override void LogicUpdate()
        {
            // Increases deltaTime in elapsedTime to simulate seconds
            _elapsedTime += Time.deltaTime;

            // Check if the player is not floating
            if (machine.EnterFallState() != null)
                return;

            // Trigger the attack if it hasn't triggered and the elapsedTime is greater than or equal to the triggerTime
            if (!_triggered && _elapsedTime >= attackData.triggerTime)
            {
                _triggered = true;
                TriggerAttack();
            }

            // Exit the state if the elapsedTime is greater than or equal to the attack duration 
            if (!(_elapsedTime >= attackData.duration)) return;

            // Switch to the next attack if it is not null and the player is holding attack button 
            if (machine.player.input.virtualAttacking && nextAttackState != null)
            {
                nextAttackState.SetActive();
                return;
            }

            // Switch to the preferred idle state in machine, like fall, crouching or the default idle. 
            machine.EnterIdleState();
        }

        /// <summary>Triggers the attack, note that method don't set or follow the property <see cref="_triggered"/></summary>
        private void TriggerAttack()
        {
            // Moves the player's position by the offset defined in the attack data
            machine.player.rb.MovePosition(machine.player.rb.position +
                                           new Vector2(attackData.horizontalMoveOffset * machine.player.facingDirection,
                                               0));

            // Get the colliders in the attack rect without allocate a new array in the memory
            var hitCount = Physics2D.OverlapBoxNonAlloc(
                machine.player.rb.position + attackData.triggerCollider.center * machine.player.transform.localScale,
                attackData.triggerCollider.size, 0, _hits, machine.player.data.hittableLayer);

            // Do nothing if don't hit any object
            if (hitCount <= 0) return;

            var hitData = new PlayerHitData(attackData.damage, attackData.force, machine.player);
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
        /// <summary>Time elapsed after entering this state. Used to deal with the transition animation</summary>
        protected float elapsedTime;

        /// <summary>True when is quitting in transition anim. Used for not repeat the <see cref="PerformCrouchExitAnim"/> process</summary>
        protected bool quittingAnim;

        /// <summary>
        /// Used for define if should make transition when enter in the state.
        /// For example, idle to crouch idle should animate, but walking crouch to idle crouch should not
        /// </summary>
        public bool shouldMakeTransition;

        /// <summary>Must be true if entry transition animation is already over</summary>
        protected bool swappedAnim;

        protected PlayerCrouchStateBase(PlayerStateMachine machine) : base(machine)
        {
        }

        /// <summary>A coroutine to perform the exit transition animation and switch the state.</summary>
        /// <param name="nextState">The state that will be active when the animation end</param>
        protected IEnumerator PerformCrouchExitAnim(PlayerStateBase nextState)
        {
            quittingAnim = true;
            machine.player.animator.SwitchAnimation(PlayerAnimator.CrouchTransitionAnimKey);
            yield return CoroutinesUtility.GetYieldSeconds(machine.player.data.crouchTransitionTime);
            nextState.SetActive();
        }
    }

    /// <summary>Player state when he is crouched and idle</summary>
    public class PlayerCrouchState : PlayerCrouchStateBase
    {
        public PlayerCrouchState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Setup properties
            elapsedTime = 0;
            quittingAnim = false;

            // Set player's horizontal velocity to 0
            machine.player.SetHorizontalVelocity(0);

            // shouldMakeTransition property validation 
            swappedAnim = !shouldMakeTransition;
            machine.player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchAnimKey);
        }

        public override void LogicUpdate()
        {
            // Do nothing if is in the quitting anim
            if (quittingAnim) return;

            // Increases deltaTime in elapsedTime to simulate seconds
            elapsedTime += Time.deltaTime;

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!swappedAnim && elapsedTime > machine.player.data.crouchTransitionTime)
            {
                swappedAnim = true;
                machine.player.animator.SwitchAnimation(PlayerAnimator.CrouchAnimKey);
            }

            // Try enter in other states
            if (machine.EnterJump() != null ||
                machine.EnterFallState() != null ||
                machine.EnterSlideState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Switches to crouch walk state if the input horizontal move isn't equal 0
            if (machine.player.input.horizontalMove != 0)
            {
                machine.crouchWalkState.shouldMakeTransition = false;
                machine.crouchWalkState.SetActive();
                return;
            }

            // Switches to idle state if the player don't is pressing the crouch button 
            if (machine.player.input.virtualCrouching == false)
                machine.player.StartCoroutine(PerformCrouchExitAnim(machine.idleState));
        }
    }

    /// <summary>Player state when he is crouched and walking</summary>
    public class PlayerCrouchWalkState : PlayerCrouchStateBase
    {
        public PlayerCrouchWalkState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Resets the properties
            elapsedTime = 0;
            quittingAnim = false;

            // shouldMakeTransition property validation 
            swappedAnim = !shouldMakeTransition;
            machine.player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchWalkAnimKey);
        }

        public override void LogicUpdate()
        {
            // Only move if is in the quitting anim
            if (quittingAnim)
            {
                machine.player.MoveHorizontalAxesUsingInput(machine.player.data.crouchSpeed);
                return;
            }

            // Increases deltaTime in elapsedTime to simulate seconds
            elapsedTime += Time.deltaTime;

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!swappedAnim && elapsedTime > machine.player.data.crouchTransitionTime)
            {
                swappedAnim = true;
                machine.player.animator.SwitchAnimation(PlayerAnimator.CrouchWalkAnimKey);
            }

            // Try enter in jump or fall state
            if (machine.EnterJump() != null || machine.EnterFallState() != null)
                return;

            // Switch to idle state
            if (machine.player.input.horizontalMove == 0)
            {
                machine.crouchState.shouldMakeTransition = false;
                machine.crouchState.SetActive();
                return;
            }

            // Switch to run state
            if (machine.player.input.virtualCrouching == false)
            {
                machine.player.StartCoroutine(PerformCrouchExitAnim(machine.runState));
                return;
            }

            // Try enter in slide or attack state
            if (machine.EnterSlideState() != null || machine.EnterAttackState() != null)
                return;

            // Perform movement 
            machine.player.MoveHorizontalAxesUsingInput(machine.player.data.crouchSpeed);
        }
    }

    // TODO: Implement this state
    /// <summary>Player state for handle he death</summary>
    public class PlayerDeathState : PlayerStateBase
    {
        public PlayerDeathState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.DeathAnimKey);
        }
    }

    /// <summary>Player state when he is not grounded</summary>
    public class PlayerFallState : PlayerStateBase
    {
        public PlayerFallState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.FallAnimKey);
        }

        public override void LogicUpdate()
        {
            if (machine.player.collisions.isGrounded)
            {
                machine.EnterIdleState();
                return;
            }

            machine.player.MoveHorizontalAxesUsingInput(machine.player.data.moveSpeed);
        }
    }

    /// <summary>Player state after take a hit</summary>
    public class PlayerHurtState : PlayerStateBase
    {
        /// <summary>Time elapsed after entering this state</summary>
        private float _elapsedTime;

        /// <summary>The force that will be applied to the rigidbody upon entering this state</summary>
        public Vector2 knockbackForce { get; set; }

        public PlayerHurtState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Reset elapsed time
            _elapsedTime = 0;

            // Reset the player velocity and add the knockback force to the player rigidbody force
            machine.player.rb.velocity = Vector2.zero;
            machine.player.rb.AddForce(knockbackForce, ForceMode2D.Impulse);

            machine.player.animator.SwitchAnimation(PlayerAnimator.HurtAnimKey);
        }

        public override void LogicUpdate()
        {
            // Increases deltaTime in elapsedTime to simulate seconds
            _elapsedTime += Time.deltaTime;

            // Enter in a idle state when the state ends
            if (_elapsedTime >= machine.player.data.hurtTime)
                machine.EnterIdleState();
        }
    }

    /// <summary>Player state when he is standing and idle</summary>
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Set the player velocity to 0
            machine.player.SetHorizontalVelocity(0);
            machine.player.animator.SwitchAnimation(PlayerAnimator.IdleAnimKey);
        }

        public override void LogicUpdate()
        {
            // Try switch states
            if (machine.EnterFallState() != null ||
                machine.EnterCrouchState() != null ||
                machine.EnterJump() != null ||
                machine.EnterRollState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Enter in run state if move input isn't equals 0
            if (machine.player.input.horizontalMove != 0)
                machine.runState.SetActive();
        }
    }

    /// <summary>Player state when he is jumping</summary>
    public class PlayerJumpState : PlayerStateBase
    {
        private static InputReader reader => InputReader.instance;

        /// <summary>Time elapsed after entering this state</summary>
        private float _elapsedTime;

        /// <summary>True when the jump button is up </summary>
        private bool _jumpCanceled;

        public PlayerJumpState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Resets the properties
            _elapsedTime = 0;
            _jumpCanceled = false;

            // Assign a method to the jumpCancelEvent in the reader to reference when the jump is canceled, to make a more smoother jump
            reader.JumpCanceledEvent += HandleJumpCancel;

            machine.player.animator.SwitchAnimation(PlayerAnimator.JumpAnimKey);
        }

        public override void Exit()
        {
            // Remove the handle method so as not to cause an unexpected error
            reader.JumpCanceledEvent -= HandleJumpCancel;
        }

        public override void LogicUpdate()
        {
            // Increases deltaTime in elapsedTime to simulate seconds
            _elapsedTime += Time.deltaTime;

            // Exits the state if the elapsed time reaches the duration
            if (_elapsedTime > machine.player.data.jumpDuration)
            {
                machine.EnterIdleState();
                return;
            }

            // Exits the state if the jump was canceled
            if (_jumpCanceled)
            {
                // Set player velocity.y to 0.15 to make a smooth jump stop
                machine.player.rb.velocity = new Vector2(machine.player.rb.velocity.x, .15f);
                machine.EnterIdleState();
                return;
            }

            // Calculates the horizontal speed
            var horizontalSpeed = machine.player.input.horizontalMove * machine.player.data.moveSpeed;

            // Calculates the vertical speed using the data.JumpCurve curve to make a smooth jump
            var verticalSpeed =
                machine.player.data.jumpCurve.Evaluate(_elapsedTime / machine.player.data.jumpDuration) *
                machine.player.data.jumpSpeed;

            machine.player.rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);

            machine.player.animator.FlipCheck();
        }

        private void HandleJumpCancel()
        {
            _jumpCanceled = true;
        }
    }

    /// <summary>Player state when he is standing and walking</summary>
    public class PlayerRunState : PlayerStateBase
    {
        public PlayerRunState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.RunAnimKey);
        }

        public override void LogicUpdate()
        {
            // Try enter in other state
            if (machine.EnterFallState() != null ||
                machine.EnterCrouchState() != null ||
                machine.EnterJump() != null ||
                machine.EnterRollState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Enter in idle state if the move input is 0
            if (machine.player.input.horizontalMove == 0)
            {
                machine.idleState.SetActive();
                return;
            }

            // Movement the player
            machine.player.MoveHorizontalAxesUsingInput(machine.player.data.moveSpeed);
        }
    }

    /// <summary>Player state when he is standing and starts a dash</summary>
    public class PlayerRollState : PlayerStateBase
    {
        /// <summary>Time elapsed after entering this state</summary>
        private float _elapsedTime;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown { get; private set; }

        public PlayerRollState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Resets the properties
            _elapsedTime = 0;
            isInCooldown = true;

            machine.player.invincibility.AddInvincibility(machine.player.data.rollDuration, false);

            machine.player.animator.SwitchAnimation(PlayerAnimator.RollAnimKey);
        }

        public override void LogicUpdate()
        {
            // Increases deltaTime in elapsedTime to simulate seconds
            _elapsedTime += Time.deltaTime;

            // Exits the state if the elapsed time reaches the duration
            if (_elapsedTime >= machine.player.data.rollDuration)
            {
                machine.idleState.SetActive();
                return;
            }

            // Calculates the vertical speed using the data.rollCurve curve to make a smooth roll
            var speed = machine.player.data.rollSpeed * machine.player.facingDirection;
            machine.player.SetHorizontalVelocity(
                machine.player.data.rollCurve.Evaluate(_elapsedTime / machine.player.data.rollDuration) * speed);
        }

        public override void Exit()
        {
            machine.player.StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return CoroutinesUtility.GetYieldSeconds(machine.player.data.rollCooldown);
            isInCooldown = false;
        }
    }

    /// <summary>Player state when he is crouching and starts a dash</summary>
    public class PlayerSlideState : PlayerStateBase
    {
        /// <summary>Time elapsed after entering this state</summary>
        private float _elapsedTime;

        /// <summary>True when the quitting animation is running.</summary>
        private bool _quittingAnim;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown { get; private set; }

        public PlayerSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            // Resets the properties
            isInCooldown = true;
            _elapsedTime = 0;
            _quittingAnim = false;

            machine.player.animator.SwitchAnimation(PlayerAnimator.SlideAnimKey);
        }

        public override void LogicUpdate()
        {
            // Increases deltaTime in elapsedTime to simulate seconds
            _elapsedTime += Time.deltaTime;

            // Starts the exit transition if it's near the end of the slide
            if (!_quittingAnim &&
                _elapsedTime >= machine.player.data.slideDuration - machine.player.data.slideTransitionTime)
            {
                machine.player.animator.SwitchAnimation(PlayerAnimator.SlideEndAnimKey);
                _quittingAnim = true;
            }

            // Exits the state if the elapsed time reaches the duration
            if (_elapsedTime >= machine.player.data.slideDuration)
            {
                machine.crouchState.shouldMakeTransition = false;
                machine.crouchState.SetActive();
                return;
            }

            // Try enter in fall state 
            if (machine.EnterFallState() != null)
                return;

            // Calculates and apply the speed
            var speed = machine.player.data.slideCurve.Evaluate(_elapsedTime / machine.player.data.slideDuration) *
                        machine.player.data.slideSpeed * machine.player.facingDirection;
            machine.player.SetHorizontalVelocity(speed);
        }

        public override void Exit()
        {
            machine.player.StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return CoroutinesUtility.GetYieldSeconds(machine.player.data.slideCooldown);
            isInCooldown = false;
        }
    }

    // TODO: Implement this state
    /// <summary>Player state when he is on wall-hand state and presses to climb it</summary>
    public class PlayerWallClimbState : PlayerStateBase
    {
        public PlayerWallClimbState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.WallClimbAnimKey);
        }
    }

    // TODO: Implement this state
    /// <summary>Player state when he is on the edge of a wall/floor</summary>
    public class PlayerWallHandState : PlayerStateBase
    {
        public PlayerWallHandState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.WallHandAnimKey);
        }
    }

    //TODO: Implement this state
    /// <summary>Player state when he is falling and walking towards the wall</summary>
    public class PlayerWallSlideState : PlayerStateBase
    {
        public PlayerWallSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter()
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.WallSlideAnimKey);
        }

        public override void LogicUpdate()
        {
            if (machine.player.collisions.isGrounded)
            {
                machine.idleState.SetActive();
                return;
            }

            if (machine.player.input.horizontalMove == 0)
            {
                machine.EnterIdleState();
                return;
            }

            machine.player.rb.velocity = new Vector2(0, -machine.player.data.wallSlideSpeed);
        }
    }
}