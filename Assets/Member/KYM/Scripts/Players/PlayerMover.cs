using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Agents;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerMover : MonoBehaviour, IMover, IModule
    {
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float groundedGravity = -0.1f;
        [SerializeField] private float maxFallSpeed = -30f;
        
        private ModuleOwner _owner;
        private CharacterController _characterController;
        private Vector3 _movementDirection;
        public bool CanManualMove { get; set; } = true;
        public bool UseRootMotion { get; set; } = true;
        public bool UseGravity { get; set; } = true;
        public bool IgnoreRootMotionCollision { get; set; }
        private Vector3 _velocity;
        public float VerticalVelocity => _verticalVelocity;
        private float _verticalVelocity;
        private Vector3 _autoVelocity;
        private bool _isGround;

        public bool IsGround => _isGround;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            Debug.Assert(_owner != null, "플레이어 무버는 플레이어 전용입니다!! : 오너가 플레이어가 아닙니다.");
            _characterController = _owner.GetComponent<CharacterController>();
            _isGround = _characterController.isGrounded;
        }

        public void SetAutoVelocity(Vector3 velocity)
        {
            
        }

        public void SetMovementDir(Vector2 movementInput)
        {
            Vector3 newMovement = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            _movementDirection = newMovement;
        }

        public void ApplyRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            if (!UseRootMotion)
                return;

            if (IgnoreRootMotionCollision)
                _owner.transform.position += deltaPosition;
            else
                _characterController.Move(deltaPosition);

            if (deltaRotation != Quaternion.identity)
            {
                _owner.transform.rotation *= deltaRotation;
            }
        }


        public void SetCurrentSpeed(float speed)
        {
            
        }

        public void AddForceToMover(Vector3 force)
        {
            
        }

        private void FixedUpdate()
        {
            CalculateMovement();
        }

        private void Update()
        {
            ApplyGravity();
        }

        private void CalculateMovement()
        {
            if(CanManualMove)
                _velocity = _movementDirection;
            else 
                _velocity = _autoVelocity;

            _velocity.y = 0;
            _velocity *= rotateSpeed * Time.fixedDeltaTime;
            if (_velocity.sqrMagnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                _owner.transform.rotation = Quaternion.Lerp(_owner.transform.rotation,
                    targetRotation, rotateSpeed * Time.fixedDeltaTime);
            }
        }
        public void RotateTo(Vector3 direction)
        {
            if (direction.magnitude < Mathf.Epsilon) return;
            direction.y = 0;
            _owner.transform.forward = direction.normalized;
        }
        
        private void ApplyGravity()
        {
            if (!UseGravity)
            {
                _verticalVelocity = 0f;
                return;
            }

            if (IsGround && _verticalVelocity < 0f)
                _verticalVelocity = groundedGravity;
            else
                _verticalVelocity = Mathf.Max(
                    _verticalVelocity + gravity * Time.deltaTime,
                    maxFallSpeed);

            CollisionFlags collisionFlags = _characterController.Move(
                Vector3.up * (_verticalVelocity * Time.deltaTime));

            _isGround = (collisionFlags & CollisionFlags.Below) != 0;
        }

        public void StopImmediately(bool stopX, bool stopY, bool stopZ)
        {
            
        }
    }
}
