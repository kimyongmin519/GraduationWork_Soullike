using UnityEngine;

namespace Member.KDH._03.Scripts.Item
{
    [CreateAssetMenu(fileName = "New Material Item", menuName = "Item/Material Item")]
    public class MaterialItemDataSO : AbstractItemDataSO
    {
        public override ItemType ItemType
        {
            get { return ItemType.Material; }
        }
    }
}
