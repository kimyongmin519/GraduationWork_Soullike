using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players.FSM.Interface;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerIdleState : AbstractPlayerState, ICanAttackState, ICanDodgeState, ICanModeChangeState, ICanFallState
    {
        private Vector2 _movement;
        public PlayerIdleState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
        }
        
        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _movement = _player.PlayerInput.CurrentMovement;
            _player.PlayerInput.OnMovementChange += HandleMovementChange;
            _mover.SetMovementDir(Vector2.zero);
        }
        private void HandleMovementChange(Vector2 movementKey) => _movement = movementKey;

        public override void Update()
        {
            base.Update();
            if (_movement.magnitude > INPUT_DEADLINE)
            {
                _player.ChangeState(PlayerStateEnum.WALK, 0.2f);
            }
        }

        public override void Exit()
        {
            _player.PlayerInput.OnMovementChange -= HandleMovementChange;
            base.Exit();
        }

    }
}
