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
            if (_player != null)
                return;

            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (player == null)
                return;

            _player = player;
            
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
            _player = null;
        }
    }
}