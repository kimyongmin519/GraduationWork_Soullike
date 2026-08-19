using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Players.FSM.Interface;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public abstract class AbstractPlayerMoveState : AbstractPlayerState, ICanAttackState, ICanDodgeState, ICanModeChangeState, ICanFallState
    {
        private readonly static int MoveXHash = Animator.StringToHash("MoveX");
        private readonly static int MoveYHash = Animator.StringToHash("MoveY");
        
        private Vector2 _currentMoveInput;
        
        public AbstractPlayerMoveState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _currentMoveInput = _player.PlayerInput.CurrentMovement;
            _player.PlayerInput.OnMovementChange += HandleMovementChange;
        }

        private void HandleMovementChange(Vector2 movementKey)
        {
            _currentMoveInput = movementKey;
        }

        public override void Update()
        {
            base.Update();

            if (IsDiagonalInput())
            {
                _renderer.Animator.SetFloat(MoveXHash, Mathf.Sign(_currentMoveInput.x));
                _renderer.Animator.SetFloat(MoveYHash, 0);
                //구르기가 4방향 밖에 애니메이션이 없어서 대각선으로 이동할 때 구를 시, 좌우를 우선적으로 타겟팅할려고 y에 0넣음
            }
            else
            {
                _renderer.Animator.SetFloat(MoveYHash, _currentMoveInput.y);
                _renderer.Animator.SetFloat(MoveXHash, _currentMoveInput.x);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _player.PlayerInput.OnMovementChange -= HandleMovementChange;
        }

        private bool IsDiagonalInput()
        {
            return Mathf.Abs(_currentMoveInput.x) > INPUT_DEADLINE
                && Mathf.Abs(_currentMoveInput.y) > INPUT_DEADLINE;
        }
    }
}
