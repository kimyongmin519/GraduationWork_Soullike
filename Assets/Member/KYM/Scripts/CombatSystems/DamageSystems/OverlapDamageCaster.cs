using System;
using System.Collections.Generic;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.DamageSystems
{
    public enum OverlapCastType
    {
        Sphere,
        Box,
        Capsule
    }

    public class OverlapDamageCaster : MonoBehaviour
    {
        [SerializeField] private bool useWeaponRendererBounds = true;
        [SerializeField, Min(0f)] private float rendererBoundsPadding = 0.1f;
        [SerializeField] private OverlapCastType castType = OverlapCastType.Box;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 boxSize = Vector3.one;
        [SerializeField, Min(0f)] private float radius = 0.5f;
        [SerializeField, Min(0f)] private float capsuleHeight = 2f;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField, Min(1)] private int maxHitCount = 32;

        public IReadOnlyList<DamageCastResult> CastResults => _castResults;
        public int CastResultCount => _castResults.Count;

        public event Action<DamageCastResult> OnDamageApplied;

        private readonly List<DamageCastResult> _castResults = new();
        private readonly HashSet<IDamageable> _uniqueTargets = new();
        private ModuleOwner _owner;
        private Collider[] _hitBuffer;
        private Renderer[] _weaponRenderers;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _hitBuffer = new Collider[Mathf.Max(1, maxHitCount)];
            Transform weaponRoot = transform.parent != null
                ? transform.parent
                : transform;
            _weaponRenderers = weaponRoot.GetComponentsInChildren<Renderer>();
        }

        public int CastAndApply(WeaponDataSO weaponData, SkillDataSO skillData)
        {
            int resultCount = CastTargets(weaponData, skillData);

            foreach (DamageCastResult result in _castResults)
            {
                result.Damageable.ApplyDamage(result.DamageData);
                OnDamageApplied?.Invoke(result);
            }

            return resultCount;
        }

        public int CastTargets(WeaponDataSO weaponData, SkillDataSO skillData)
        {
            _castResults.Clear();
            _uniqueTargets.Clear();

            if (_owner == null || weaponData == null || skillData == null)
                return 0;

            EnsureBuffer();

            int hitCount = Overlap(out Vector3 center);
            float damageAmount = weaponData.AttackPower
                                 * Mathf.Max(0f, skillData.damageMultiplier);

            for (int index = 0; index < hitCount; index++)
            {
                Collider hitCollider = _hitBuffer[index];

                if (hitCollider == null)
                    continue;

                ModuleOwner targetOwner =
                    hitCollider.GetComponentInParent<ModuleOwner>();
                IDamageable damageable = targetOwner != null
                    ? targetOwner.GetModule<IDamageable>()
                    : hitCollider.GetComponentInParent<IDamageable>();

                if (damageable == null
                    || targetOwner == _owner
                    || !_uniqueTargets.Add(damageable))
                {
                    continue;
                }

                Vector3 hitPoint = hitCollider.ClosestPoint(center);
                Vector3 hitNormal = hitPoint - center;

                if (hitNormal.sqrMagnitude > 0.001f)
                    hitNormal.Normalize();

                DamageData damageData = new(
                    _owner,
                    weaponData,
                    skillData,
                    damageAmount,
                    skillData.staggerDamage,
                    skillData.knockbackForce,
                    hitPoint,
                    hitNormal);

                _castResults.Add(new DamageCastResult(
                    damageable,
                    hitCollider,
                    damageData));
            }

            return _castResults.Count;
        }

        private int Overlap(out Vector3 center)
        {
            if (useWeaponRendererBounds
                && TryGetWeaponBounds(out Bounds weaponBounds))
            {
                center = weaponBounds.center;
                return Physics.OverlapBoxNonAlloc(
                    center,
                    weaponBounds.extents + Vector3.one * rendererBoundsPadding,
                    _hitBuffer,
                    Quaternion.identity,
                    targetLayers,
                    QueryTriggerInteraction.Collide);
            }

            center = transform.TransformPoint(offset);

            return castType switch
            {
                OverlapCastType.Sphere => Physics.OverlapSphereNonAlloc(
                    center,
                    Mathf.Max(0f, radius),
                    _hitBuffer,
                    targetLayers,
                    QueryTriggerInteraction.Collide),

                OverlapCastType.Box => Physics.OverlapBoxNonAlloc(
                    center,
                    Vector3.Max(Vector3.zero, boxSize) * 0.5f,
                    _hitBuffer,
                    transform.rotation,
                    targetLayers,
                    QueryTriggerInteraction.Collide),

                OverlapCastType.Capsule => OverlapCapsule(center),

                _ => 0
            };
        }

        private int OverlapCapsule(Vector3 center)
        {
            float safeRadius = Mathf.Max(0f, radius);
            float halfLine = Mathf.Max(0f, capsuleHeight * 0.5f - safeRadius);
            Vector3 axis = transform.up * halfLine;

            return Physics.OverlapCapsuleNonAlloc(
                center + axis,
                center - axis,
                safeRadius,
                _hitBuffer,
                targetLayers,
                QueryTriggerInteraction.Collide);
        }

        private bool TryGetWeaponBounds(out Bounds bounds)
        {
            if (_weaponRenderers == null || _weaponRenderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = _weaponRenderers[0].bounds;

            for (int index = 1; index < _weaponRenderers.Length; index++)
                bounds.Encapsulate(_weaponRenderers[index].bounds);

            return true;
        }

        private void EnsureBuffer()
        {
            int size = Mathf.Max(1, maxHitCount);

            if (_hitBuffer == null || _hitBuffer.Length != size)
                _hitBuffer = new Collider[size];
        }
    }
}
