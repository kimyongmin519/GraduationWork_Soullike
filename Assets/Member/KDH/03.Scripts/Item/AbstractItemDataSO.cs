using UnityEngine;

namespace Member.KDH._03.Scripts.Item
{
    public enum ItemType
    {
        Equipment,
        Consumable,
        Material
    }

    public abstract class AbstractItemDataSO : ScriptableObject
    {
        [Header("공통 아이템 정보")]
        public string itemName;
        public Sprite icon;
        [TextArea]
        public string description;
        public int maxStack = 99;

        public abstract ItemType ItemType { get; }
    }
}
