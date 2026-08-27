using System;
using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.CombatSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.PlayerSkills
{
    public class PlayerStringAttack : AbstractPlayerSkill
    {
        [SerializeField] private AnimParamSO skillParam;
        [SerializeField, Min(0f)] private float staminaCost = 30f;
        private AgentTrigger _trigger;
        private StaminaModule _staminaModule;

        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _trigger = _player.GetModule<AgentTrigger>();
            _staminaModule = _player.GetModule<StaminaModule>();
            Debug.Assert(_trigger != null, "플레이어 스트롱 스킬은 에이전트 트리거가 필요함!!");
            Debug.Assert(_staminaModule != null, "플레이어 스트롱 스킬은 StaminaModule이 필요합니다.");
        }
        
        public override void UseSkill(GameObject target = null)
        {
            if (_staminaModule != null && !_staminaModule.TryConsume(staminaCost))
                return;

            base.UseSkill(target);

            _mover.CanManualMove = false;
            _renderer.PlayClip(skillParam.ParamHash);

            _trigger.OnAnimationEnd += HandleAnimationEnd;
        }

        private void HandleAnimationEnd()
        {
            StopSkill();
        }

        public override void StopSkill()
        {
            base.StopSkill();
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            _mover.CanManualMove = true;
        }

        public override bool CanUseSkill(GameObject target = null)
        {
            return NormalizedCooldown >= 1f
                   && !IsUsing
                   && (_staminaModule == null || _staminaModule.CanConsume(staminaCost));
        }
    }
}
