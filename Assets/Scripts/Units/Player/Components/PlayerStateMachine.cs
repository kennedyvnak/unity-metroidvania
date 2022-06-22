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
        public readonly PlayerDiedState deathState;
        public readonly PlayerHurtState hurtState;
        public readonly PlayerWallSlideState wallSlideState;
        public readonly PlayerWallJumpState wallJumpState;
        public readonly PlayerAttackState attackOneState;
        public readonly PlayerAttackState attackTwoState;

        public Coroutine crouchTransitionCoroutine;

        public bool isCrouching { get; private set; }
        public bool isInvincible { get; private set; }

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
            wallJumpState = new PlayerWallJumpState(this);
            hurtState = new PlayerHurtState(this);
            deathState = new PlayerDiedState(this);

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
            PlayerStateBase oldState = currentState;
            oldState?.Exit();
            currentState = state;

            isCrouching = PlayerStatesUtility.IsCrouchState(state);
            isInvincible = state.metadatas.TryGetMetadata<PlayerInvincibilityModule>(out PlayerInvincibilityModule _);

            currentState.Enter(oldState);
        }

        // Shortcut methods for states, return null if cannot enter on state else return the entered state
        public PlayerStateBase EnterJumpState()
        {
            bool jumpPressed = Time.time - player.input.lastJumpInputTime < player.data.jumpInputDelay;
            return !jumpPressed || !player.collisions.canStand || !player.collisions.isGrounded ? null : (PlayerStateBase)jumpState.SetActive();
        }

        public PlayerStateBase EnterCrouchState()
        {
            return (!player.input.crouchAction.IsPressed() && player.collisions.canStand) || !player.collisions.isGrounded
                ? null
                : player.input.horizontalMove != 0 ? crouchWalkState.SetActive() : crouchState.SetActive();
        }

        public PlayerStateBase EnterFallState()
        {
            return player.collisions.isGrounded ? null : (PlayerStateBase)fallSate.SetActive();
        }

        public PlayerStateBase EnterIdleState()
        {
            PlayerStateBase enteredFall = EnterFallState();
            if (enteredFall != null) return enteredFall;

            PlayerStateBase enteredCrouch = EnterCrouchState();
            if (enteredCrouch != null) return enteredCrouch;

            PlayerStateBase enteredWallState = EnterWallState();
            return enteredWallState ?? (player.input.horizontalMove == 0 ? idleState.SetActive() : runState.SetActive());
        }

        public PlayerStateBase EnterSlideState()
        {
            return !player.collisions.isGrounded || !isCrouching || !player.input.dashAction.WasPerformedThisFrame() || slideState.isInCooldown
                ? null
                : (PlayerStateBase)slideState.SetActive();
        }

        public PlayerStateBase EnterWallState()
        {
            return player.collisions.isGrounded
                ? null
                : player.collisions.isTouchingWall && player.input.horizontalMove == player.facingDirection ? wallSlideState.SetActive() : (PlayerStateBase)null;
        }

        public PlayerStateBase EnterRollState()
        {
            return !player.collisions.isGrounded || isCrouching || !player.input.dashAction.WasPerformedThisFrame() || rollState.isInCooldown
                ? null
                : (PlayerStateBase)rollState.SetActive();
        }

        public PlayerAttackState EnterAttackState()
        {
            return !player.input.attackAction.WasPerformedThisFrame() || !player.collisions.isGrounded
                ? null
                : isCrouching ? crouchAttackState.SetActive() : attackOneState.SetActive();
        }

        public PlayerStateBase EnterHurt(Vector2 entityHitDirection)
        {
            hurtState.knockbackForce = entityHitDirection;
            return hurtState.SetActive();
        }
    }
}