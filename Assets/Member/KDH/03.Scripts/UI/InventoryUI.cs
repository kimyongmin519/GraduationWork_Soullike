using System.Collections.Generic;
using Member.KDH._03.Scripts.Item;
using UnityEngine;

namespace Member.KDH._03.Scripts.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public Transform content;
        public GameObject slotPrefab;

        private List<GameObject> pool = new List<GameObject>();

        public void Refresh(List<InventoryItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                GameObject slotObj = GetSlot(i);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                slot.Setup(items[i], OnItemClicked);
            }

            for (int i = items.Count; i < pool.Count; i++)
            {
                pool[i].SetActive(false);
            }
        }

        private GameObject GetSlot(int index)
        {
            if (index < pool.Count)
            {
                pool[index].SetActive(true);
                return pool[index];
            }

            GameObject newSlot = Instantiate(slotPrefab, content);
            pool.Add(newSlot);
            return newSlot;
        }

        private void OnItemClicked(InventoryItem data)
        {
            Debug.Log("클릭한 아이템: " + data.itemData.itemName + ", 개수: " + data.count);
        }
    }
}
