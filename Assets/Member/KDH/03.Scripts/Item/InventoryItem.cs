namespace Script.Item
{
    [System.Serializable]
    public class InventoryItem
    {
        public AbstractItemDataSO itemData;
        public int count;

        public InventoryItem(AbstractItemDataSO data, int c)
        {
            itemData = data;
            count = c;
        }
    }
}
