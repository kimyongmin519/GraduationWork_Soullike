using System.Collections.Generic;
using Member.KDH._03.Scripts.Item;
using UnityEngine;

namespace Member.KDH._03.Scripts.UI
{
    public class TestInventory : MonoBehaviour
    {
        public InventoryUI inventoryUI;
        public List<AbstractItemDataSO> testItemDatas;

        private InventoryManager inventoryManager;

        void Awake()
        {
            inventoryManager = gameObject.AddComponent<InventoryManager>();
        }

        void Start()
        {
            for (int i = 0; i < testItemDatas.Count; i++)
            {
                inventoryManager.AddItem(testItemDatas[i]);
            }

            inventoryUI.Refresh(inventoryManager.items);
        }

        public void ShowAll()
        {
            inventoryUI.Refresh(inventoryManager.items);
        }

        public void ShowEquipment()
        {
            inventoryUI.Refresh(inventoryManager.GetItemsByType(ItemType.Equipment));
        }

        public void ShowConsumable()
        {
            inventoryUI.Refresh(inventoryManager.GetItemsByType(ItemType.Consumable));
        }

        public void ShowMaterial()
        {
            inventoryUI.Refresh(inventoryManager.GetItemsByType(ItemType.Material));
        }
    }
}
