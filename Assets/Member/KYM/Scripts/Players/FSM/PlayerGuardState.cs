using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players.FSM.Interface;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerGuardState : AbstractPlayerState, ICanAttackState, ICanDodgeState, ICanModeChangeState
    {
        public PlayerGuardState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _player.PlayerInput.OnGuardKeyPressed += HandleGuardRelease;

            _mover.CanManualMove = false;
        }

        public override void Exit()
        {
            base.Exit();
            _player.PlayerInput.OnGuardKeyPressed -= HandleGuardRelease;

            _mover.CanManualMove = true;
        }

        private void HandleGuardRelease(bool value)
        {
            if (!value)
                _player.ChangeState(PlayerStateEnum.IDLE);
        }
    }
}