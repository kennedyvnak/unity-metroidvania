using Metroidvania.Entities;
using UnityEngine;
using KnightStateBase = Metroidvania.Characters.CharacterStateBase<Metroidvania.Characters.Knight.KnightCharacterController>;

namespace Metroidvania.Characters.Knight {
    public interface ICrouchState { }
    public interface IInvincibleState { }

    public class KnightIdleState : KnightStateBase {
        public KnightIdleState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            character.rb.velocity = new Vector2(0, character.rb.velocity.y);
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.IdleAnimHash);
        }

        public override void Update() {
            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterCrouchState() ||
                character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterRollState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove != 0)
                machine.EnterState(character.stateMachine.runState);
        }
    }

    public class KnightRunState : KnightStateBase {
        public KnightRunState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Update() {
            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterCrouchState() ||
                character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterRollState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove == 0)
                machine.EnterState(character.stateMachine.idleState);
            else
                character.SetHorizontalVelocity(character.data.moveSpeed * character.horizontalMove);
        }
    }

    public class KnightJumpState : KnightStateBase {
        private float _elapsedTime;
        private bool _collidedTop;

        public KnightJumpState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;
            _collidedTop = false;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.JumpAnimHash);
            character.particles.jump.Play();
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.jumpDuration || _collidedTop) {
                character.stateMachine.EnterDefaultState();
            } else if (!character.jumpAction.IsPressed()) {
                character.rb.velocity = new Vector2(character.rb.velocity.x, 0.15f);
                character.stateMachine.EnterDefaultState();
            } else {
                float horizontalSpeed = character.horizontalMove * character.data.moveSpeed;

                float jumpProgress = _elapsedTime / character.data.jumpDuration;
                float jumpCurveMultiplier = character.data.jumpCurve.Evaluate(jumpProgress);
                float verticalSpeed = character.data.jumpSpeed * jumpCurveMultiplier;

                character.rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);

                character.FlipByVelocity();
            }
        }

        private void OnCollisionEnter(Collision2D collision) {
            if (collision.GetContact(0).normal.y == -1)
                _collidedTop = true;
        }
    }

    public class KnightFallState : KnightStateBase {
        private float _fallStartPositionY;

        public KnightFallState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.FallAnimHash);
            _fallStartPositionY = character.rb.position.y;
        }

        public override void Update() {
            if (character.isGrounded) {
                if (_fallStartPositionY - character.rb.position.y > character.data.fallParticlesDistance)
                    character.particles.landing.Play();
                character.stateMachine.EnterDefaultState();
            } else if (!character.stateMachine.EnterWallState())
                character.SetHorizontalVelocity(character.data.moveSpeed * character.horizontalMove);
        }
    }

    public class KnightRollState : KnightStateBase, IInvincibleState {
        private float _elapsedTime;
        private float _lastExitTime;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.rollCooldown;

        public KnightRollState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RollAnimHash, true);
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.rollDuration) {
                character.stateMachine.EnterDefaultState();
            } else {
                float curveMultiplier = character.data.rollHorizontalMoveCurve.Evaluate(_elapsedTime / character.data.rollDuration);
                character.rb.velocity = new Vector2(character.data.rollSpeed * curveMultiplier * character.facingDirection, character.rb.velocity.y);
            }
        }
    }

    public class KnightCrouchIdleState : KnightStateBase, ICrouchState {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchIdleState(KnightStateMachine machine) : base(machine) {
        }

        public override void Enter(KnightStateBase previousState) {
            bool shouldMakeTransition = previousState is not ICrouchState;

            _elapsedTime = 0;
            _quittingAnimElapsedTime = 0;
            _inQuittingAnim = false;

            character.rb.velocity = new Vector2(0, character.rb.velocity.y);
            character.SetColliderBounds(character.data.crouchColliderBounds);

            _hasSwappedAnim = !shouldMakeTransition;
            character.SwitchAnimation(shouldMakeTransition
                ? KnightCharacterController.CrouchTransitionAnimHash
                : KnightCharacterController.CrouchIdleAnimHash);
        }

        public override void Update() {
            if (_inQuittingAnim) {
                _quittingAnimElapsedTime += Time.deltaTime;
                if (_quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                    machine.EnterState(character.stateMachine.idleState);
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (!_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime) {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchIdleAnimHash);
            }

            if (character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterSlideState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove != 0)
                machine.EnterState(character.stateMachine.crouchWalkState);
            else if (!character.crouchAction.IsPressed() && character.canStand)
                _inQuittingAnim = true;
        }
    }

    public class KnightCrouchWalkState : KnightStateBase, ICrouchState {
        private float _elapsedTime;
        private bool _inQuittingAnim;
        private float _quittingAnimElapsedTime;
        private bool _hasSwappedAnim;

        public KnightCrouchWalkState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
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

        public override void Update() {
            if (_inQuittingAnim) {
                _quittingAnimElapsedTime += Time.deltaTime;
                if (_quittingAnimElapsedTime >= character.data.crouchTransitionTime)
                    machine.EnterState(character.stateMachine.idleState);
                else
                    character.SetHorizontalVelocity(character.data.crouchWalkSpeed * character.horizontalMove);
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (_inQuittingAnim)
                character.rb.velocity = new Vector2(character.data.crouchWalkSpeed, character.rb.velocity.y);
            else if (!_hasSwappedAnim && _elapsedTime > character.data.crouchTransitionTime) {
                _hasSwappedAnim = true;
                character.SwitchAnimation(KnightCharacterController.CrouchWalkAnimHash);
            }

            if (character.stateMachine.EnterJumpState() ||
                character.stateMachine.EnterFallState() ||
                character.stateMachine.EnterSlideState() ||
                character.stateMachine.EnterAttackState())
                return;

            if (character.horizontalMove == 0)
                machine.EnterState(character.stateMachine.crouchIdleState);
            else if (!character.crouchAction.IsPressed() && character.canStand)
                _inQuittingAnim = true;
            else
                character.SetHorizontalVelocity(character.data.crouchWalkSpeed * character.horizontalMove);
        }
    }

    public class KnightSlideState : KnightStateBase, ICrouchState {
        private float _elapsedTime;
        private float _lastExitTime = int.MinValue;

        private bool _inQuittingAnim;

        public bool isInCooldown => Time.time - _lastExitTime < character.data.slideCooldown;

        public KnightSlideState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;
            _inQuittingAnim = false;

            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.SwitchAnimation(KnightCharacterController.SlideAnimHash, true);
            character.particles.slide.Play();
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (!_inQuittingAnim && _elapsedTime > character.data.slideDuration - character.data.slideTransitionTime) {
                character.SwitchAnimation(KnightCharacterController.SlideEndAnimHash);
                _inQuittingAnim = true;
            }

            if (_elapsedTime > character.data.slideDuration)
                machine.EnterState(character.stateMachine.crouchIdleState);
            else if (!character.stateMachine.EnterFallState()) {
                float slideProgress = _elapsedTime / character.data.slideDuration;
                float curveMultiplier = character.data.slideMoveCurve.Evaluate(slideProgress);
                character.rb.velocity = new Vector2(character.data.slideSpeed * curveMultiplier * character.facingDirection, character.rb.velocity.y);
            }
        }

        public override void Exit() {
            _lastExitTime = Time.time;
            character.particles.slide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public class KnightWallslideState : KnightStateBase {
        public KnightWallslideState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.WallslideAnimHash);
            character.particles.wallslide.Play();
        }

        public override void Update() {
            if (character.jumpAction.WasPerformedThisFrame())
                machine.EnterState(character.stateMachine.walljumpState);
            else if (character.isGrounded || !character.isTouchingWall || character.horizontalMove != character.facingDirection)
                character.stateMachine.EnterDefaultState();
            else
                character.rb.velocity = new Vector2(0, -character.data.wallSlideSpeed);
        }

        public override void Exit() {
            character.particles.wallslide.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public class KnightWalljumpState : KnightStateBase {
        private float _elapsedTime;

        private bool _collidedWithSomething;

        public KnightWalljumpState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
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

            character.rb.velocity = new Vector2(character.data.wallJumpForce.x * character.facingDirection, character.data.wallJumpForce.y);
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.wallJumpDuration || _collidedWithSomething)
                character.stateMachine.EnterDefaultState();
        }

        private void OnCollisionEnter(Collision2D collision) {
            _collidedWithSomething = true;
        }
    }

    public class KnightAttackState : KnightStateBase {
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

        public KnightAttackState(KnightStateMachine machine, KnightData.Attack attackData, int animHash, KnightData.ColliderBounds colliderBounds) : base(machine) {
            this.attackData = attackData;
            this.animHash = animHash;
            this.colliderBounds = colliderBounds;
        }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;
            _triggered = false;
            _currentExitCommand = ExitAttackCommand.None;

            character.SetColliderBounds(colliderBounds);
            character.SwitchAnimation(animHash, true);
            character.rb.velocity = new Vector2(0, character.rb.velocity.y);
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (character.stateMachine.EnterFallState())
                return;

            if (character.dashAction.WasPerformedThisFrame()) {
                _currentExitCommand = character.crouchAction.IsPressed() || !character.canStand ? ExitAttackCommand.Slide : ExitAttackCommand.Roll;
            }

            if (!_triggered && _elapsedTime >= attackData.triggerTime) {
                _triggered = true;
                character.PerformAttack(attackData);
            }

            if (_elapsedTime < attackData.duration - attackData.attackEndOffset)
                return;

            switch (_currentExitCommand) {
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

            if (character.attackAction.WasPerformedThisFrame()) {
                if (nextAttackState is ICrouchState && !character.crouchAction.IsPressed() && character.canStand)
                    machine.EnterState(character.stateMachine.firstAttackState);
                else if (nextAttackState is not ICrouchState && character.crouchAction.IsPressed())
                    machine.EnterState(character.stateMachine.crouchAttackState);
                else
                    machine.EnterState(nextAttackState);
            } else if (_elapsedTime > attackData.duration)
                character.stateMachine.EnterDefaultState();
        }

        public override void Exit() {
            if (character.horizontalMove != 0)
                character.FlipTo((int)Mathf.Sign(character.horizontalMove));
        }

        public static int StepAttack(float attackComboMaxDelay) {
            KnightAttackState.lastStandAttack++;
            KnightAttackState.lastAttackTime = Time.time;

            if (KnightAttackState.lastStandAttack > 2 || Time.time - KnightAttackState.lastAttackTime >= attackComboMaxDelay)
                KnightAttackState.lastStandAttack = 1;

            return KnightAttackState.lastStandAttack;
        }
    }

    public class KnightCrouchAttackState : KnightAttackState, ICrouchState {
        public KnightCrouchAttackState(KnightStateMachine machine)
            : base(machine, machine.character.data.crouchAttack, KnightCharacterController.CrouchAttackAnimHash, machine.character.data.crouchColliderBounds) { }
    }

    public class KnightHurtState : KnightStateBase {
        private float _elapsedTime;

        public EntityHitData hitData { get; set; }

        public KnightHurtState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;

            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.HurtAnimHash);

            character.rb.velocity = Vector2.zero;
            character.rb.AddForce(hitData.knockbackForce, ForceMode2D.Impulse);
        }

        public override void Update() {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > character.data.hurtTime)
                character.stateMachine.EnterDefaultState();
        }
    }

    public class KnightDieState : KnightStateBase {
        public KnightDieState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            character.SwitchAnimation(KnightCharacterController.DieAnimHash, true);
            character.SetColliderBounds(character.data.crouchColliderBounds);
            character.rb.velocity = Vector2.zero;
            character.data.onDieChannel.Raise(character);
        }
    }

    public class KnightFakeWalkState : KnightStateBase {
        private float _elapsedTime;

        public float currentWalkDuration { get; set; }

        public KnightFakeWalkState(KnightStateMachine machine)
            : base(machine) { }

        public override void Enter(KnightStateBase previousState) {
            _elapsedTime = 0;
            character.SetColliderBounds(character.data.standColliderBounds);
            character.SwitchAnimation(KnightCharacterController.RunAnimHash);
        }

        public override void Update() {
            if (character.stateMachine.EnterFallState())
                return;

            if (_elapsedTime > currentWalkDuration)
                character.stateMachine.EnterDefaultState();
            else
                character.rb.velocity = new Vector2(character.facingDirection * character.data.moveSpeed, character.rb.velocity.y);
        }
    }
}