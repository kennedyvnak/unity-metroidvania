using System.Collections;
using Metroidvania.Combat;
using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania.Player.States
{
    //TODO: Implement wall states
    /// <summary>Base classes for all player states</summary>
    public abstract class PlayerStateBase
    {
        private readonly PlayerStateMetadatas _metadatas;
        public PlayerStateMetadatas metadatas => _metadatas;

        /// <summary>The player machine</summary>
        public readonly PlayerStateMachine machine;

        protected PlayerStateBase(PlayerStateMachine machine)
        {
            this.machine = machine;
            _metadatas = new PlayerStateMetadatas(this);
        }

        /// <summary>This method should be called when the player switch to this state</summary> 
        public virtual void Enter(PlayerStateBase previousState)
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

    /// <summary>This state is the first state of player, used for always make the Enter(previousState) not null</summary>
    public class PlayerValidationState : PlayerStateBase
    {
        public PlayerValidationState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            machine.idleState.SetActive();
        }
    }

    /// <summary>Player state when he is standing and idle</summary>
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Set the player velocity to 0
            machine.player.collisions.SetCollisionsData(machine.player.data.standColliderData);
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

    /// <summary>Player state when he is standing and walking</summary>
    public class PlayerRunState : PlayerStateBase
    {
        public PlayerRunState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            machine.player.collisions.SetCollisionsData(machine.player.data.standColliderData);
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

    /// <summary>Player state when he is jumping</summary>
    public class PlayerJumpState : PlayerStateBase
    {
        private static InputReader reader => InputReader.instance;

        private PlayerDurationModule _durationModule;

        /// <summary>True when the jump button is up</summary>
        private bool _jumpCanceled;

        /// <summary>True when the player collides with the top of the collider</summary>
        private bool _collidedTop;

        public PlayerJumpState(PlayerStateMachine machine) : base(machine)
        {
            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets the properties
            _durationModule.Enter();
            _jumpCanceled = false;
            _collidedTop = false;

            // Assign a method to the jumpCancelEvent in the reader to reference when the jump is canceled, to make a more smoother jump
            reader.JumpCanceledEvent += HandleJumpCancel;

            machine.player.collisions.SetCollisionsData(machine.player.data.standColliderData);
            machine.player.animator.SwitchAnimation(PlayerAnimator.JumpAnimKey);
            machine.player.CollisionEntered += CollisionEntered;
        }

        public override void Exit()
        {
            // Remove the handle method so as not to cause an unexpected error
            reader.JumpCanceledEvent -= HandleJumpCancel;
            machine.player.CollisionEntered -= CollisionEntered;
        }

        public override void LogicUpdate()
        {
            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(machine.player.data.jumpDuration) || _collidedTop)
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
            var verticalSpeed = machine.player.data.jumpSpeed *
                machine.player.data.jumpCurve.Evaluate(_durationModule.GetElapsedTime() / machine.player.data.jumpDuration);

            machine.player.rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);

            machine.player.animator.FlipCheck();
        }

        private void HandleJumpCancel()
        {
            _jumpCanceled = true;
        }

        private void CollisionEntered(Collision2D collision)
        {
            // Check if the contact is on top
            // x == 1 = left   || x == -1 = right
            // y == 1 = bottom || y == -1 = top   
            if (collision.GetContact(0).normal.y == -1)
                _collidedTop = true;
        }
    }

    /// <summary>Player state when he is not grounded</summary>
    public class PlayerFallState : PlayerStateBase
    {
        public PlayerFallState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
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

    /// <summary>Player state when he is standing and starts a dash</summary>
    public class PlayerRollState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown { get; private set; }

        public PlayerRollState(PlayerStateMachine machine) : base(machine)
        {
            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets the properties
            _durationModule.Enter();
            isInCooldown = true;

            machine.player.collisions.SetCollisionsData(machine.player.data.standColliderData);
            machine.player.invincibility.AddInvincibility(machine.player.data.rollDuration, false);

            machine.player.animator.SwitchAnimation(PlayerAnimator.RollAnimKey);
        }

        public override void LogicUpdate()
        {
            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(machine.player.data.rollDuration))
            {
                machine.idleState.SetActive();
                return;
            }

            // Calculates the vertical speed using the data.rollCurve curve to make a smooth roll
            var speed = machine.player.data.rollSpeed * machine.player.facingDirection;
            machine.player.SetHorizontalVelocity(
                machine.player.data.rollCurve.Evaluate(_durationModule.GetElapsedTime() / machine.player.data.rollDuration) * speed);
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

    /// <summary>Player state when he is crouched and idle</summary>
    public class PlayerCrouchState : PlayerStateBase
    {
        private PlayerCrouchMetadata _crouchMetadata;

        private PlayerDurationModule _durationModule;

        /// <summary>True when is quitting in transition anim. Used for not repeat the <see cref="_crouchMetadata.ExitCrouchState"/> process</summary>
        private bool _quittingAnim;

        /// <summary>Must be true if entry transition animation is already over</summary>
        private bool _swappedAnim;

        public PlayerCrouchState(PlayerStateMachine machine) : base(machine)
        {
            _crouchMetadata = new PlayerCrouchMetadata(this);

            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            var shouldMakeTransition = !PlayerStatesUtility.IsCrouchState(previousState);

            _durationModule.Enter();
            _quittingAnim = false;

            // Set player's horizontal velocity to 0
            machine.player.SetHorizontalVelocity(0);
            machine.player.collisions.SetCollisionsData(machine.player.data.crouchColliderData);

            // shouldMakeTransition property validation 
            _swappedAnim = !shouldMakeTransition;
            machine.player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchAnimKey);
        }

        public override void LogicUpdate()
        {
            // Do nothing if is in the quitting anim
            if (_quittingAnim) return;

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!_swappedAnim && _durationModule.HasElapsed(machine.player.data.crouchTransitionTime))
            {
                _swappedAnim = true;
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
                machine.crouchWalkState.SetActive();
                return;
            }

            // Switches to idle state if the player don't is pressing the crouch button 
            if (machine.player.input.virtualCrouching == false && machine.player.collisions.canStand)
                machine.player.StartCoroutine(_crouchMetadata.ExitCrouchState(machine.idleState));
        }
    }

    /// <summary>Player state when he is crouched and walking</summary>
    public class PlayerCrouchWalkState : PlayerStateBase
    {
        private PlayerCrouchMetadata _crouchMetadata;

        private PlayerDurationModule _durationMetadata;

        /// <summary>True when is quitting in transition anim. Used for not repeat the <see cref="_crouchMetadata.ExitCrouchState"/> process</summary>
        private bool _quittingAnim;

        /// <summary>Must be true if entry transition animation is already over</summary>
        private bool _swappedAnim;

        public PlayerCrouchWalkState(PlayerStateMachine machine) : base(machine)
        {
            _crouchMetadata = new PlayerCrouchMetadata(this);

            _durationMetadata = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            var shouldMakeTransition = !PlayerStatesUtility.IsCrouchState(previousState);

            // Resets the properties
            _durationMetadata.Enter();
            _quittingAnim = false;

            // shouldMakeTransition property validation 
            machine.player.collisions.SetCollisionsData(machine.player.data.crouchColliderData);
            _swappedAnim = !shouldMakeTransition;
            machine.player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchWalkAnimKey);
        }

        public override void LogicUpdate()
        {
            // Only move if is in the quitting anim
            if (_quittingAnim)
            {
                machine.player.MoveHorizontalAxesUsingInput(machine.player.data.crouchSpeed);
                return;
            }

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!_swappedAnim && _durationMetadata.HasElapsed(machine.player.data.crouchTransitionTime))
            {
                _swappedAnim = true;
                machine.player.animator.SwitchAnimation(PlayerAnimator.CrouchWalkAnimKey);
            }

            // Try enter in jump or fall state
            if (machine.EnterJump() != null || machine.EnterFallState() != null)
                return;

            // Switch to idle state
            if (machine.player.input.horizontalMove == 0)
            {
                machine.crouchState.SetActive();
                return;
            }

            // Switch to run state
            if (machine.player.input.virtualCrouching == false && machine.player.collisions.canStand)
            {
                machine.player.StartCoroutine(_crouchMetadata.ExitCrouchState(machine.runState));
                return;
            }

            // Try enter in slide or attack state
            if (machine.EnterSlideState() != null || machine.EnterAttackState() != null)
                return;

            // Perform movement 
            machine.player.MoveHorizontalAxesUsingInput(machine.player.data.crouchSpeed);
        }
    }

    /// <summary>Player state when he is crouching and starts a dash</summary>
    public class PlayerSlideState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        /// <summary>True when the quitting animation is running.</summary>
        private bool _quittingAnim;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown { get; private set; }

        public PlayerSlideState(PlayerStateMachine machine) : base(machine)
        {
            new PlayerCrouchMetadata(this);
            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets the properties
            _durationModule.Enter();
            isInCooldown = true;
            _quittingAnim = false;

            machine.player.collisions.SetCollisionsData(machine.player.data.crouchColliderData);
            machine.player.animator.SwitchAnimation(PlayerAnimator.SlideAnimKey);
        }

        public override void LogicUpdate()
        {
            // Starts the exit transition if it's near the end of the slide
            if (!_quittingAnim &&
                _durationModule.HasElapsed(machine.player.data.slideDuration - machine.player.data.slideTransitionTime))
            {
                machine.player.animator.SwitchAnimation(PlayerAnimator.SlideEndAnimKey);
                _quittingAnim = true;
            }

            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(machine.player.data.slideDuration))
            {
                machine.crouchState.SetActive();
                return;
            }

            // Try enter in fall state 
            if (machine.EnterFallState() != null)
                return;

            // Calculates and apply the speed
            var speed = machine.player.data.slideCurve.Evaluate(_durationModule.GetElapsedTime() / machine.player.data.slideDuration) *
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

    /// <summary>Player state when he is on wall-hand state and presses to climb it</summary>
    public class PlayerWallClimbState : PlayerStateBase
    {
        public PlayerWallClimbState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.WallClimbAnimKey);
        }
    }

    /// <summary>Player state when he is on the edge of a wall/floor</summary>
    public class PlayerWallHandState : PlayerStateBase
    {
        public PlayerWallHandState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.WallHandAnimKey);
        }
    }

    /// <summary>Player state when he is falling and walking towards the wall</summary>
    public class PlayerWallSlideState : PlayerStateBase
    {
        public PlayerWallSlideState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
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

    // TODO: Add interaction frames during the attack perform (to use the roll or other action)
    /// <summary>Class for use attack states</summary>
    public class PlayerAttackState : PlayerStateBase
    {
        protected PlayerDurationModule durationModule;

        /// <summary>Colliders hit on last trigger. Used for allocate hits array only once</summary>
        private readonly Collider2D[] _hits = new Collider2D[8];

        /// <summary>The attack data that stores the collision rect, move offset, damage...</summary>
        public readonly PlayerDataChannel.Attack attackData;

        /// <summary>The animation key of this attack</summary>
        public readonly string animKey;

        /// <summary>The collider data of this attack</summary>
        public readonly PlayerDataChannel.ColliderData colliderData;

        /// <summary>On reach the end and this prop don't is null, switch to this state</summary>
        public PlayerAttackState nextAttackState;

        /// <summary>Is the state triggered? Used to trigger the attack only once</summary>
        protected bool triggered { get; set; }

        public PlayerAttackState(PlayerStateMachine machine, PlayerDataChannel.Attack attackData, string animKey, PlayerDataChannel.ColliderData colliderData) :
            base(machine)
        {
            this.attackData = attackData;
            this.animKey = animKey;
            this.colliderData = colliderData;

            durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets all properties
            durationModule.Enter();
            triggered = false;

            // Set player's horizontal velocity to 0 and switch the animation
            machine.player.SetHorizontalVelocity(0);
            machine.player.animator.SwitchAnimation(animKey);
            machine.player.collisions.SetCollisionsData(colliderData);
        }

        public override void LogicUpdate()
        {
            // Check if the player is not floating
            if (machine.EnterFallState() != null)
                return;

            // Trigger the attack if it hasn't triggered and the elapsedTime is greater than or equal to the triggerTime
            if (!triggered && durationModule.HasElapsed(attackData.triggerTime))
            {
                triggered = true;
                TriggerAttack();
            }

            // Exit the state if the elapsedTime is greater than or equal to the attack duration 
            if (!durationModule.HasElapsed(attackData.duration)) return;

            // Switch to the next attack if it is not null and the player is holding attack button 
            if (machine.player.input.virtualAttacking && nextAttackState != null)
            {
                nextAttackState.SetActive();
                return;
            }

            // Switch to the preferred idle state in machine, like fall, crouching or the default idle.
            machine.EnterIdleState();
        }

        /// <summary>Triggers the attack, note that the method don't set or follow the property <see cref="triggered"/></summary>
        private void TriggerAttack()
        {
            // Moves the player's position by the offset defined in the attack data
            machine.player.rb.MovePosition(machine.player.rb.position +
                new Vector2(attackData.horizontalMoveOffset * machine.player.facingDirection, 0));

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

    /// <summary>Player state after take a hit</summary>
    public class PlayerHurtState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        /// <summary>The force that will be applied to the rigidbody upon entering this state</summary>
        public Vector2 knockbackForce { get; set; }

        public PlayerHurtState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Reset elapsed time
            _durationModule.Enter();

            // Reset the player velocity and add the knockback force to the player rigidbody force
            machine.player.rb.velocity = Vector2.zero;
            machine.player.rb.AddForce(knockbackForce, ForceMode2D.Impulse);

            machine.player.collisions.SetCollisionsData(machine.player.data.standColliderData);
            machine.player.animator.SwitchAnimation(PlayerAnimator.HurtAnimKey);
        }

        public override void LogicUpdate()
        {
            // Enter in a idle state when the state ends
            if (_durationModule.HasElapsed(machine.player.data.hurtTime))
                machine.EnterIdleState();
        }
    }

    // TODO: Implement death state
    /// <summary>Player state for handle he death</summary>
    public class PlayerDeathState : PlayerStateBase
    {
        public PlayerDeathState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            machine.player.animator.SwitchAnimation(PlayerAnimator.DeathAnimKey);
        }
    }
}