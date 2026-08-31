using System;
using System.Collections.Generic;
using KimLIb.ModuleSystems;
using Member.KDH._03.Scripts.Item;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public readonly struct EquipmentChangedEvent
    {
        public EquipmentSlotType SlotType { get; }
        public EquipmentItemDataSO PreviousItem { get; }
        public EquipmentItemDataSO CurrentItem { get; }

        public EquipmentChangedEvent(
            EquipmentSlotType slotType,
            EquipmentItemDataSO previousItem,
            EquipmentItemDataSO currentItem)
        {
            SlotType = slotType;
            PreviousItem = previousItem;
            CurrentItem = currentItem;
        }
    }

    public class PlayerEquipmentModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [Header("게임 시작 시 장착할 장비")]
        [SerializeField]
        private EquipmentItemDataSO[] startingEquipment =
            Array.Empty<EquipmentItemDataSO>();

        public event Action<EquipmentChangedEvent> OnEquipmentChanged;

        public IReadOnlyDictionary<EquipmentSlotType, EquipmentItemDataSO>
            EquippedItems => _equippedItems;

        private readonly Dictionary<EquipmentSlotType, EquipmentItemDataSO>
            _equippedItems = new();

        private PlayerWeaponController _weaponController;
        private bool _isApplyingWeaponChange;

        public void Initialize(ModuleOwner owner)
        {
            _weaponController = owner.GetModule<PlayerWeaponController>();

            Debug.Assert(
                _weaponController != null,
                "PlayerEquipmentModule requires PlayerWeaponController.");

            InitializeSlots();

            if (_weaponController != null)
                _weaponController.OnWeaponChanged += HandleWeaponChanged;
        }

        public void AfterInit()
        {
            if (startingEquipment == null)
                return;

            foreach (EquipmentItemDataSO item in startingEquipment)
            {
                if (item == null)
                    continue;

                if (!TryEquip(item, out _))
                {
                    Debug.LogWarning(
                        $"시작 장비 '{item.name}'을 장착하지 못했습니다.",
                        item);
                }
            }
        }

        public bool TryEquip(
            EquipmentItemDataSO item,
            out EquipmentItemDataSO replacedItem)
        {
            replacedItem = null;

            if (item == null)
                return false;

            EquipmentSlotType slotType = item.slotType;

            if (!_equippedItems.TryGetValue(
                    slotType,
                    out EquipmentItemDataSO previousItem))
            {
                Debug.LogError(
                    $"지원하지 않는 장비 슬롯입니다: {slotType}",
                    item);
                return false;
            }

            if (previousItem == item)
            {
                replacedItem = previousItem;
                return true;
            }

            if (slotType == EquipmentSlotType.Weapon)
            {
                if (item is not WeaponDataSO weaponData)
                {
                    Debug.LogError(
                        $"Weapon 슬롯의 장비 '{item.name}'은 " +
                        $"{nameof(WeaponDataSO)}여야 합니다.",
                        item);
                    return false;
                }

                if (!TryApplyWeapon(weaponData))
                    return false;
            }

            replacedItem = previousItem;
            SetEquippedItem(slotType, item);
            return true;
        }

        public bool TryUnequip(
            EquipmentSlotType slotType,
            out EquipmentItemDataSO removedItem)
        {
            removedItem = null;

            if (!_equippedItems.TryGetValue(
                    slotType,
                    out EquipmentItemDataSO currentItem)
                || currentItem == null)
            {
                return false;
            }

            if (slotType == EquipmentSlotType.Weapon
                && !TryRemoveWeapon())
            {
                return false;
            }

            removedItem = currentItem;
            SetEquippedItem(slotType, null);
            return true;
        }

        public EquipmentItemDataSO GetEquippedItem(
            EquipmentSlotType slotType)
        {
            return _equippedItems.TryGetValue(
                slotType,
                out EquipmentItemDataSO item)
                ? item
                : null;
        }

        public bool IsEquipped(EquipmentItemDataSO item)
        {
            return item != null
                   && GetEquippedItem(item.slotType) == item;
        }

        public void UnequipAll()
        {
            foreach (EquipmentSlotType slotType
                     in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                TryUnequip(slotType, out _);
            }
        }

        private bool TryApplyWeapon(WeaponDataSO weaponData)
        {
            if (_weaponController == null)
                return false;

            _isApplyingWeaponChange = true;

            try
            {
                return _weaponController.EquipWeapon(weaponData);
            }
            finally
            {
                _isApplyingWeaponChange = false;
            }
        }

        private bool TryRemoveWeapon()
        {
            if (_weaponController == null)
                return false;

            _isApplyingWeaponChange = true;

            try
            {
                return _weaponController.UnequipWeapon();
            }
            finally
            {
                _isApplyingWeaponChange = false;
            }
        }

        private void HandleWeaponChanged(WeaponDataSO weaponData)
        {
            if (_isApplyingWeaponChange)
                return;

            SetEquippedItem(
                EquipmentSlotType.Weapon,
                weaponData);
        }

        private void SetEquippedItem(
            EquipmentSlotType slotType,
            EquipmentItemDataSO newItem)
        {
            EquipmentItemDataSO previousItem =
                GetEquippedItem(slotType);

            if (previousItem == newItem)
                return;

            _equippedItems[slotType] = newItem;

            OnEquipmentChanged?.Invoke(
                new EquipmentChangedEvent(
                    slotType,
                    previousItem,
                    newItem));
        }

        private void InitializeSlots()
        {
            _equippedItems.Clear();

            foreach (EquipmentSlotType slotType
                     in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                _equippedItems.Add(slotType, null);
            }
        }

        private void OnDestroy()
        {
            if (_weaponController != null)
                _weaponController.OnWeaponChanged -= HandleWeaponChanged;
        }
    }
}
