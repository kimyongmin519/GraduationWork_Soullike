using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players.FSM.Interface;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerLandState : AbstractPlayerState, ICanDodgeState
    {
        private AgentTrigger _trigger;
        
        public PlayerLandState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            
            if (_trigger == null)
                _trigger = _player.GetModule<AgentTrigger>();

            _mover.CanManualMove = false;
            
            _trigger.OnAnimationEnd += HandleAnimationEnd;
        }

        private void HandleAnimationEnd()
        {
            _player.ChangeState(PlayerStateEnum.IDLE);
        }

        public override void Exit()
        {
            base.Exit();
            _mover.CanManualMove = true;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
        }
    }
}