using KimLIb.ModuleSystems;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.DamageSystems
{
    public readonly struct DamageData
    {
        public ModuleOwner Attacker { get; }
        public WeaponDataSO WeaponData { get; }
        public SkillDataSO SkillData { get; }
        public float DamageAmount { get; }
        public float StaggerDamage { get; }
        public float KnockbackForce { get; }
        public Vector3 HitPoint { get; }
        public Vector3 HitNormal { get; }

        public DamageData(
            ModuleOwner attacker,
            WeaponDataSO weaponData,
            SkillDataSO skillData,
            float damageAmount,
            float staggerDamage,
            float knockbackForce,
            Vector3 hitPoint,
            Vector3 hitNormal)
        {
            Attacker = attacker;
            WeaponData = weaponData;
            SkillData = skillData;
            DamageAmount = Mathf.Max(0f, damageAmount);
            StaggerDamage = Mathf.Max(0f, staggerDamage);
            KnockbackForce = Mathf.Max(0f, knockbackForce);
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }

    public readonly struct DamageCastResult
    {
        public IDamageable Damageable { get; }
        public Collider HitCollider { get; }
        public DamageData DamageData { get; }

        public DamageCastResult(
            IDamageable damageable,
            Collider hitCollider,
            DamageData damageData)
        {
            Damageable = damageable;
            HitCollider = hitCollider;
            DamageData = damageData;
        }
    }
}
