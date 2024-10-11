using UnityEngine;
public class StackableItem : Item
{
    public int Quantity { get; private set; }
    public int MaxStackSize { get; private set; }

    public StackableItem(StackableItemData data, int quantity) : base(data)
    {
        Quantity = quantity;
        MaxStackSize = data.maxStackSize;
    }

    public int AddQuantity(int amount)
    {
        int maxStackSize = MaxStackSize;
        int total = Quantity + amount;
        if (total > maxStackSize)
        {
            int remaining = total - maxStackSize;
            Quantity = maxStackSize;
            //Sửa lý khi vượt giá trị stack
            Debug.Log($"{Data.itemName} stack is full.");
            return remaining;
        }
        else
        {
            Quantity = total;
            return 0;
        }
    }

    public void RemoveQuantity(int amount)
    {
        Quantity -= amount;
        if (Quantity <= 0)
        {
            Quantity = 0;
            //hành động khi stack 0 giá trị
            Debug.Log($"{Data.itemName} is depleted.");
        }
    }
}
