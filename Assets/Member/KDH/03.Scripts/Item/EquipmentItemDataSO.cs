using UnityEngine;

namespace Member.KDH._03.Scripts.Item
{
    public enum EquipmentSlotType
    {
        Weapon,
        Head,
        Chest,
        Hands,
        Legs
    }

    [CreateAssetMenu(fileName = "New Equipment Item", menuName = "Item/Equipment Item")]
    public class EquipmentItemDataSO : AbstractItemDataSO
    {
        public override ItemType ItemType
        {
            get { return ItemType.Equipment; }
        }

        [Header("장비 전용 정보")]
        public EquipmentSlotType slotType;
        public int attackPower;
        public int defensePower;
    }
}
