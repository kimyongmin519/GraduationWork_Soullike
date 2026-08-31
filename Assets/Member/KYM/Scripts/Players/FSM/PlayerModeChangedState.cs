using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerModeChangedState : AbstractPlayerState
    {
        private readonly int isCombatHash = Animator.StringToHash("IsCombat");
        private AgentTrigger _trigger;
        private PlayerWeaponController _weaponController;
        
        public PlayerModeChangedState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            if (_weaponController == null)
                _weaponController = _player.GetModule<PlayerWeaponController>();

            _player.CombatMode = _weaponController?.CurrentWeaponData != null
                ? PlayerCombatModes.COMBAT
                : PlayerCombatModes.NORMAL;
            _renderer.Animator.SetFloat(isCombatHash, (float)_player.CombatMode);

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
