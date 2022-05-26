using System;
using UnityEngine.InputSystem;

namespace Metroidvania.InputSystem
{
    /// <summary>This class handle the unity input actions events</summary>
    /// <see cref="PlayerInput"/>
    [ResourceObjectPath("Data/Input Reader")]
    public class InputReader : ScriptableSingleton<InputReader>, InputActions.IGameplayActions
    {
        private InputActions _inputActions;

        /// <summary>Triggered when the move button is performed or canceled. arg0 is the move horizontal direction</summary>
        public event Action<float> MoveEvent;

        // This actions is: [...]Event: when the input button down; [...]CanceledEvent: when the input button up;

        public event Action AttackEvent;
        public event Action AttackCanceledEvent;

        public event Action CrouchEvent;
        public event Action CrouchCanceledEvent;

        public event Action DashEvent;
        public event Action DashCanceledEvent;

        public event Action JumpEvent;
        public event Action JumpCanceledEvent;

        /// <summary>Enable the gameplay input</summary>
        public void EnableGameplayInput()
        {
            _inputActions.Gameplay.Enable();
        }

        /// <summary>Disable All inputs</summary>
        public void DisableAllInput()
        {
            _inputActions.Gameplay.Disable();
        }

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();
                // Use this method to implement callbacks On[Action](InputAction.CallbackContext)
                _inputActions.Gameplay.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        void InputActions.IGameplayActions.OnMove(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    MoveEvent?.Invoke(context.ReadValue<float>());
                    break;
                case InputActionPhase.Canceled:
                    MoveEvent?.Invoke(0);
                    break;
            }
        }

        void InputActions.IGameplayActions.OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    AttackEvent?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    AttackCanceledEvent?.Invoke();
                    break;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    CrouchEvent?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    CrouchCanceledEvent?.Invoke();
                    break;
            }
        }

        void InputActions.IGameplayActions.OnDash(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    DashEvent?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DashCanceledEvent?.Invoke();
                    break;
            }
        }

        void InputActions.IGameplayActions.OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    JumpEvent?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    JumpCanceledEvent?.Invoke();
                    break;
            }
        }
    }
}