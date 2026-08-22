using System.Collections.Generic;
using UnityEngine;

namespace Script.Item
{
    public class InventoryManager : MonoBehaviour
    {
        public List<InventoryItem> items = new List<InventoryItem>();

        public void AddItem(AbstractItemDataSO itemData, int count = 1)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemData == itemData)
                {
                    items[i].count += count;
                    if (items[i].count > itemData.maxStack)
                        items[i].count = itemData.maxStack;
                    return;
                }
            }

            items.Add(new InventoryItem(itemData, count));
        }

        public void RemoveItem(AbstractItemDataSO itemData, int count = 1)
        {
            InventoryItem target = null;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemData == itemData)
                {
                    target = items[i];
                    break;
                }
            }

            if (target == null)
                return;

            target.count -= count;

            if (target.count <= 0)
                items.Remove(target);
        }

        public List<InventoryItem> GetItemsByType(ItemType type)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in items)
            {
                if (item.itemData.ItemType == type)
                    result.Add(item);
            }

            return result;
        }
    }
}
