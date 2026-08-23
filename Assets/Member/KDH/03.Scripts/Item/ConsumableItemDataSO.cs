using UnityEngine;

namespace Script.Item
{
    [CreateAssetMenu(fileName = "New Consumable Item", menuName = "Item/Consumable Item")]
    public class ConsumableItemDataSO : AbstractItemDataSO
    {
        public override ItemType ItemType
        {
            get { return ItemType.Consumable; }
        }

        public int healAmount;
    }
}
