using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerRunState : AbstractPlayerMoveState
    {
        private Vector2 _currentMoveInput;
        
        public PlayerRunState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }
        
        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _currentMoveInput = _player.PlayerInput.CurrentMovement;
            _player.PlayerInput.OnMovementChange += HandleMovementChange;
            _player.PlayerInput.OnSpaceBarPressed += HandleSpaceBarReleased;
        }

        private void HandleSpaceBarReleased(bool value)
        {
            if (!value)
                _player.ChangeState(PlayerStateEnum.IDLE);
        }

        private void HandleMovementChange(Vector2 movementKey)
        {
            _currentMoveInput = movementKey;
        }

        public override void Update()
        {
            base.Update();

            if (_currentMoveInput.magnitude < INPUT_DEADLINE)
            {
                _mover.SetMovementDir(Vector2.zero);
                _player.ChangeState(PlayerStateEnum.IDLE, 0.2f);
                return;
            }

            Vector3 cameraForward = _player.UIInput.MainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = _player.UIInput.MainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            Vector3 semiDir = (_currentMoveInput.x * cameraRight) + (_currentMoveInput.y * cameraForward);
            Vector2 realMoveInput = new Vector2(semiDir.x, semiDir.z);

            _mover.SetMovementDir(realMoveInput);
        }
        
        public override void Exit()
        {
            _player.PlayerInput.OnMovementChange -= HandleMovementChange;
            _player.PlayerInput.OnSpaceBarPressed -= HandleSpaceBarReleased;
            base.Exit();
        }
    }
}
