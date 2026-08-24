using KimLIb.AnimatorSystems;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Players;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.DamageSystems
{
    public class PlayerDamageModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private PlayerController _player;
        private PlayerSkillModule _skillModule;
        private PlayerWeaponController _weaponController;
        private AgentTrigger _agentTrigger;

        public void Initialize(ModuleOwner owner)
        {
            _player = owner as PlayerController;
            _skillModule = owner.GetModule<PlayerSkillModule>();
            _weaponController = owner.GetModule<PlayerWeaponController>();
            _agentTrigger = owner.GetModule<AgentTrigger>();

            Debug.Assert(_player != null, "PlayerDamageModule is player-only.");
            Debug.Assert(_skillModule != null, "PlayerDamageModule requires PlayerSkillModule.");
            Debug.Assert(_weaponController != null, "PlayerDamageModule requires PlayerWeaponController.");
            Debug.Assert(_agentTrigger != null, "PlayerDamageModule requires AgentTrigger.");
        }

        public void AfterInit()
        {
            if (_agentTrigger != null)
                _agentTrigger.OnDamageCastTrigger += HandleDamageCastTrigger;
        }

        private void HandleDamageCastTrigger()
        {
            ISkill currentSkill = _skillModule?.CurrentSkill;
            WeaponDataSO currentWeapon = _weaponController?.CurrentWeaponData;

            if (currentSkill is not { IsUsing: true } || currentWeapon == null)
                return;

            _weaponController.CurrentDamageCaster?.CastAndApply(
                currentWeapon,
                currentSkill.SkillData);
        }

        private void OnDestroy()
        {
            if (_agentTrigger != null)
                _agentTrigger.OnDamageCastTrigger -= HandleDamageCastTrigger;
        }
    }
}
