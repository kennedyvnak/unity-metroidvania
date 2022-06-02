using System;
using UnityEngine.InputSystem;

namespace Metroidvania.InputSystem
{
    /// <summary>This class handle the unity input actions events</summary>
    /// <see cref="PlayerInput"/>
    public class InputReader : ScriptableSingleton<InputReader>, InputActions.IGameplayActions, InputActions.IMenusActions
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

        public event Action PauseEvent;

        public event Action MenuUnpauseEvent;
        public event Action MenuCloseEvent;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();

                // Use this method to implement callbacks On[Action](InputAction.CallbackContext)
                _inputActions.Gameplay.SetCallbacks(this);
                _inputActions.Menus.SetCallbacks(this);
            }
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        /// <summary>Enable the gameplay input</summary>
        public void EnableGameplayInput()
        {
            _inputActions.Menus.Disable();

            _inputActions.Gameplay.Enable();
        }

        /// <summary>Enable menu input</summary>
        public void EnableMenuInput()
        {
            _inputActions.Gameplay.Disable();

            _inputActions.Menus.Enable();
        }

        /// <summary>Disable All inputs</summary>
        public void DisableAllInput()
        {
            _inputActions.Menus.Disable();
            _inputActions.Gameplay.Disable();
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

        void InputActions.IGameplayActions.OnCrouch(InputAction.CallbackContext context)
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

        void InputActions.IGameplayActions.OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                PauseEvent?.Invoke();
        }


        void InputActions.IMenusActions.OnMoveSelection(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnNavigate(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnSubmit(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnConfirm(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MenuCloseEvent.Invoke();
        }

        void InputActions.IMenusActions.OnUnpause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MenuUnpauseEvent?.Invoke();
        }

        void InputActions.IMenusActions.OnClick(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnPoint(InputAction.CallbackContext context)
        {
        }

        void InputActions.IMenusActions.OnRightClick(InputAction.CallbackContext context)
        {
        }
    }
}