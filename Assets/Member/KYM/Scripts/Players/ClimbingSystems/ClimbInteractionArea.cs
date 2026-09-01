using UnityEngine;

namespace Member.KYM.Scripts.Players.ClimbingSystems
{
    [RequireComponent(typeof(BoxCollider))]
    public class ClimbInteractionArea : MonoBehaviour
    {
        [SerializeField] private ClimbLink climbLink;

        private PlayerController _player;

        private void OnTriggerEnter(Collider other)
        {
            if (_player == null)
                _player = other.GetComponentInParent<PlayerController>();
            
            _player.PlayerInput.OnEKeyPressed -= HandleInteraction;
            _player.PlayerInput.OnEKeyPressed += HandleInteraction;
        }

        private void OnTriggerExit(Collider other)
        {
            UnsubscribeInput();
        }

        private void HandleInteraction()
        {
            if (_player == null)
                return;

            _player.StartClimb(climbLink);
        }

        private void OnDisable()
        {
            UnsubscribeInput();
        }

        private void UnsubscribeInput()
        {
            if (_player == null)
                return;

            _player.PlayerInput.OnEKeyPressed -= HandleInteraction;
        }
    }
}