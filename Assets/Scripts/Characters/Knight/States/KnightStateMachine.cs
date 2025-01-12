namespace Metroidvania.Characters.Knight
{
    public class KnightStateMachine
    {
        public readonly KnightCharacterController character;

        public KnightStateBase currentState { get; private set; }

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

        public KnightStateMachine(KnightCharacterController character)
        {
            this.character = character;
            EnterState(new KnightValidationState(this));

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

        public void Update()
        {
            currentState.Update();
            currentState.Transition();
        }

        public void PhysicsUpdate()
        {
            currentState.PhysicsUpdate();
        }

        public void EnterState(KnightStateBase state)
        {
            KnightStateBase previousState = currentState;
            currentState = state;
            previousState?.Exit();
            state.Enter(previousState);
        }

        public void EnterDefaultState()
        {
            if (!character.collisionChecker.isGrounded)
            {
                EnterState(fallState);
            }
            else if (character.horizontalMove == 0)
            {
                EnterState(idleState);
            }
            else
            {
                EnterState(runState);
            }
        }

        public bool TryEnterAttackState()
        {
            if (character.attackAction.WasPerformedThisFrame() && character.collisionChecker.isGrounded)
            {
                EnterState(KnightAttackState.StepAttack(character.data.attackComboMaxDelay) == 1 ? firstAttackState : secondAttackState);
                return true;
            }
            return false;
        }
    }
}
