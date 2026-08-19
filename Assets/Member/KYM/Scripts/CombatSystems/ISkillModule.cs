using System;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems
{
    public interface ISkillModule
    {
        ModuleOwner Owner { get; }

        event Action OnCurrentSkillEnd;
        bool CanUseSkill(int skillIndex, GameObject target = null); 
        void UseSkill(int skillIndex, GameObject target = null);
        void InvokeSkillEnd();
        void StopSkillIfNotFinished(); //스킬이 종료되지 않았다다면 종료시키는 메서드
    }
}