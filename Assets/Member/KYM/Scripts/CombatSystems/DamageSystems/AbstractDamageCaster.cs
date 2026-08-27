using System;
using System.Collections.Generic;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.DamageSystems
{
    public abstract class AbstractDamageCaster : MonoBehaviour
    {
        [SerializeField] protected LayerMask targetLayers = ~0;

        public ModuleOwner CasterOwner { get; private set; }
        public IReadOnlyList<DamageCastResult> CastResults => _castResults;
        public int CastResultCount { get; protected set; }

        public event Action<DamageCastResult> OnDamageApplied;

        protected DamageCastResult[] _castResults =
            Array.Empty<DamageCastResult>();

        public virtual void Initialize(ModuleOwner owner)
        {
            CasterOwner = owner;
        }

        public abstract int CastTargets(
            Vector3 position,
            Vector3 direction,
            WeaponDataSO weaponData,
            SkillDataSO skillData);

        public int CastTargets(
            WeaponDataSO weaponData,
            SkillDataSO skillData)
        {
            return CastTargets(
                transform.position,
                transform.forward,
                weaponData,
                skillData);
        }

        public int CastAndApply(
            WeaponDataSO weaponData,
            SkillDataSO skillData)
        {
            CastTargets(weaponData, skillData);
            ApplyDamageToCastResults();
            return CastResultCount;
        }

        public void ApplyDamageToCastResults()
        {
            for (int index = 0; index < CastResultCount; index++)
            {
                DamageCastResult result = _castResults[index];

                if (result.Damageable == null)
                    continue;

                result.Damageable.ApplyDamage(result.DamageData);
                OnDamageApplied?.Invoke(result);
            }
        }

        public void SetTargetLayer(LayerMask layerMask)
        {
            targetLayers = layerMask;
        }

        protected void InitializeCastResults(int capacity)
        {
            _castResults = new DamageCastResult[Mathf.Max(1, capacity)];
            CastResultCount = 0;
        }

        protected void ClearCastResults()
        {
            CastResultCount = 0;
            Array.Clear(_castResults, 0, _castResults.Length);
        }

        protected bool TryAddCastResult(DamageCastResult result)
        {
            if (CastResultCount >= _castResults.Length)
                return false;

            _castResults[CastResultCount] = result;
            CastResultCount++;
            return true;
        }
    }
}
