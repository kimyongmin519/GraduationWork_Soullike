using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerDodgeState : AbstractPlayerState
    {
        private AgentTrigger _trigger;
        
        public PlayerDodgeState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            if (_trigger == null)
                _trigger = _player.GetModule<AgentTrigger>();
            
            _mover.RotateTo(_player.UIInput.GetHorizontalCameraForward());
            
            base.Enter(transitionDuration, layerIndex);
            _mover.CanManualMove = false;
            _trigger.OnAnimationEnd += HandleAnimationEnd;
        }

        private void HandleAnimationEnd()
        {
            _player.ChangeState(PlayerStateEnum.IDLE);
        }

        public override void Exit()
        {
            _mover.CanManualMove = true;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            base.Exit();
        }
    }
}