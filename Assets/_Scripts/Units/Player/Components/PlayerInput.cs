using Metroidvania.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.Player
{
    /// <summary>Player component used for handle player inputs</summary>
    public class PlayerInput : PlayerComponent
    {
        public PlayerInput(PlayerController player) : base(player)
        {
            player.Enabled += Enable;
            player.Disabled += Disable;
        }

        public static InputReader reader => InputReader.instance;

        /// <summary>True if is reading reader events</summary>
        public bool enabled { get; private set; }

        /// <summary>Raw horizontal input</summary>
        public float horizontalMove { get; private set; }

        public float lastJumpInputTime;

        public InputAction crouchAction => PlayerInput.reader.inputActions.Gameplay.Crouch;
        public InputAction dashAction => PlayerInput.reader.inputActions.Gameplay.Dash;
        public InputAction attackAction => PlayerInput.reader.inputActions.Gameplay.Attack;
        public InputAction jumpAction => PlayerInput.reader.inputActions.Gameplay.Jump;

        public override void OnDestroy()
        {
            base.OnDestroy();
            player.Enabled -= Enable;
            player.Disabled -= Disable;
        }

        public void Enable()
        {
            if (enabled)
                return;

            reader.MoveEvent += ReadMove;
            reader.JumpEvent += ReadJump;

            enabled = true;
        }

        public void Disable()
        {
            if (!enabled)
                return;

            reader.MoveEvent -= ReadMove;
            reader.JumpEvent -= ReadJump;

            enabled = false;
        }

        // Events handles
        private void ReadMove(float move) => horizontalMove = move;

        private void ReadJump()
        {
            lastJumpInputTime = Time.time;
        }
    }
}