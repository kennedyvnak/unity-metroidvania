using Metroidvania.Entities;
using UnityEngine;

namespace Metroidvania.Characters.Knight
{
    public abstract class KnightStateBase
    {
        public readonly KnightStateMachine machine;

        public KnightCharacterController character => machine.character;

        public virtual bool isCrouchState => false;
        public virtual bool isInvincible => false;

        protected KnightStateBase(KnightStateMachine machine)
        {
            this.machine = machine;
        }

        public abstract bool CanEnter();

        public virtual void Enter(KnightStateBase previousState) { }

        public virtual void Transition() { }
        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }

        public virtual void Exit() { }

        public virtual bool TryEnter()
        {
            if (CanEnter())
            {
                machine.EnterState(this);
                return true;
            }
            return false;
        }

        public virtual void HandleJump()
        {
            machine.jumpState.TryEnter();
        }

        public virtual void HandleDash()
        {
            machine.rollState.TryEnter();
        }

        public virtual void HandleAttack()
        {
            machine.TryEnterAttackState();
        }
    }

    public class KnightIdleState : KnightStateBase
    {
        public KnightIdleState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.IdleAnimHash);
        }

        public override void Transition()
        {
            if (!(machine.fallState.TryEnter() || machine.crouchIdleState.TryEnter() || machine.crouchWalkState.TryEnter()) && character.horizontalMove != 0)
                machine.EnterState(machine.runState);
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(Vector2.zero, Time.deltaTime, character.data.slideMovement);
        }
    }

    public class KnightRunState : KnightStateBase
    {
        public KnightRunState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return Mathf.Abs(character.horizontalMove) > 0.0f && character.collisionChecker.isGrounded && !character.collisionChecker.CollidingInWall(character.horizontalMove);
        }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Transition()
        {
            if (!(machine.fallState.TryEnter() || machine.crouchIdleState.TryEnter() || machine.crouchWalkState.TryEnter()) && character.horizontalMove == 0)
                machine.EnterState(machine.idleState);
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(new Vector2(character.data.moveSpeed * character.horizontalMove, 0.0f), Time.deltaTime, character.data.slideMovement);
            character.FlipFacingDirection(character.horizontalMove);
        }
    }

    public class KnightJumpState : KnightStateBase
    {
        private bool _jumpPressed;

        public KnightJumpState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return character.jumpAction.IsPressed() && character.canStand && character.collisionChecker.isGrounded;
        }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.JumpAnimHash);
            character.particles.jump.Play();

            character.rb.linearVelocityY = character.data.jumpHeight;
            _jumpPressed = true;
        }

        public override void Transition()
        {
            if (character.rb.linearVelocityY < 0.0f)
                machine.EnterDefaultState();
        }

        public override void Update()
        {
            _jumpPressed = character.jumpAction.IsPressed();
        }

        public override void PhysicsUpdate()
        {
            if (!_jumpPressed)
            {
                character.rb.linearVelocityY += (character.data.jumpLowMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime;
            }
            if (character.collisionChecker.CollidingInWall(character.horizontalMove))
            {
                character.rb.linearVelocityX = 0.0f;
            }
            else
            {
                character.rb.linearVelocityX = character.data.airMoveSpeed * character.horizontalMove;
                character.FlipFacingDirection(character.horizontalMove);
            }
        }

        public override void HandleJump() { }
    }

    public class KnightFallState : KnightStateBase
    {
        private float _fallStartPositionY;

        public KnightFallState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return !character.collisionChecker.isGrounded;
        }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.FallAnimHash);
            _fallStartPositionY = character.rb.position.y;
        }

        public override void Transition()
        {
            if (character.collisionChecker.isGrounded)
            {
                if (_fallStartPositionY - character.rb.position.y > character.data.fallParticlesDistance)
                    character.particles.landing.Play();
                machine.EnterDefaultState();
            }
            else
            {
                machine.wallslideState.TryEnter();
            }
        }

        public override void PhysicsUpdate()
        {
            if (!character.collisionChecker.CollidingInWall(character.horizontalMove))
                character.rb.linearVelocityX = character.data.airMoveSpeed * character.horizontalMove;
            else
                character.rb.linearVelocityX = 0.0f;

            character.rb.linearVelocityY += (character.data.jumpFallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime;
            character.FlipFacingDirection(character.horizontalMove);
        }
    }

    public class KnightRollState : KnightStateBase
    {
        public override bool isInvincible => true;

        private float _elapsedTime;
        private float _lastExitTime;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.rollCooldown;

        public KnightRollState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return character.collisionChecker.isGrounded && !machine.currentState.isCrouchState && character.dashAction.WasPerformedThisFrame() && !isInCooldown;
        }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RollAnimHash, true);
            character.FlipFacingDirection(character.facingDirection);
        }

        public override void Transition()
        {

            if (_elapsedTime > character.data.rollDuration)
                machine.EnterDefaultState();
            else
                machine.fallState.TryEnter();
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

        }

        public override void PhysicsUpdate()
        {
            float curveMultiplier = character.data.rollHorizontalMoveCurve.Evaluate(_elapsedTime / character.data.rollDuration);
            character.rb.Slide(new Vector2(character.data.rollSpeed * curveMultiplier * character.facingDirection, 0.0f), Time.deltaTime, character.data.slideMovement);
        }

        public override void Exit()
        {
            _lastExitTime = Time.time;
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }
    }

    public abstract class KnightCrouchStateBase : KnightStateBase
    {
        public override bool isCrouchState => true;

        public KnightCrouchStateBase(KnightStateMachine machine) : base(machine) { }

        public override void HandleJump()
        {
            character.TryDropPlatform();
        }

        public override void HandleDash()
        {
            machine.slideState.TryEnter();
        }

        public override void HandleAttack()
        {
            machine.crouchAttackState.TryEnter();
        }
    }

    public class KnightCrouchIdleState : KnightCrouchStateBase
    {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchIdleState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return (character.crouchAction.IsPressed() || !character.canStand) && character.collisionChecker.isGrounded && character.horizontalMove == 0;
        }

        public override void Enter(KnightStateBase previousState)
        {
            bool shouldMakeTransition = !previousState.isCrouchState;

            _elapsedTime = 0;
            _quittingAnimElapsedTime = 0;
            _inQuittingAnim = false;

            character.SetColliderBounds(character.data.crouchColliderBounds);

            _hasSwappedAnim = !shouldMakeTransition;
            character.SwitchAnimation(shouldMakeTransition
                ? KnightCharacterController.CrouchTransitionAnimHash
                : KnightCharacterController.CrouchIdleAnimHash);
        }

        public override void Transition()
        {
            if (!(machine.fallState.TryEnter() || machine.crouchWalkState.TryEnter()) && _quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                machine.EnterState(machine.idleState);
        }

        public override void Update()
        {
            if (_inQuittingAnim)
                _quittingAnimElapsedTime += Time.deltaTime;

            _elapsedTime += Time.deltaTime;

            if (!_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime)
            {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchIdleAnimHash);
            }
            else if (!character.crouchAction.IsPressed() && character.canStand)
            {
                _inQuittingAnim = true;
            }

            if (character.jumpAction.WasPerformedThisFrame())
                character.TryDropPlatform();
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(Vector2.zero, Time.deltaTime, character.data.slideMovement);
        }
    }

    public class KnightCrouchWalkState : KnightCrouchStateBase
    {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchWalkState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return (character.crouchAction.IsPressed() || !character.canStand) && character.collisionChecker.isGrounded && character.horizontalMove != 0;
        }

        public override void Enter(KnightStateBase previousState)
        {
            bool shouldMakeTransition = !previousState.isCrouchState;

            _elapsedTime = 0;
            _inQuittingAnim = false;
            _quittingAnimElapsedTime = 0;

            character.SetColliderBounds(character.data.crouchColliderBounds);

            _hasSwappedAnim = !shouldMakeTransition;
            character.SwitchAnimation(shouldMakeTransition
                ? KnightCharacterController.CrouchTransitionAnimHash
                : KnightCharacterController.CrouchWalkAnimHash);
        }

        public override void Transition()
        {
            if (!(machine.fallState.TryEnter() || machine.crouchIdleState.TryEnter()) && _quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                machine.EnterState(machine.idleState);
        }

        public override void Update()
        {
            if (_inQuittingAnim)
                _quittingAnimElapsedTime += Time.deltaTime;

            _elapsedTime += Time.deltaTime;

            if (!_inQuittingAnim && !_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime)
            {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchWalkAnimHash);
            }

            if (!character.crouchAction.IsPressed() && character.canStand)
                _inQuittingAnim = true;

            if (character.jumpAction.WasPerformedThisFrame())
                character.TryDropPlatform();
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(new Vector2(character.data.crouchWalkSpeed * character.horizontalMove, 0.0f), Time.deltaTime, character.data.slideMovement);
            character.FlipFacingDirection(character.horizontalMove);
        }
    }

    public class KnightSlideState : KnightStateBase
    {
        public override bool isCrouchState => true;
        public override bool isInvincible => true;

        private float _elapsedTime;
        private float _lastExitTime = int.MinValue;

        private bool _inQuittingAnim;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.slideCooldown;

        public KnightSlideState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return character.collisionChecker.isGrounded && machine.currentState.isCrouchState && character.dashAction.WasPerformedThisFrame() && !isInCooldown;
        }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            _inQuittingAnim = false;

            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.SwitchAnimation(KnightCharacterController.SlideAnimHash, true);
            character.particles.slide.Play();
            character.FlipFacingDirection(character.facingDirection);
        }

        public override void Transition()
        {
            if (_elapsedTime > character.data.slideDuration)
                machine.EnterState(machine.crouchIdleState);
            else
                machine.fallState.TryEnter();
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (!_inQuittingAnim && _elapsedTime > character.data.slideDuration - character.data.slideTransitionTime)
            {
                character.SwitchAnimation(KnightCharacterController.SlideEndAnimHash);
                _inQuittingAnim = true;
            }
        }

        public override void PhysicsUpdate()
        {
            float slideProgress = _elapsedTime / character.data.slideDuration;
            float curveMultiplier = character.data.slideMoveCurve.Evaluate(slideProgress);
            character.rb.Slide(new Vector2(character.data.slideSpeed * curveMultiplier * character.facingDirection, 0.0f), Time.deltaTime, character.data.slideMovement);
        }

        public override void Exit()
        {
            _lastExitTime = Time.time;
            character.particles.slide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }
    }

    public class KnightWallslideState : KnightStateBase
    {
        public KnightWallslideState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter()
        {
            return !character.collisionChecker.isGrounded && character.collisionChecker.CollidingInWall(character.horizontalMove) && character.horizontalMove == character.facingDirection;
        }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.WallslideAnimHash);
            character.particles.wallslide.Play();
            character.rb.linearVelocityX = 0.0f;
        }

        public override void Transition()
        {
            if (!CanEnter())
                machine.EnterDefaultState();
        }

        public override void PhysicsUpdate()
        {
            character.rb.linearVelocityY = -character.data.wallSlideSpeed;
        }

        public override void Exit()
        {
            character.particles.wallslide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public override void HandleJump()
        {
            machine.walljumpState.TryEnter();
        }

        public override void HandleDash() { }
        public override void HandleAttack() { }
    }

    public class KnightWalljumpState : KnightStateBase
    {
        private float _elapsedTime;

        public KnightWalljumpState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.JumpAnimHash);
            character.Flip();
            {
                ParticleSystem.ShapeModule shape = character.particles.walljump.shape;
                shape.rotation = new Vector3(0, 0, 90 * -character.facingDirection);
                character.particles.walljump.Play();
            }

            character.rb.linearVelocity = new Vector2(character.data.wallJumpForce.x * character.facingDirection, character.data.wallJumpForce.y);
        }

        public override void Transition()
        {
            if (_elapsedTime > character.data.wallJumpDuration)
                machine.EnterDefaultState();
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }
    }

    public class KnightAttackState : KnightStateBase
    {
        protected enum ExitAttackCommand { None, Roll, Slide }

        public static int lastStandAttack = 0;
        public static float lastAttackTime = 0;

        protected float _elapsedTime;
        protected bool _triggered;
        protected ExitAttackCommand _currentExitCommand;

        public readonly KnightData.Attack attackData;
        public readonly int animHash;
        public readonly KnightData.ColliderBounds colliderBounds;

        public KnightAttackState nextAttackState { get; set; }

        public KnightAttackState(KnightStateMachine machine, KnightData.Attack attackData, int animHash, KnightData.ColliderBounds colliderBounds) : base(machine)
        {
            this.attackData = attackData;
            this.animHash = animHash;
            this.colliderBounds = colliderBounds;
        }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            _triggered = false;
            _currentExitCommand = ExitAttackCommand.None;

            character.SetColliderBounds(colliderBounds);
            character.SwitchAnimation(animHash, true);
            character.rb.linearVelocityX = 0;
        }

        public override void Transition()
        {
            _ = machine.fallState.TryEnter();

            if (_elapsedTime < attackData.duration - attackData.attackEndOffset)
                return;

            switch (_currentExitCommand)
            {
                case ExitAttackCommand.Roll:
                    if (machine.rollState.isInCooldown)
                        break;
                    machine.EnterState(machine.rollState);
                    return;
                case ExitAttackCommand.Slide:
                    if (machine.slideState.isInCooldown)
                        break;
                    machine.EnterState(machine.slideState);
                    return;
            }

            if (character.attackAction.WasPerformedThisFrame())
            {
                if (nextAttackState.isCrouchState && !character.crouchAction.IsPressed() && character.canStand)
                    machine.EnterState(machine.firstAttackState);
                else if (!nextAttackState.isCrouchState && character.crouchAction.IsPressed())
                    machine.EnterState(machine.crouchAttackState);
                else
                    machine.EnterState(nextAttackState);
            }
            else if (_elapsedTime > attackData.duration)
                machine.EnterDefaultState();
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (character.dashAction.WasPerformedThisFrame())
            {
                _currentExitCommand = character.crouchAction.IsPressed() || !character.canStand ? ExitAttackCommand.Slide : ExitAttackCommand.Roll;
            }

            if (!_triggered && _elapsedTime >= attackData.triggerTime)
            {
                _triggered = true;
                character.PerformAttack(attackData);
            }
        }

        public override void Exit()
        {
            if (character.horizontalMove != 0)
                character.FlipTo((int)Mathf.Sign(character.horizontalMove));
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }

        public static int StepAttack(float attackComboMaxDelay)
        {
            lastStandAttack++;
            lastAttackTime = Time.time;

            if (lastStandAttack > 2 || Time.time - lastAttackTime >= attackComboMaxDelay)
                lastStandAttack = 1;

            return lastStandAttack;
        }
    }

    public class KnightCrouchAttackState : KnightAttackState
    {
        public override bool isCrouchState => true;

        public KnightCrouchAttackState(KnightStateMachine machine) : base(machine, machine.character.data.crouchAttack, KnightCharacterController.CrouchAttackAnimHash, machine.character.data.crouchColliderBounds) { }
    }

    public class KnightHurtState : KnightStateBase
    {
        public override bool isInvincible => true;

        private float _elapsedTime;

        public EntityHitData hitData { get; set; }

        public KnightHurtState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.HurtAnimHash);

            character.rb.linearVelocity = Vector2.zero;
            character.rb.AddForce(hitData.knockbackForce, ForceMode2D.Impulse);
        }

        public override void Transition()
        {
            if (_elapsedTime > character.data.hurtTime)
                machine.EnterDefaultState();
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }

        public void EnterHurtState(EntityHitData hitData)
        {
            this.hitData = hitData;
            machine.EnterState(this);
        }
    }

    public class KnightDieState : KnightStateBase
    {
        public override bool isInvincible => true;

        public KnightDieState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            character.SwitchAnimation(KnightCharacterController.DieAnimHash, true);
            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.rb.linearVelocity = Vector2.zero;
            character.data.onDieChannel.Raise(character);
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }
    }

    public class KnightFakeWalkState : KnightStateBase
    {
        public override bool isInvincible => true;

        private float _elapsedTime;

        public float currentWalkDuration { get; set; }

        public KnightFakeWalkState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Transition()
        {
            if (!machine.fallState.TryEnter() && _elapsedTime > currentWalkDuration)
            {
                machine.EnterDefaultState();
            }
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(new Vector2(character.facingDirection * character.data.moveSpeed, 0.0f), Time.deltaTime, character.data.slideMovement);
        }

        public override void HandleJump() { }
        public override void HandleDash() { }
        public override void HandleAttack() { }

        public void EnterFakeWalk(float duration)
        {
            currentWalkDuration = duration;
            machine.EnterState(this);
        }
    }

    public class KnightValidationState : KnightStateBase
    {
        public KnightValidationState(KnightStateMachine machine) : base(machine) { }

        public override bool CanEnter() => true;
    }
}
