using System;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.DamageSystems
{
    public enum OverlapCastType
    {
        Sphere,
        Box
    }

    public class OverlapDamageCaster : AbstractDamageCaster
    {
        [Header("Cast Shape")]
        [SerializeField] private OverlapCastType castType = OverlapCastType.Box;
        [SerializeField, Min(0f)] private float radius = 0.5f;
        [SerializeField] private Vector3 boxSize = Vector3.one;

        [Header("Cast Buffer")]
        [SerializeField, Min(1)] private int maxHitCount = 32;

        [Header("Debug")]
        [SerializeField] private bool isDebug = true;

        private Collider[] _hitResults = Array.Empty<Collider>();

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            InitializeBuffers();
        }

        public override int CastTargets(
            Vector3 position,
            Vector3 direction,
            WeaponDataSO weaponData,
            SkillDataSO skillData)
        {
            EnsureBuffers();
            ClearResults();

            if (CasterOwner == null || weaponData == null || skillData == null)
                return 0;

            int hitCount = OverlapTargets(position);
            float damageAmount = weaponData.AttackPower
                                 * Mathf.Max(0f, skillData.damageMultiplier);

            for (int index = 0; index < hitCount; index++)
            {
                Collider hitCollider = _hitResults[index];

                if (hitCollider == null)
                    continue;

                ModuleOwner targetOwner =
                    hitCollider.GetComponentInParent<ModuleOwner>();
                IDamageable damageable = targetOwner != null
                    ? targetOwner.GetModule<IDamageable>()
                    : hitCollider.GetComponentInParent<IDamageable>();

                if (damageable == null || targetOwner == CasterOwner)
                    continue;

                Vector3 hitPoint = hitCollider.ClosestPoint(position);
                Vector3 hitNormal = hitPoint - position;

                if (hitNormal.sqrMagnitude > 0.001f)
                    hitNormal.Normalize();

                DamageData damageData = new(
                    CasterOwner,
                    weaponData,
                    skillData,
                    damageAmount,
                    skillData.staggerDamage,
                    skillData.knockbackForce,
                    hitPoint,
                    hitNormal);

                TryAddCastResult(new DamageCastResult(
                    damageable,
                    hitCollider,
                    damageData));
            }

            return CastResultCount;
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0f, value);
        }

        public void SetBoxSize(Vector3 value)
        {
            boxSize = Vector3.Max(Vector3.zero, value);
        }

        private int OverlapTargets(Vector3 position)
        {
            return castType switch
            {
                OverlapCastType.Sphere => Physics.OverlapSphereNonAlloc(
                    position,
                    Mathf.Max(0f, radius),
                    _hitResults,
                    targetLayers,
                    QueryTriggerInteraction.Collide),

                OverlapCastType.Box => Physics.OverlapBoxNonAlloc(
                    position,
                    Vector3.Max(Vector3.zero, boxSize) * 0.5f,
                    _hitResults,
                    transform.rotation,
                    targetLayers,
                    QueryTriggerInteraction.Collide),

                _ => 0
            };
        }

        private void InitializeBuffers()
        {
            int bufferSize = Mathf.Max(1, maxHitCount);
            _hitResults = new Collider[bufferSize];
            InitializeCastResults(bufferSize);
        }

        private void EnsureBuffers()
        {
            int bufferSize = Mathf.Max(1, maxHitCount);

            if (_hitResults.Length != bufferSize
                || _castResults.Length != bufferSize)
            {
                InitializeBuffers();
            }
        }

        private void ClearResults()
        {
            ClearCastResults();
            Array.Clear(_hitResults, 0, _hitResults.Length);
        }

        private void OnDrawGizmos()
        {
            if (!isDebug)
                return;

            Gizmos.color = Color.red;

            switch (castType)
            {
                case OverlapCastType.Sphere:
                    Gizmos.DrawWireSphere(
                        transform.position,
                        Mathf.Max(0f, radius));
                    break;

                case OverlapCastType.Box:
                    Matrix4x4 previousMatrix = Gizmos.matrix;

                    Gizmos.matrix = Matrix4x4.TRS(
                        transform.position,
                        transform.rotation,
                        Vector3.one);

                    Gizmos.DrawWireCube(Vector3.zero, boxSize);
                    Gizmos.matrix = previousMatrix;
                    break;
            }
        }
    }
}
