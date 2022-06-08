using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for simulate a state machine</summary>
    public class PlayerStateMachine : PlayerComponent
    {
        // States
        public readonly PlayerIdleState idleState;
        public readonly PlayerRunState runState;
        public readonly PlayerJumpState jumpState;
        public readonly PlayerFallState fallSate;
        public readonly PlayerCrouchState crouchState;
        public readonly PlayerCrouchWalkState crouchWalkState;
        public readonly PlayerAttackState crouchAttackState;
        public readonly PlayerSlideState slideState;
        public readonly PlayerRollState rollState;
        public readonly PlayerDeathState deathState;
        public readonly PlayerHurtState hurtState;
        public readonly PlayerWallClimbState wallClimbState;
        public readonly PlayerWallHandState wallHandState;
        public readonly PlayerWallSlideState wallSlideState;
        public readonly PlayerAttackState attackOneState;
        public readonly PlayerAttackState attackTwoState;

        public Coroutine crouchTransitionCoroutine;

        public bool isCrouching { get; private set; }

        public PlayerStateMachine(PlayerController player) : base(player)
        {
            idleState = new PlayerIdleState(this);
            runState = new PlayerRunState(this);
            jumpState = new PlayerJumpState(this);
            fallSate = new PlayerFallState(this);
            slideState = new PlayerSlideState(this);
            rollState = new PlayerRollState(this);
            crouchState = new PlayerCrouchState(this);
            crouchWalkState = new PlayerCrouchWalkState(this);
            crouchAttackState = new PlayerAttackState(this, player.data.crouchAttack, PlayerAnimator.CrouchAttackAnimKey, player.data.crouchColliderData);
            attackOneState = new PlayerAttackState(this, player.data.attackOne, PlayerAnimator.AttackOneAnimKey, player.data.standColliderData);
            attackTwoState = new PlayerAttackState(this, player.data.attackTwo, PlayerAnimator.AttackTwoAnimKey, player.data.standColliderData);
            wallSlideState = new PlayerWallSlideState(this);
            wallClimbState = new PlayerWallClimbState(this);
            wallHandState = new PlayerWallHandState(this);
            hurtState = new PlayerHurtState(this);
            deathState = new PlayerDeathState(this);

            attackOneState.nextAttackState = attackTwoState;
            attackTwoState.nextAttackState = attackOneState;
            crouchAttackState.nextAttackState = crouchAttackState;

            new PlayerCrouchMetadata(crouchAttackState);

            new PlayerValidationState(this).SetActive();
            player.LogicUpdated += Update;
        }

        /// <summary>The state that is running</summary>
        public PlayerStateBase currentState { get; private set; }

        private void Update()
        {
            currentState?.LogicUpdate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            player.LogicUpdated -= Update;
        }

        /// <summary>Switches the current state</summary>
        public void SwitchState(PlayerStateBase state)
        {
            var oldState = currentState;
            oldState?.Exit();
            currentState = state;
            isCrouching = PlayerStatesUtility.IsCrouchState(state);
            currentState.Enter(oldState);
        }

        // Shortcut methods for states, return null if cannot enter on state else return the entered state
        public PlayerStateBase EnterJump()
        {
            if (!player.input.virtualJumping || !player.collisions.canStand || !player.collisions.isGrounded) return null;
            jumpState.SetActive();
            return jumpState;
        }

        public PlayerStateBase EnterCrouchState()
        {
            if ((!player.input.virtualCrouching && player.collisions.canStand) || !player.collisions.isGrounded) return null;

            if (player.input.horizontalMove != 0)
            {
                crouchWalkState.SetActive();
                return crouchWalkState;
            }

            crouchState.SetActive();
            return crouchState;
        }

        public PlayerStateBase EnterFallState()
        {
            if (player.collisions.isGrounded) return null;
            fallSate.SetActive();
            return fallSate;
        }

        public PlayerStateBase EnterIdleState()
        {
            var enteredFall = EnterFallState();
            if (enteredFall != null) return enteredFall;

            var enteredCrouch = EnterCrouchState();
            if (enteredCrouch != null) return enteredCrouch;

            var enteredWallState = EnterWallState();
            if (enteredWallState != null) return enteredWallState;

            if (player.input.horizontalMove == 0)
            {
                idleState.SetActive();
                return idleState;
            }

            runState.SetActive();
            return runState;
        }

        public PlayerStateBase EnterSlideState()
        {
            if (!player.collisions.isGrounded || !isCrouching || !player.input.virtualDashing || slideState.isInCooldown)
                return null;

            slideState.SetActive();
            return slideState;
        }

        public PlayerStateBase EnterWallState()
        {
            if (player.collisions.isTouchingWall && player.input.horizontalMove != 0)
            {
                wallSlideState.SetActive();
                return wallSlideState;
            }

            return null;
        }

        public PlayerStateBase EnterRollState()
        {
            if (!player.collisions.isGrounded || isCrouching || !player.input.virtualDashing || rollState.isInCooldown)
                return null;

            rollState.SetActive();
            return rollState;
        }

        public PlayerAttackState EnterAttackState()
        {
            if (!player.input.virtualAttacking || !player.collisions.isGrounded)
                return null;

            if (isCrouching)
            {
                crouchAttackState.SetActive();
                return crouchAttackState;
            }

            attackOneState.SetActive();
            return attackOneState;
        }

        public PlayerStateBase EnterHurt(Vector2 entityHitDirection)
        {
            hurtState.knockbackForce = entityHitDirection;
            hurtState.SetActive();
            return hurtState;
        }
    }
}