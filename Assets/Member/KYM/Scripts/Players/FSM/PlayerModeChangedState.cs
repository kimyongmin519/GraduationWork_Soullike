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
        private PlayerWeaponHolder _weaponHoler;
        
        public PlayerModeChangedState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            _player.CombatMode = _player.CombatMode == PlayerCombatModes.NORMAL ? PlayerCombatModes.COMBAT : PlayerCombatModes.NORMAL;
            _renderer.Animator.SetFloat(isCombatHash, (float)_player.CombatMode);

            base.Enter(transitionDuration, layerIndex);
            if (_trigger == null)
                _trigger = _player.GetModule<AgentTrigger>();
            if (_weaponHoler == null)
                _weaponHoler = _player.GetModule<PlayerWeaponHolder>();

            _mover.CanManualMove = false;

            _trigger.OnAnimationSpecialTrigger += HandleWeaponTrmSwitch;
            _trigger.OnAnimationEnd += HandleAnimationEnd;
        }

        private void HandleWeaponTrmSwitch()
        {
            switch (_player.CombatMode)
            {
                case PlayerCombatModes.NORMAL:
                    _weaponHoler.AttachToBack();
                    break;
                case PlayerCombatModes.COMBAT:
                    _weaponHoler.AttachToHand();
                    break;
            }
        }

        private void HandleAnimationEnd()
        {
            _player.ChangeState(PlayerStateEnum.IDLE);
        }

        public override void Exit()
        {
            base.Exit();
            _mover.CanManualMove = true;
            _trigger.OnAnimationSpecialTrigger -= HandleWeaponTrmSwitch;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
        }
    }
}
