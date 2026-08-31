using KimLIb.ModuleSystems;
using Member.KDH._03.Scripts.Item;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerInventoryEquipmentBridge : MonoBehaviour, IModule
    {
        [SerializeField]
        private InventoryManager inventoryManager;

        private PlayerEquipmentModule _equipmentModule;

        public void Initialize(ModuleOwner owner)
        {
            _equipmentModule = owner.GetModule<PlayerEquipmentModule>();

            Debug.Assert(
                _equipmentModule != null,
                "PlayerInventoryEquipmentBridge requires " +
                "PlayerEquipmentModule.");
        }

        public void SetInventoryManager(InventoryManager newInventoryManager)
        {
            inventoryManager = newInventoryManager;
        }

        public bool TryEquip(InventoryItem inventoryItem)
        {
            if (!IsOwnedItem(inventoryItem))
                return false;

            if (inventoryItem.itemData
                is not EquipmentItemDataSO equipmentData)
            {
                return false;
            }

            return _equipmentModule != null
                   && _equipmentModule.TryEquip(
                       equipmentData,
                       out _);
        }

        public bool TryUnequip(EquipmentSlotType slotType)
        {
            return _equipmentModule != null
                   && _equipmentModule.TryUnequip(
                       slotType,
                       out _);
        }

        private bool IsOwnedItem(InventoryItem inventoryItem)
        {
            return inventoryManager != null
                   && inventoryItem != null
                   && inventoryItem.count > 0
                   && inventoryManager.items.Contains(inventoryItem);
        }
    }
}
