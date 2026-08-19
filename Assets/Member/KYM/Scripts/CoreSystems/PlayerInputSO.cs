using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.KYM.Scripts.CoreSystems
{
    [CreateAssetMenu(fileName = "Player input", menuName = "KimSO/Core/Player input", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
        public event Action<bool> OnSpaceBarPressed;
        public bool IsShiftPress { get; private set; }
        public bool IsSpaceBarPress { get; private set; }
        public Vector2 CurrentMovement { get; private set; }
        private Controls _controls;

        public event Action OnAttackKeyPressed;
        public event Action<bool> OnGuardKeyPressed;
        public event Action OnFKeyPressed;
        public event Action OnEKeyPressed;
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            CurrentMovement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(CurrentMovement);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackKeyPressed?.Invoke();
        }

        public void OnGuard(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnGuardKeyPressed?.Invoke(true);
            if (context.canceled)
                OnGuardKeyPressed?.Invoke(false);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnSpaceBar(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpaceBarPressed?.Invoke(true);
                IsSpaceBarPress = true;
            }
            if (context.canceled)
            {
                OnSpaceBarPressed?.Invoke(false);
                IsSpaceBarPress = false;
            }
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            if (context.performed)
                IsShiftPress = true;
            if (context.canceled)
                IsShiftPress = false;
        }

        public void OnFKey(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnFKeyPressed?.Invoke();
        }

        public void OnEKey(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnEKeyPressed?.Invoke();
        }
    }
}
