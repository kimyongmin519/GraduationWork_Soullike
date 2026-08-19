using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public interface IMover
    {
        bool CanManualMove { get; set; }
        bool UseRootMotion { get; set; }
        void SetAutoVelocity(Vector3 velocity);
        void SetMovementDir(Vector2 moveInput);
        
        void ApplyRootMotion(Vector3 deltaPosition, Quaternion deltaRotation);
        
        void SetCurrentSpeed(float speed);
        void AddForceToMover(Vector3 force);
        public void RotateTo(Vector3 direction);
        void StopImmediately(bool stopX, bool stopY, bool stopZ);
    }
}