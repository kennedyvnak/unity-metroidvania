using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for simulate a state machine</summary>
    public class PlayerStateMachine : PlayerComponent
    {
        // States
        public readonly PlayerAttackOneState attackOneState;
        public readonly PlayerAttackTwoState attackTwoState;
        public readonly PlayerCrouchAttackState crouchAttackState;
        public readonly PlayerCrouchState crouchState;
        public readonly PlayerCrouchWalkState crouchWalkState;
        public readonly PlayerDeathState deathState;
        public readonly PlayerFallState fallSate;
        public readonly PlayerHurtState hurtState;
        public readonly PlayerIdleState idleState;
        public readonly PlayerJumpState jumpState;
        public readonly PlayerRollState rollState;
        public readonly PlayerRunState runState;
        public readonly PlayerSlideState slideState;
        public readonly PlayerWallClimbState wallClimbState;
        public readonly PlayerWallHandState wallHandState;
        public readonly PlayerWallSlideState wallSlideState;

        public PlayerStateMachine(PlayerController target) : base(target)
        {
            idleState = new PlayerIdleState(this);
            runState = new PlayerRunState(this);
            jumpState = new PlayerJumpState(this);
            fallSate = new PlayerFallState(this);
            slideState = new PlayerSlideState(this);
            rollState = new PlayerRollState(this);
            crouchState = new PlayerCrouchState(this);
            crouchWalkState = new PlayerCrouchWalkState(this);
            crouchAttackState = new PlayerCrouchAttackState(this);
            attackOneState = new PlayerAttackOneState(this);
            attackTwoState = new PlayerAttackTwoState(this);
            wallSlideState = new PlayerWallSlideState(this);
            wallClimbState = new PlayerWallClimbState(this);
            wallHandState = new PlayerWallHandState(this);
            hurtState = new PlayerHurtState(this);
            deathState = new PlayerDeathState(this);

            target.LogicUpdated += Update;
            idleState.SetActive();
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
            target.LogicUpdated -= Update;
        }

        /// <summary>Switches the current state</summary>
        public void SwitchState(PlayerStateBase state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
        
        // Shortcut methods for states, return null if cannot enter on state else return the entered state
        public PlayerStateBase EnterJump()
        {
            if (!target.input.virtualJumping || !target.collisions.isGrounded) return null;
            jumpState.SetActive();
            return jumpState;
        }

        public PlayerStateBase EnterCrouchState(bool shouldMakeTransition = true)
        {
            if (!target.input.virtualCrouching || !target.collisions.isGrounded) return null;
            if (target.input.horizontalMove != 0)
            {
                target.stateMachine.crouchWalkState.shouldMakeTransition = shouldMakeTransition;
                crouchWalkState.SetActive();
                return crouchWalkState;
            }

            target.stateMachine.crouchState.shouldMakeTransition = shouldMakeTransition;
            crouchState.SetActive();
            return crouchState;
        }

        public PlayerStateBase EnterFallState()
        {
            if (target.collisions.isGrounded) return null;
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

            if (target.input.horizontalMove == 0)
            {
                idleState.SetActive();
                return idleState;
            }

            runState.SetActive();
            return runState;
        }

        public PlayerStateBase EnterSlideState()
        {
            if (!target.collisions.isGrounded || !target.input.virtualCrouching || !target.input.virtualDashing ||
                slideState.isInCooldown)
                return null;

            slideState.SetActive();
            return slideState;
        }

        public PlayerStateBase EnterWallState()
        {
            if (target.collisions.isTouchingWall && target.input.horizontalMove != 0)
            {
                wallSlideState.SetActive();
                return wallSlideState;
            }

            return null;
        }

        public PlayerStateBase EnterRollState()
        {
            if (!target.collisions.isGrounded || target.input.virtualCrouching || !target.input.virtualDashing ||
                rollState.isInCooldown)
                return null;

            rollState.SetActive();
            return rollState;
        }

        public PlayerAttackStateBase EnterAttackState()
        {
            if (!target.input.virtualAttacking || !target.collisions.isGrounded)
                return null;

            if (target.input.virtualCrouching)
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