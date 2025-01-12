using Metroidvania.Entities;
using UnityEngine;

namespace Metroidvania.Characters.Knight
{
    using KnightStateBase = CharacterStateBase<KnightCharacterController>;

    public interface ICrouchState { }
    public interface IInvincibleState { }

    public class KnightIdleState : KnightStateBase
    {
        public KnightIdleState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.IdleAnimHash);
        }

        public override void Update()
        {
            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterCrouchState() ||
                character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterRollState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove != 0)
                machine.EnterState(character.stateMachine.runState);
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(Vector2.zero, Time.deltaTime, character.data.slideMovement);
        }
    }

    public class KnightRunState : KnightStateBase
    {
        public KnightRunState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Update()
        {
            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterCrouchState() ||
                character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterRollState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove == 0)
                machine.EnterState(character.stateMachine.idleState);
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(new Vector2(character.data.moveSpeed * character.horizontalMove, 0.0f), Time.deltaTime, character.data.slideMovement);
            character.FlipByVelocity(character.horizontalMove);
        }
    }

    public class KnightJumpState : KnightStateBase
    {
        public KnightJumpState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.JumpAnimHash);
            character.particles.jump.Play();

            character.rb.linearVelocityY = character.data.jumpHeight;
        }

        public override void Update()
        {
            if (character.rb.linearVelocityY < 0.0f)
            {
                character.stateMachine.EnterDefaultState();
            }

            character.SetHorizontalVelocity(character.data.airMoveSpeed * character.horizontalMove);
            character.FlipByVelocity();

            if (character.rb.linearVelocityY > 0.0f && !character.jumpAction.IsPressed())
            {
                character.rb.linearVelocity += (character.data.jumpLowMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
    }

    public class KnightFallState : KnightStateBase
    {
        private float _fallStartPositionY;

        public KnightFallState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.FallAnimHash);
            _fallStartPositionY = character.rb.position.y;
        }

        public override void Update()
        {
            if (character.collisionChecker.isGrounded)
            {
                if (_fallStartPositionY - character.rb.position.y > character.data.fallParticlesDistance)
                    character.particles.landing.Play();
                character.stateMachine.EnterDefaultState();
            }
            else if (!character.stateMachine.EnterWallState())
            {
                character.SetHorizontalVelocity(character.data.airMoveSpeed * character.horizontalMove);
                character.FlipByVelocity();
                character.rb.linearVelocity += (character.data.jumpFallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
    }

    public class KnightRollState : KnightStateBase, IInvincibleState
    {
        private float _elapsedTime;
        private float _lastExitTime;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.rollCooldown;

        public KnightRollState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RollAnimHash, true);
            character.FlipByVelocity(character.facingDirection);
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.rollDuration)
            {
                character.stateMachine.EnterDefaultState();
            }
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
    }

    public class KnightCrouchIdleState : KnightStateBase, ICrouchState
    {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchIdleState(KnightStateMachine machine) : base(machine)
        {
        }

        public override void Enter(KnightStateBase previousState)
        {
            bool shouldMakeTransition = previousState is not ICrouchState;

            _elapsedTime = 0;
            _quittingAnimElapsedTime = 0;
            _inQuittingAnim = false;

            character.SetColliderBounds(character.data.crouchColliderBounds);

            _hasSwappedAnim = !shouldMakeTransition;
            character.SwitchAnimation(shouldMakeTransition
                ? KnightCharacterController.CrouchTransitionAnimHash
                : KnightCharacterController.CrouchIdleAnimHash);
        }

        public override void Update()
        {
            if (_inQuittingAnim)
            {
                _quittingAnimElapsedTime += Time.deltaTime;
                if (_quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                    machine.EnterState(character.stateMachine.idleState);
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (!_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime)
            {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchIdleAnimHash);
            }

            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterSlideState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove != 0)
                machine.EnterState(character.stateMachine.crouchWalkState);
            else if (!character.crouchAction.IsPressed() && character.canStand)
                _inQuittingAnim = true;

            if (character.jumpAction.WasPerformedThisFrame())
            {
                character.TryDropPlatform();
            }
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(Vector2.zero, Time.deltaTime, character.data.slideMovement);
        }
    }

    public class KnightCrouchWalkState : KnightStateBase, ICrouchState
    {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchWalkState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            bool shouldMakeTransition = previousState is not ICrouchState;

            _elapsedTime = 0;
            _inQuittingAnim = false;
            _quittingAnimElapsedTime = 0;

            character.SetColliderBounds(character.data.crouchColliderBounds);

            _hasSwappedAnim = !shouldMakeTransition;
            character.SwitchAnimation(shouldMakeTransition
                ? KnightCharacterController.CrouchTransitionAnimHash
                : KnightCharacterController.CrouchWalkAnimHash);
        }

        public override void Update()
        {
            if (_inQuittingAnim)
            {
                _quittingAnimElapsedTime += Time.deltaTime;
                if (_quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                    machine.EnterState(character.stateMachine.idleState);
                else
                    character.SetHorizontalVelocity(character.data.crouchWalkSpeed * character.horizontalMove);
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (!_inQuittingAnim && !_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime)
            {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchWalkAnimHash);
            }

            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterSlideState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove == 0)
                machine.EnterState(character.stateMachine.crouchIdleState);
            else if (!character.crouchAction.IsPressed() && character.canStand)
                _inQuittingAnim = true;

            if (character.jumpAction.WasPerformedThisFrame())
            {
                character.TryDropPlatform();
            }
        }

        public override void PhysicsUpdate()
        {
            character.rb.Slide(new Vector2(character.data.crouchWalkSpeed * character.horizontalMove, 0.0f), Time.deltaTime, character.data.slideMovement);
            character.FlipByVelocity(character.horizontalMove);
        }
    }

    public class KnightSlideState : KnightStateBase, ICrouchState
    {
        private float _elapsedTime;
        private float _lastExitTime = int.MinValue;

        private bool _inQuittingAnim;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.slideCooldown;

        public KnightSlideState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            _inQuittingAnim = false;

            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.SwitchAnimation(KnightCharacterController.SlideAnimHash, true);
            character.particles.slide.Play();
            character.FlipByVelocity(character.facingDirection);
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (!_inQuittingAnim && _elapsedTime > character.data.slideDuration - character.data.slideTransitionTime)
            {
                character.SwitchAnimation(KnightCharacterController.SlideEndAnimHash);
                _inQuittingAnim = true;
            }

            if (_elapsedTime > character.data.slideDuration)
                machine.EnterState(character.stateMachine.crouchIdleState);
            else if (!character.stateMachine.EnterFallState()) { }
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
    }

    public class KnightWallslideState : KnightStateBase
    {
        public KnightWallslideState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.WallslideAnimHash);
            character.particles.wallslide.Play();
        }

        public override void Update()
        {
            if (character.jumpAction.WasPerformedThisFrame())
                machine.EnterState(character.stateMachine.walljumpState);
            else if (character.collisionChecker.isGrounded || !character.collisionChecker.CollidingInWall(character.horizontalMove) || character.horizontalMove != character.facingDirection)
                character.stateMachine.EnterDefaultState();
            else
                character.rb.linearVelocity = new Vector2(0, -character.data.wallSlideSpeed);
        }

        public override void Exit()
        {
            character.particles.wallslide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public class KnightWalljumpState : KnightStateBase
    {
        private float _elapsedTime;

        private bool _collidedWithSomething;

        public KnightWalljumpState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            _collidedWithSomething = false;

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

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.wallJumpDuration || _collidedWithSomething)
                character.stateMachine.EnterDefaultState();
        }

        private void OnCollisionEnter(Collision2D collision)
        {
            _collidedWithSomething = true;
        }
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

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            _triggered = false;
            _currentExitCommand = ExitAttackCommand.None;

            character.SetColliderBounds(colliderBounds);
            character.SwitchAnimation(animHash, true);
            character.rb.linearVelocity = new Vector2(0, character.rb.linearVelocity.y);
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (character.stateMachine.EnterFallState())
                return;

            if (character.dashAction.WasPerformedThisFrame())
            {
                _currentExitCommand = character.crouchAction.IsPressed() || !character.canStand ? ExitAttackCommand.Slide : ExitAttackCommand.Roll;
            }

            if (!_triggered && _elapsedTime >= attackData.triggerTime)
            {
                _triggered = true;
                character.PerformAttack(attackData);
            }

            if (_elapsedTime < attackData.duration - attackData.attackEndOffset)
                return;

            switch (_currentExitCommand)
            {
                case ExitAttackCommand.Roll:
                    if (character.stateMachine.rollState.isInCooldown)
                        break;
                    machine.EnterState(character.stateMachine.rollState);
                    return;
                case ExitAttackCommand.Slide:
                    if (character.stateMachine.slideState.isInCooldown)
                        break;
                    machine.EnterState(character.stateMachine.slideState);
                    return;
            }

            if (character.attackAction.WasPerformedThisFrame())
            {
                if (nextAttackState is ICrouchState && !character.crouchAction.IsPressed() && character.canStand)
                    machine.EnterState(character.stateMachine.firstAttackState);
                else if (nextAttackState is not ICrouchState && character.crouchAction.IsPressed())
                    machine.EnterState(character.stateMachine.crouchAttackState);
                else
                    machine.EnterState(nextAttackState);
            }
            else if (_elapsedTime > attackData.duration)
                character.stateMachine.EnterDefaultState();
        }

        public override void Exit()
        {
            if (character.horizontalMove != 0)
                character.FlipTo((int)Mathf.Sign(character.horizontalMove));
        }

        public static int StepAttack(float attackComboMaxDelay)
        {
            lastStandAttack++;
            lastAttackTime = Time.time;

            if (lastStandAttack > 2 || Time.time - lastAttackTime >= attackComboMaxDelay)
                lastStandAttack = 1;

            return lastStandAttack;
        }
    }

    public class KnightCrouchAttackState : KnightAttackState, ICrouchState
    {
        public KnightCrouchAttackState(KnightStateMachine machine)
            : base(machine, machine.character.data.crouchAttack, KnightCharacterController.CrouchAttackAnimHash, machine.character.data.crouchColliderBounds) { }
    }

    public class KnightHurtState : KnightStateBase
    {
        private float _elapsedTime;

        public EntityHitData hitData { get; set; }

        public KnightHurtState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.HurtAnimHash);

            character.rb.linearVelocity = Vector2.zero;
            character.rb.AddForce(hitData.knockbackForce, ForceMode2D.Impulse);
        }

        public override void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.hurtTime)
                character.stateMachine.EnterDefaultState();
        }
    }

    public class KnightDieState : KnightStateBase
    {
        public KnightDieState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            character.SwitchAnimation(KnightCharacterController.DieAnimHash, true);
            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.rb.linearVelocity = Vector2.zero;
            character.data.onDieChannel.Raise(character);
        }
    }

    public class KnightFakeWalkState : KnightStateBase
    {
        private float _elapsedTime;

        public float currentWalkDuration { get; set; }

        public KnightFakeWalkState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState)
        {
            _elapsedTime = 0;
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Update()
        {
            if (character.stateMachine.EnterFallState())
                return;

            if (_elapsedTime > currentWalkDuration)
                character.stateMachine.EnterDefaultState();
            else
                character.rb.linearVelocity = new Vector2(character.facingDirection * character.data.moveSpeed, character.rb.linearVelocity.y);
        }
    }
}
