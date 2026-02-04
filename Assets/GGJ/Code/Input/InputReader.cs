using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Code.Input
{
    [CreateAssetMenu(menuName = "GGJ/Input/Input Reader")]
    public class InputReader : ScriptableObject, IA_MCInput.IGameplayActions, IA_MCInput.IUIActions
    {
        public Action<Vector2> OnMoveEvent;
        public Action OnInteractEvent;

        IA_MCInput _input;

        void OnEnable()
        {
            if (_input == null)
            {
                _input = new IA_MCInput();
                _input.Gameplay.SetCallbacks(this);
                _input.UI.SetCallbacks(this);
            }

            _input.Gameplay.Enable();
        }

        void OnDisable()
        {
            _input?.Gameplay.Disable();
            _input?.UI.Disable();
        }

        public void EnableGameplay()
        {
            _input.Gameplay.Enable();
            _input.UI.Disable();
        }

        public void DisableGameplay()
        {
            _input.Gameplay.Disable();
            _input.UI.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                OnInteractEvent?.Invoke();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}