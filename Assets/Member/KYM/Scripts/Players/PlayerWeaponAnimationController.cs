using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using Member.KYM.Scripts.Players.FSM;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerWeaponAnimationController : MonoBehaviour, IModule, IAfterInitModule
    {
        private static readonly int IsCombatHash = Animator.StringToHash("IsCombat");

        public WeaponType CurrentWeaponType { get; private set; } = WeaponType.None;
        public int ActiveWeaponLayerIndex { get; private set; } = -1;

        private PlayerController _player;
        private PlayerWeaponController _weaponController;
        private AgentRenderer _playerRenderer;
        private WeaponDataSO _currentWeaponData;

        public void Initialize(ModuleOwner owner)
        {
            _player = owner as PlayerController;
            _weaponController = owner.GetModule<PlayerWeaponController>();
            _playerRenderer = owner.GetModule<PlayerRenderer>();

            Debug.Assert(_player != null, "Weapon animation layers are player-only.");
            Debug.Assert(
                _weaponController != null,
                "PlayerWeaponAnimationController requires PlayerWeaponController.");
            Debug.Assert(
                _playerRenderer != null,
                "PlayerWeaponAnimationController requires AgentRenderer.");
        }

        public void AfterInit()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponChanged += HandleWeaponChanged;

            HandleWeaponChanged(_weaponController?.CurrentWeaponData);
        }

        private void OnDestroy()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponChanged -= HandleWeaponChanged;

            DeactivateActiveLayer();
        }

        private void HandleWeaponChanged(WeaponDataSO weaponData)
        {
            _currentWeaponData = weaponData;
            CurrentWeaponType = weaponData != null
                ? weaponData.WeaponType
                : WeaponType.None;
            _playerRenderer.Animator.SetFloat(
                IsCombatHash,
                weaponData != null ? (float)PlayerCombatModes.COMBAT : (float)PlayerCombatModes.NORMAL);
            RefreshLayerWeights();
        }

        private void RefreshLayerWeights()
        {
            DeactivateActiveLayer();

            if (_currentWeaponData == null)
                return;

            string layerName = _currentWeaponData.LayerName;

            if (string.IsNullOrWhiteSpace(layerName))
            {
                Debug.LogWarning(
                    $"Weapon '{_currentWeaponData.name}' has no animator layer name.",
                    _currentWeaponData);
                return;
            }

            int activeLayerIndex = _playerRenderer.Animator.GetLayerIndex(layerName);

            if (activeLayerIndex <= 0)
            {
                Debug.LogError(
                    $"Weapon animator layer '{layerName}' was not found or points to the Base Layer.",
                    this);
                return;
            }

            _playerRenderer.Animator.SetLayerWeight(activeLayerIndex, 1f);
            ActiveWeaponLayerIndex = activeLayerIndex;
        }

        private void DeactivateActiveLayer()
        {
            if (_playerRenderer == null || _playerRenderer.Animator == null)
            {
                ActiveWeaponLayerIndex = -1;
                return;
            }

            Animator animator = _playerRenderer.Animator;

            if (ActiveWeaponLayerIndex > 0
                && ActiveWeaponLayerIndex < animator.layerCount)
            {
                animator.SetLayerWeight(ActiveWeaponLayerIndex, 0f);
            }

            ActiveWeaponLayerIndex = -1;
        }
    }
}
