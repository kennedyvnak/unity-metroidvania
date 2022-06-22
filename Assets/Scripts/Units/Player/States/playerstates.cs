using UnityEngine;

namespace Metroidvania.Player.States
{
    // TODO: Add sounds
    // TODO: Implement death state
    /// <summary>Base classes for all player states</summary>
    public abstract class PlayerStateBase
    {
        private readonly PlayerStateMetadatas _metadatas;
        public PlayerStateMetadatas metadatas => _metadatas;

        /// <summary>The player machine</summary>
        public readonly PlayerStateMachine machine;
        public PlayerController player => machine.player;

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
            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.SetHorizontalVelocity(0);
            player.animator.SwitchAnimation(PlayerAnimator.IdleAnimKey);
        }

        public override void LogicUpdate()
        {
            // Try switch states
            if (machine.EnterFallState() != null ||
                machine.EnterCrouchState() != null ||
                machine.EnterJumpState() != null ||
                machine.EnterRollState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Enter in run state if move input isn't equals 0
            if (player.input.horizontalMove != 0)
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
            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.animator.SwitchAnimation(PlayerAnimator.RunAnimKey);
        }

        public override void LogicUpdate()
        {
            // Try enter in other state
            if (machine.EnterFallState() != null ||
                machine.EnterCrouchState() != null ||
                machine.EnterJumpState() != null ||
                machine.EnterRollState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Enter in idle state if the move input is 0
            if (player.input.horizontalMove == 0)
            {
                machine.idleState.SetActive();
                return;
            }

            // Movement the player
            player.MoveHorizontalAxesUsingInput(player.data.moveSpeed);
        }
    }

    /// <summary>Player state when he is jumping</summary>
    public class PlayerJumpState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

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
            _collidedTop = false;

            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.animator.SwitchAnimation(PlayerAnimator.JumpAnimKey);
            player.CollisionEntered += CollisionEntered;
            player.particles.jump.Play();
        }

        public override void Exit()
        {
            player.CollisionEntered -= CollisionEntered;
        }

        public override void LogicUpdate()
        {
            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(player.data.jumpDuration) || _collidedTop)
            {
                machine.EnterIdleState();
                return;
            }

            // Exits the state if the jump was canceled
            if (player.input.jumpAction.WasReleasedThisFrame())
            {
                // Set player velocity.y to 0.15 to make a smooth jump stop
                player.rb.velocity = new Vector2(player.rb.velocity.x, .15f);
                machine.EnterIdleState();
                return;
            }

            // Calculates the horizontal speed
            float horizontalSpeed = player.input.horizontalMove * player.data.moveSpeed;

            // Calculates the vertical speed using the data.JumpCurve curve to make a smooth jump
            float jumpProgressNormalized = _durationModule.GetElapsedTime() / player.data.jumpDuration;
            float jumpCurveSpeed = player.data.jumpCurve.Evaluate(jumpProgressNormalized);
            float verticalSpeed = player.data.jumpSpeed * jumpCurveSpeed;

            player.rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);

            player.animator.FlipCheck();
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
        private float _fallStartY;

        public PlayerFallState(PlayerStateMachine machine) : base(machine)
        {
        }

        public override void Enter(PlayerStateBase previousState)
        {
            player.animator.SwitchAnimation(PlayerAnimator.FallAnimKey);
            _fallStartY = player.rb.position.y;
        }

        public override void LogicUpdate()
        {
            if (player.collisions.isGrounded)
            {
                if (_fallStartY - player.rb.position.y > player.data.fallParticlesDistance)
                    player.particles.landing.Play();
                machine.EnterIdleState();
                return;
            }

            if (machine.EnterWallState() != null)
                return;

            player.MoveHorizontalAxesUsingInput(player.data.moveSpeed);
        }
    }

    /// <summary>Player state when he is standing and starts a dash</summary>
    public class PlayerRollState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        private PlayerCooldownModule _cooldownModule;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown => _cooldownModule.IsInCooldown();

        public PlayerRollState(PlayerStateMachine machine) : base(machine)
        {
            _durationModule = new PlayerDurationModule(this);
            _cooldownModule = new PlayerCooldownModule(this, player.data.rollCooldown);
            new PlayerInvincibilityModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets the properties
            _durationModule.Enter();

            player.collisions.SetCollisionsData(player.data.standColliderData);

            player.animator.SwitchAnimation(PlayerAnimator.RollAnimKey, true);
        }

        public override void LogicUpdate()
        {
            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(player.data.rollDuration))
            {
                machine.idleState.SetActive();
                return;
            }

            // Calculates the vertical speed using the data.rollCurve curve to make a smooth roll
            float speed = player.data.rollSpeed * player.facingDirection;
            player.SetHorizontalVelocity(
                player.data.rollCurve.Evaluate(_durationModule.GetElapsedTime() / player.data.rollDuration) * speed);
        }

        public override void Exit()
        {
            _cooldownModule.StartCooldown();
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
            bool shouldMakeTransition = !PlayerStatesUtility.IsCrouchState(previousState);

            _durationModule.Enter();
            _quittingAnim = false;

            // Set player's horizontal velocity to 0
            player.SetHorizontalVelocity(0);
            player.collisions.SetCollisionsData(player.data.crouchColliderData);

            // shouldMakeTransition property validation 
            _swappedAnim = !shouldMakeTransition;
            player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchAnimKey);
        }

        public override void LogicUpdate()
        {
            // Do nothing if is in the quitting anim
            if (_quittingAnim) return;

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!_swappedAnim && _durationModule.HasElapsed(player.data.crouchTransitionTime))
            {
                _swappedAnim = true;
                player.animator.SwitchAnimation(PlayerAnimator.CrouchAnimKey);
            }

            // Try enter in other states
            if (machine.EnterJumpState() != null ||
                machine.EnterFallState() != null ||
                machine.EnterSlideState() != null ||
                machine.EnterAttackState() != null)
                return;

            // Switches to crouch walk state if the input horizontal move isn't equal 0
            if (player.input.horizontalMove != 0)
            {
                machine.crouchWalkState.SetActive();
                return;
            }

            // Switches to idle state if the player don't is pressing the crouch button 
            if (!player.input.crouchAction.IsPressed() && player.collisions.canStand)
                player.StartCoroutine(_crouchMetadata.ExitCrouchState(machine.idleState));
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
            bool shouldMakeTransition = !PlayerStatesUtility.IsCrouchState(previousState);

            // Resets the properties
            _durationMetadata.Enter();
            _quittingAnim = false;

            // shouldMakeTransition property validation 
            player.collisions.SetCollisionsData(player.data.crouchColliderData);
            _swappedAnim = !shouldMakeTransition;
            player.animator.SwitchAnimation(shouldMakeTransition
                ? PlayerAnimator.CrouchTransitionAnimKey
                : PlayerAnimator.CrouchWalkAnimKey);
        }

        public override void LogicUpdate()
        {
            // Only move if is in the quitting anim
            if (_quittingAnim)
            {
                player.MoveHorizontalAxesUsingInput(player.data.crouchSpeed);
                return;
            }

            // Switches to idle animation if not previously switched and elapsedTime is greater than or equal to transition time  
            if (!_swappedAnim && _durationMetadata.HasElapsed(player.data.crouchTransitionTime))
            {
                _swappedAnim = true;
                player.animator.SwitchAnimation(PlayerAnimator.CrouchWalkAnimKey);
            }

            // Try enter in jump or fall state
            if (machine.EnterJumpState() != null || machine.EnterFallState() != null)
                return;

            // Switch to idle state
            if (player.input.horizontalMove == 0)
            {
                machine.crouchState.SetActive();
                return;
            }

            // Switch to run state
            if (!player.input.crouchAction.IsPressed() && player.collisions.canStand)
            {
                player.StartCoroutine(_crouchMetadata.ExitCrouchState(machine.runState));
                return;
            }

            // Try enter in slide or attack state
            if (machine.EnterSlideState() != null || machine.EnterAttackState() != null)
                return;

            // Perform movement 
            player.MoveHorizontalAxesUsingInput(player.data.crouchSpeed);
        }
    }

    /// <summary>Player state when he is crouching and starts a dash</summary>
    public class PlayerSlideState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        private PlayerCooldownModule _cooldownModule;

        /// <summary>True when the quitting animation is running.</summary>
        private bool _quittingAnim;

        /// <summary>Boolean to controls the slide cooldown</summary>
        public bool isInCooldown => _cooldownModule.IsInCooldown();

        public PlayerSlideState(PlayerStateMachine machine) : base(machine)
        {
            new PlayerCrouchMetadata(this);
            _durationModule = new PlayerDurationModule(this);
            _cooldownModule = new PlayerCooldownModule(this, player.data.slideCooldown);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Resets the properties
            _durationModule.Enter();
            _quittingAnim = false;

            player.collisions.SetCollisionsData(player.data.crouchColliderData);
            player.animator.SwitchAnimation(PlayerAnimator.SlideAnimKey, true);
            player.particles.slide.Play();
        }

        public override void Exit()
        {
            _cooldownModule.StartCooldown();
            player.particles.slide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public override void LogicUpdate()
        {
            // Starts the exit transition if it's near the end of the slide
            if (!_quittingAnim && _durationModule.HasElapsed(player.data.slideDuration - player.data.slideTransitionTime))
            {
                player.animator.SwitchAnimation(PlayerAnimator.SlideEndAnimKey, true);
                _quittingAnim = true;
            }

            // Exits the state if the elapsed time reaches the duration
            if (_durationModule.HasElapsed(player.data.slideDuration))
            {
                machine.crouchState.SetActive();
                return;
            }

            // Try enter in fall state 
            if (machine.EnterFallState() != null)
                return;

            float slideProgressNormalized = _durationModule.GetElapsedTime() / player.data.slideDuration;
            float slideCurveSpeed = player.data.slideCurve.Evaluate(slideProgressNormalized);
            float speed = slideCurveSpeed * player.data.slideSpeed * player.facingDirection;
            player.SetHorizontalVelocity(speed);
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
            player.animator.SwitchAnimation(PlayerAnimator.WallSlideAnimKey);
            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.particles.wallSlide.Play();
        }

        public override void Exit()
        {
            player.particles.wallSlide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public override void LogicUpdate()
        {
            if (player.input.jumpAction.WasPerformedThisFrame())
                machine.wallJumpState.SetActive();
            else if (player.collisions.isGrounded || !player.collisions.isTouchingWall || player.input.horizontalMove != player.facingDirection)
                machine.EnterIdleState();
            else
                player.rb.velocity = new Vector2(0, -player.data.wallSlideSpeed);
        }
    }

    public class PlayerWallJumpState : PlayerStateBase
    {
        private PlayerDurationModule _durationModule;

        private bool _collidedInAnything;

        public PlayerWallJumpState(PlayerStateMachine machine) : base(machine)
        {
            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            _collidedInAnything = false;

            player.animator.SwitchAnimation(PlayerAnimator.JumpAnimKey);
            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.animator.Flip();
            {
                ParticleSystem.ShapeModule shape = player.particles.wallJump.shape;
                shape.rotation = new Vector3(0, 0, 90 * -player.facingDirection);
                player.particles.wallJump.Play();
            }
            player.rb.velocity = GetJumpForce();

            player.CollisionEntered += OnCollisionEnter;
            _durationModule.Enter();
        }

        public override void Exit()
        {
            player.CollisionEntered -= OnCollisionEnter;
        }

        public override void LogicUpdate()
        {
            if (_durationModule.HasElapsed(player.data.wallJumpDuration) || _collidedInAnything)
            {
                machine.EnterIdleState();
                return;
            }
        }

        private void OnCollisionEnter(Collision2D collision)
        {
            _collidedInAnything = true;
        }

        public Vector2 GetJumpForce()
        {
            return new Vector2(player.data.wallJumpForce.x * player.facingDirection, player.data.wallJumpForce.y);
        }
    }

    /// <summary>Class for use attack states</summary>
    public class PlayerAttackState : PlayerStateBase
    {
        protected enum ExitAttackCommand { None, Roll, Slide }

        protected PlayerDurationModule durationModule;

        /// <summary>The attack data that stores the collision rect, move offset, damage...</summary>
        public readonly PlayerDataChannel.Attack attackData;

        /// <summary>The animation key of this attack</summary>
        public readonly int animKey;

        /// <summary>The collider data of this attack</summary>
        public readonly PlayerDataChannel.ColliderData colliderData;

        /// <summary>On reach the end and this prop don't is null, switch to this state</summary>
        public PlayerAttackState nextAttackState;

        /// <summary>Is the state triggered? Used to trigger the attack only once</summary>
        protected bool triggered { get; set; }
        protected ExitAttackCommand currentExitCommand { get; set; }

        public PlayerAttackState(PlayerStateMachine machine, PlayerDataChannel.Attack attackData, int animKey, PlayerDataChannel.ColliderData colliderData) :
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
            currentExitCommand = ExitAttackCommand.None;

            // Set player's horizontal velocity to 0 and switch the animation
            player.SetHorizontalVelocity(0);
            player.animator.SwitchAnimation(animKey, true);
            player.collisions.SetCollisionsData(colliderData);
        }

        public override void Exit()
        {
            player.animator.FlipCheck();
        }

        public override void LogicUpdate()
        {
            // Check if the player is not floating
            if (machine.EnterFallState() != null)
                return;

            if (player.input.dashAction.WasPerformedThisFrame())
            {
                if (player.input.crouchAction.IsPressed())
                    currentExitCommand = ExitAttackCommand.Slide;
                else if (player.collisions.canStand)
                    currentExitCommand = ExitAttackCommand.Roll;
            }

            // Trigger the attack if it hasn't triggered and the elapsedTime is greater than or equal to the triggerTime
            if (!triggered && durationModule.HasElapsed(attackData.triggerTime))
            {
                triggered = true;
                player.combat.PerformAttack(attackData);
            }

            // Exit the state if the elapsedTime is greater than or equal to the attack duration 
            if (!durationModule.HasElapsed(attackData.duration - attackData.attackEndOffset)) return;

            switch (currentExitCommand)
            {
                case ExitAttackCommand.None:
                    if (!player.input.attackAction.IsPressed() || nextAttackState == null) break;

                    if (PlayerStatesUtility.IsCrouchState(this) && !player.input.crouchAction.IsPressed() && player.collisions.canStand)
                        machine.attackOneState.SetActive();
                    else if (player.input.crouchAction.IsPressed())
                        machine.crouchAttackState.SetActive();
                    else nextAttackState.SetActive();
                    return;

                case ExitAttackCommand.Roll:
                    if (machine.rollState.isInCooldown) break;
                    machine.rollState.SetActive();
                    return;

                case ExitAttackCommand.Slide:
                    if (machine.slideState.isInCooldown) break;
                    machine.slideState.SetActive();
                    return;
            }

            if (durationModule.HasElapsed(attackData.duration))
                machine.EnterIdleState();
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
            _durationModule = new PlayerDurationModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            // Reset elapsed time
            _durationModule.Enter();

            // Reset the player velocity and add the knockback force to the player rigidbody force
            player.rb.velocity = Vector2.zero;
            player.rb.AddForce(knockbackForce, ForceMode2D.Impulse);

            player.collisions.SetCollisionsData(player.data.standColliderData);
            player.animator.SwitchAnimation(PlayerAnimator.HurtAnimKey);
        }

        public override void LogicUpdate()
        {
            // Enter in a idle state when the state ends
            if (_durationModule.HasElapsed(player.data.hurtTime))
                machine.EnterIdleState();
        }
    }

    /// <summary>Player state for handle he death</summary>
    public class PlayerDiedState : PlayerStateBase
    {
        public PlayerDiedState(PlayerStateMachine machine) : base(machine)
        {
            new PlayerInvincibilityModule(this);
        }

        public override void Enter(PlayerStateBase previousState)
        {
            player.animator.SwitchAnimation(PlayerAnimator.DeathAnimKey, true);
            player.collisions.SetCollisionsData(player.data.crouchColliderData);
            player.rb.velocity = Vector2.zero;
            player.data.onDieChannel?.Raise(player);
        }
    }
}