using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Item
{
    public class InventorySlot : MonoBehaviour
    {
        public Image icon;
        public TMP_Text nameText;
        public TMP_Text countText;
        public Button button;

        private InventoryItem currentData;

        public void Setup(InventoryItem data, Action<InventoryItem> onClick)
        {
            currentData = data;

            icon.sprite = data.itemData.icon;
            nameText.text = data.itemData.itemName;
            countText.text = "X" + data.count;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { onClick(currentData); });
        }
    }
}
