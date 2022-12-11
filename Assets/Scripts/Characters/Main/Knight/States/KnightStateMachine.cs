namespace Metroidvania.Characters.Knight {
    public class KnightStateMachine : CharacterStateMachine<KnightCharacterController> {
        public KnightIdleState idleState;
        public KnightRunState runState;
        public KnightJumpState jumpState;
        public KnightFallState fallState;
        public KnightRollState rollState;
        public KnightCrouchIdleState crouchIdleState;
        public KnightCrouchWalkState crouchWalkState;
        public KnightSlideState slideState;
        public KnightWallslideState wallslideState;
        public KnightWalljumpState walljumpState;
        public KnightAttackState firstAttackState, secondAttackState, crouchAttackState;
        public KnightHurtState hurtState;
        public KnightDieState dieState;
        public KnightFakeWalkState fakeWalkState;

        public bool inCrouchState { get; private set; }
        public bool inInvincibleState { get; private set; }

        public KnightStateMachine(KnightCharacterController character) : base(character) {
            idleState = new KnightIdleState(this);
            runState = new KnightRunState(this);
            jumpState = new KnightJumpState(this);
            fallState = new KnightFallState(this);
            rollState = new KnightRollState(this);
            crouchIdleState = new KnightCrouchIdleState(this);
            crouchWalkState = new KnightCrouchWalkState(this);
            slideState = new KnightSlideState(this);
            wallslideState = new KnightWallslideState(this);
            walljumpState = new KnightWalljumpState(this);
            firstAttackState = new KnightAttackState(this, character.data.firstAttack, KnightCharacterController.FirstAttackAnimHash, character.data.standColliderBounds);
            secondAttackState = new KnightAttackState(this, character.data.secondAttack, KnightCharacterController.SecondAttackAnimHash, character.data.standColliderBounds);
            crouchAttackState = new KnightCrouchAttackState(this);
            hurtState = new KnightHurtState(this);
            dieState = new KnightDieState(this);
            fakeWalkState = new KnightFakeWalkState(this);

            firstAttackState.nextAttackState = secondAttackState;
            secondAttackState.nextAttackState = firstAttackState;

            crouchAttackState.nextAttackState = crouchAttackState;

            EnterState(idleState);
        }


        public override void EnterState(CharacterStateBase<KnightCharacterController> state) {
            base.EnterState(state);

            inCrouchState = state is ICrouchState;
            inInvincibleState = state is IInvincibleState;
        }

        public void EnterDefaultState() {
            if (EnterFallState() || EnterCrouchState() || EnterWallState())
                return;

            EnterState(character.horizontalMove == 0 ? idleState : runState);
        }

        public bool EnterJumpState() {
            if (!character.jumpAction.IsPressed() || !character.canStand || !character.isGrounded)
                return false;

            EnterState(jumpState);
            return true;
        }

        public bool EnterCrouchState() {
            if ((!character.crouchAction.IsPressed() && character.canStand) || !character.isGrounded)
                return false;

            EnterState(character.horizontalMove != 0 ? crouchWalkState : crouchIdleState);
            return true;
        }

        public bool EnterFallState() {
            if (character.isGrounded)
                return false;

            EnterState(fallState);
            return true;
        }

        public bool EnterSlideState() {
            if (!character.isGrounded || !inCrouchState || !character.dashAction.WasPerformedThisFrame() || slideState.isInCooldown)
                return false;

            EnterState(slideState);
            return true;
        }

        public bool EnterRollState() {
            if (!character.isGrounded || inCrouchState || !character.dashAction.WasPerformedThisFrame() || rollState.isInCooldown)
                return false;

            EnterState(rollState);
            return true;
        }

        public bool EnterWallState() {
            if (!character.isGrounded && character.isTouchingWall && character.horizontalMove == character.facingDirection) {
                EnterState(wallslideState);
                return true;
            }
            return false;
        }

        public bool EnterAttackState() {
            if (!character.attackAction.WasPerformedThisFrame() || !character.isGrounded)
                return false;

            if (inCrouchState)
                EnterState(crouchAttackState);
            else
                EnterState(KnightAttackState.StepAttack(character.data.attackComboMaxDelay) == 1 ? firstAttackState : secondAttackState);

            return true;
        }

        public void EnterHurt(Entities.EntityHitData hitData) {
            hurtState.hitData = hitData;
            EnterState(hurtState);
        }

        public void EnterFakeWalk(float duration) {
            fakeWalkState.currentWalkDuration = duration;
            EnterState(fakeWalkState);
        }
    }
}