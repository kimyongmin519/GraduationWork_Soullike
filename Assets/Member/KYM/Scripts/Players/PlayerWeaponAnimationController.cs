using System.Collections.Generic;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.CombatSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerWeaponAnimationController : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private WeaponAnimationProfileSO[] animationProfiles;

        public WeaponType CurrentWeaponType { get; private set; } = WeaponType.None;

        private PlayerWeaponController _weaponController;
        private Animator _animator;
        private RuntimeAnimatorController _baseController;
        private AnimatorOverrideController _runtimeOverrideController;
        private readonly List<AnimationClip> _runtimeClips = new();

        public void Initialize(ModuleOwner owner)
        {
            _weaponController = owner.GetModule<PlayerWeaponController>();
            AgentRenderer playerRenderer = owner.GetModule<AgentRenderer>();

            Debug.Assert(
                _weaponController != null,
                "PlayerWeaponAnimationController requires PlayerWeaponController.");
            Debug.Assert(
                playerRenderer != null,
                "PlayerWeaponAnimationController requires an AgentRenderer.");

            if (playerRenderer == null)
                return;

            _animator = playerRenderer.Animator != null
                ? playerRenderer.Animator
                : playerRenderer.GetComponent<Animator>();

            Debug.Assert(
                _animator != null,
                "PlayerWeaponAnimationController requires an Animator.");

            if (_animator == null)
                return;

            _baseController = GetBaseController(_animator.runtimeAnimatorController);
        }

        public void AfterInit()
        {
            if (_weaponController == null)
                return;

            _weaponController.OnWeaponChanged += HandleWeaponChanged;

            if (_weaponController.CurrentWeaponData != null)
                HandleWeaponChanged(_weaponController.CurrentWeaponData);
        }

        private void OnDestroy()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponChanged -= HandleWeaponChanged;

            ReleaseRuntimeOverrides();
        }

        private void HandleWeaponChanged(WeaponDataSO weaponData)
        {
            if (_animator == null || _baseController == null || weaponData == null)
                return;

            if (CurrentWeaponType == weaponData.WeaponType)
                return;

            WeaponAnimationProfileSO profile = FindProfile(weaponData.WeaponType);
            ApplyProfile(profile, weaponData.WeaponType);
        }

        private WeaponAnimationProfileSO FindProfile(WeaponType weaponType)
        {
            if (animationProfiles == null)
                return null;

            foreach (WeaponAnimationProfileSO profile in animationProfiles)
            {
                if (profile != null && profile.WeaponType == weaponType)
                    return profile;
            }

            return null;
        }

        private void ApplyProfile(
            WeaponAnimationProfileSO profile,
            WeaponType weaponType)
        {
            AnimatorOverrideController nextController = null;
            List<AnimationClip> nextRuntimeClips = new();

            if (profile != null
                && profile.ClipOverrides != null
                && profile.ClipOverrides.Length > 0)
            {
                nextController = CreateOverrideController(profile, nextRuntimeClips);
            }

            _animator.runtimeAnimatorController = nextController != null
                ? nextController
                : _baseController;

            ReleaseRuntimeOverrides();
            _runtimeOverrideController = nextController;
            _runtimeClips.AddRange(nextRuntimeClips);
            CurrentWeaponType = weaponType;

            if (profile == null)
            {
                Debug.LogWarning(
                    $"No weapon animation profile for {weaponType}. " +
                    "The base animator clips will be used.",
                    this);
            }
        }

        private AnimatorOverrideController CreateOverrideController(
            WeaponAnimationProfileSO profile,
            List<AnimationClip> runtimeClips)
        {
            AnimatorOverrideController controller =
                new AnimatorOverrideController(_baseController);
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new();
            controller.GetOverrides(overrides);

            foreach (WeaponAnimationClipOverride clipOverride in profile.ClipOverrides)
            {
                if (!clipOverride.IsValid)
                    continue;

                int overrideIndex = FindOverrideIndex(
                    overrides,
                    clipOverride.OriginalClip);

                if (overrideIndex < 0)
                {
                    Debug.LogWarning(
                        $"{clipOverride.OriginalClip.name} is not used by the base " +
                        "animator controller.",
                        profile);
                    continue;
                }

                AnimationClip runtimeClip = Instantiate(clipOverride.OverrideClip);
                runtimeClip.name = clipOverride.OverrideClip.name + " (Runtime Override)";
                runtimeClip.events = clipOverride.OriginalClip.events;
                runtimeClips.Add(runtimeClip);

                overrides[overrideIndex] = new KeyValuePair<AnimationClip, AnimationClip>(
                    clipOverride.OriginalClip,
                    runtimeClip);
            }

            controller.ApplyOverrides(overrides);
            return controller;
        }

        private static int FindOverrideIndex(
            IReadOnlyList<KeyValuePair<AnimationClip, AnimationClip>> overrides,
            AnimationClip originalClip)
        {
            for (int index = 0; index < overrides.Count; index++)
            {
                if (overrides[index].Key == originalClip)
                    return index;
            }

            return -1;
        }

        private static RuntimeAnimatorController GetBaseController(
            RuntimeAnimatorController controller)
        {
            while (controller is AnimatorOverrideController overrideController)
                controller = overrideController.runtimeAnimatorController;

            return controller;
        }

        private void ReleaseRuntimeOverrides()
        {
            if (_runtimeOverrideController != null)
                Destroy(_runtimeOverrideController);

            foreach (AnimationClip runtimeClip in _runtimeClips)
            {
                if (runtimeClip != null)
                    Destroy(runtimeClip);
            }

            _runtimeOverrideController = null;
            _runtimeClips.Clear();
        }
    }
}
