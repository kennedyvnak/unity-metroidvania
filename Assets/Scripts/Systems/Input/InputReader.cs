using System;
using UnityEngine.InputSystem;

namespace Metroidvania.InputSystem {
    /// <summary>This class handle the unity input actions events</summary>
    public class InputReader : ScriptableSingleton<InputReader>, InputActions.IGameplayActions, InputActions.IMenusActions {
        public InputActions inputActions { get; private set; }

        /// <summary>Triggered when the move button is performed or canceled. arg0 is the move horizontal direction</summary>
        public event Action<float> MoveEvent;

        // This actions is: [...]Event: when the input button down; [...]CanceledEvent: when the input button up;

        public event Action JumpEvent;

        public event Action PauseEvent;

        public event Action MenuCloseEvent;

        private void OnEnable() {
            if (inputActions == null) {
                inputActions = new InputActions();

                // Use this method to implement callbacks On[Action](InputAction.CallbackContext)
                inputActions.Gameplay.SetCallbacks(this);
                inputActions.Menus.SetCallbacks(this);
            }
        }

        private void OnDisable() {
            DisableAllInput();
        }

        /// <summary>Enable the gameplay input</summary>
        public void EnableGameplayInput() {
            inputActions.Menus.Disable();

            inputActions.Gameplay.Enable();
        }

        /// <summary>Enable menu input</summary>
        public void EnableMenuInput() {
            inputActions.Gameplay.Disable();

            inputActions.Menus.Enable();
        }

        /// <summary>Disable All inputs</summary>
        public void DisableAllInput() {
            inputActions.Menus.Disable();
            inputActions.Gameplay.Disable();
        }

        void InputActions.IGameplayActions.OnMove(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Performed:
                    float value = context.ReadValue<float>();
                    int valueNormalized = value == 0 ? 0 : MathF.Sign(value);
                    MoveEvent?.Invoke(valueNormalized);
                    break;
                case InputActionPhase.Canceled:
                    MoveEvent?.Invoke(0);
                    break;
            }
        }

        void InputActions.IGameplayActions.OnAttack(InputAction.CallbackContext context) {
        }

        void InputActions.IGameplayActions.OnCrouch(InputAction.CallbackContext context) {
        }

        void InputActions.IGameplayActions.OnDash(InputAction.CallbackContext context) {
        }

        void InputActions.IGameplayActions.OnJump(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Performed)
                JumpEvent?.Invoke();
        }

        void InputActions.IGameplayActions.OnPause(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Performed)
                PauseEvent?.Invoke();
        }


        void InputActions.IMenusActions.OnNavigate(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnSubmit(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnCancel(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Performed)
                MenuCloseEvent?.Invoke();
        }

        void InputActions.IMenusActions.OnClick(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnPoint(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnRightClick(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnScrollWheel(InputAction.CallbackContext context) {
        }

        void InputActions.IMenusActions.OnMiddleClick(InputAction.CallbackContext context) {
        }
    }
}