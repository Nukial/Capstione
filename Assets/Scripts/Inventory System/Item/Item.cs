using UnityEngine;

public enum ItemType
{
    None,
    Sword,
    Pickaxe,
    Hammer,
    Plow,
    FishingRod
}
public abstract class Item
{
    public ItemData Data { get; private set; }

    protected Item(ItemData data)
    {
        Data = data;
    }
}
