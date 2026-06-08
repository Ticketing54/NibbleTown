using System;

[Serializable]
public class ItemSlot
{
    public int itemId;
    public int count;

    public ItemSlot(int _itemId, int _count)
    {
        itemId = _itemId;
        count  = _count;
    }
}
