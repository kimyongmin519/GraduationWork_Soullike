using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.KYM.Scripts.CoreSystems
{
    [CreateAssetMenu(fileName = "UI input", menuName = "KimSO/Core/UI Input", order = 0)]
    public class UIInputSO : ScriptableObject, Controls.IUIActions
    {
        private Controls _controls;
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera == null ? _mainCamera = Camera.main : _mainCamera;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.UI.SetCallbacks(this);
            }
            _controls.UI.Enable();
        }

        private void OnDisable()
        {
            _controls.UI.Disable();
        }
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
            
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
        }
        
        public Vector3 GetHorizontalCameraForward()
        {
            Vector3 forward = MainCamera.transform.forward.normalized;
            forward.y = 0;
            return forward;
        }
    }
}