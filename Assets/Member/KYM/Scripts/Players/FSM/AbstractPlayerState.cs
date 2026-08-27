using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public abstract class AbstractPlayerState : AgentState
    {
        protected PlayerMover _mover;
        protected PlayerController _player;
        private readonly PlayerAnimationRiggingController _riggingController;
        protected const float INPUT_DEADLINE = 0.1f;
        
        public AbstractPlayerState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            _mover = agent.GetModule<PlayerMover>();
            Debug.Assert(_mover != null, "mover is null");
            _player = agent as PlayerController;
            Debug.Assert(_player != null, "agent is not player");
            _riggingController =
                agent.GetModule<PlayerAnimationRiggingController>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            ResetAnimationRigging();
            base.Enter(transitionDuration, layerIndex);
        }

        protected void ResetAnimationRigging()
        {
            _riggingController?.ResetRigging();
        }
    }
}
