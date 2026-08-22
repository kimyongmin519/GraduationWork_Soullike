using UnityEngine;

namespace Script.Item
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
