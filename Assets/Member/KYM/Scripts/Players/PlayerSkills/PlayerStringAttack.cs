using System;
using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.CombatSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.PlayerSkills
{
    public class PlayerStringAttack : AbstractPlayerSkill
    {
        [SerializeField] private AnimParamSO skillParam;
        private AgentTrigger _trigger;

        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _trigger = _player.GetModule<AgentTrigger>();
            Debug.Assert(_trigger != null, "플레이어 스트롱 스킬은 에이전트 트리거가 필요함!!");
        }
        
        public override void UseSkill(GameObject target = null)
        {
            base.UseSkill(target);

            _mover.CanManualMove = false;
            _mover.RotateTo(_player.UIInput.GetHorizontalCameraForward());
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
            return NormalizedCooldown >= 1f && !IsUsing;
        }
    }
}