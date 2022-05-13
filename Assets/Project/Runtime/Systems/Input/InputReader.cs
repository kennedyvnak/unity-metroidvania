using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.InputSystem
{
    [CreateAssetMenu(menuName = "Scriptables/Input/Reader")]
    public class InputReader : ScriptableObject, InputActions.IGameplayActions
    {
        private InputActions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();
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

        public event Action<float> MoveEvent;
        public event Action AttackEvent;
        public event Action AttackCanceledEvent;
        public event Action CrouchEvent;
        public event Action CrouchCanceledEvent;
        public event Action DashEvent;
        public event Action DashCanceledEvent;
        public event Action JumpEvent;
        public event Action JumpCanceledEvent;

        public void EnableGameplayInput()
        {
            _inputActions.Gameplay.Enable();
        }

        public void DisableAllInput()
        {
            _inputActions.Gameplay.Disable();
        }
    }
}