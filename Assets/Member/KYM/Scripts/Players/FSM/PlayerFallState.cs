using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerFallState : AbstractPlayerState
    {
        
        public PlayerFallState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Update()
        {
            base.Update();

            if (_mover.IsGround)
            {
                _player.ChangeState(PlayerStateEnum.LAND);
            }
        }
    }
}