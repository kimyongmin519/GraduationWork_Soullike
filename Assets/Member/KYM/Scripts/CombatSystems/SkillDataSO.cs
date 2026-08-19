using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems
{
    [CreateAssetMenu(fileName = "Skill data", menuName = "KimSO/Combat/Skill data", order = 25)]
    public class SkillDataSO : ScriptableObject
    {
        public int skillIndex;
        public string skillName;
        public float cooldown;
        public float skillRange = 1f;
        public float damageMultiplier = 1f;
    }
}