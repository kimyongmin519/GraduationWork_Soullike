using UnityEngine;

namespace Member.KYM.Scripts.Players.TraversalSystems
{
    [RequireComponent(typeof(BoxCollider))]
    public class JumpInteractionArea : MonoBehaviour
    {
        [SerializeField] private JumpLink jumpLink;

        private PlayerController _player;
        private int _playerColliderCount;

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (player == null)
                return;

            if (_player != null && _player != player)
                return;

            _player = player;
            _playerColliderCount++;

            if (_playerColliderCount > 1)
                return;

            _player.PlayerInput.OnEKeyPressed -= HandleInteraction;
            _player.PlayerInput.OnEKeyPressed += HandleInteraction;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_player == null)
                return;

            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (player != _player)
                return;

            _playerColliderCount = Mathf.Max(0, _playerColliderCount - 1);

            if (_playerColliderCount == 0)
                UnsubscribeInput();
        }

        private void HandleInteraction()
        {
            if (_player != null)
                _player.StartTraversalJump(jumpLink);
        }

        private void OnDisable() => UnsubscribeInput();

        private void UnsubscribeInput()
        {
            if (_player != null && _player.PlayerInput != null)
                _player.PlayerInput.OnEKeyPressed -= HandleInteraction;

            _player = null;
            _playerColliderCount = 0;
        }

        private void Reset()
        {
            BoxCollider interactionCollider = GetComponent<BoxCollider>();
            interactionCollider.isTrigger = true;
        }
    }
}
