using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int MaxSlots { get; private set; }
    private List<Item> items;

    // Sự kiện phát ra khi inventory thay đổi
    public event Action OnInventoryChanged;

    public Inventory(int maxSlots)
    {
        MaxSlots = maxSlots;
        items = new List<Item>(MaxSlots);
    }

    public bool AddItem(Item newItem)
    {
        bool itemAdded = false;

        if (newItem is StackableItem newStackableItem)
        {
            int remainingQuantity = newStackableItem.Quantity;
            // Cố gắng thêm vào các stack hiện có
            foreach (Item item in items)
            {
                if (item is StackableItem existingStackableItem &&
                    existingStackableItem.Data.itemID == newStackableItem.Data.itemID)
                {
                    if (remainingQuantity <= 0)
                        break;

                    // Sử dụng AddQuantity và cập nhật remainingQuantity
                    remainingQuantity = existingStackableItem.AddQuantity(remainingQuantity);
                    itemAdded = true;
                }
            }
            // Tạo các stack mới nếu cần
            while (remainingQuantity > 0 && items.Count < MaxSlots)
            {
                int quantityToAdd = Math.Min(remainingQuantity, newStackableItem.MaxStackSize);
                StackableItem stackItem = new StackableItem((StackableItemData)newStackableItem.Data, 0);
                remainingQuantity = stackItem.AddQuantity(remainingQuantity);
                items.Add(stackItem);
                itemAdded = true;
            }

            if (remainingQuantity > 0)
            {
                Debug.Log("Kho đã đầy. Không thể thêm tất cả các item.");
                // itemAdded vẫn là true nếu đã thêm được một số item
            }
        }
        else
        {
            if (items.Count < MaxSlots)
            {
                items.Add(newItem);
                itemAdded = true;
            }
            else
            {
                Debug.Log("Kho đã đầy.");
                itemAdded = false;
            }
        }

        if (itemAdded)
        {
            // Kích hoạt sự kiện khi kho thay đổi
            OnInventoryChanged?.Invoke();
        }

        return itemAdded;
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            // Phát ra sự kiện khi inventory thay đổi
            OnInventoryChanged?.Invoke();
        }
    }

    public Item GetItem(int index)
    {
        if (index >= 0 && index < items.Count)
            return items[index];
        else
            return null;
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items);
    }

    private StackableItem FindStackableItem(string itemID)
    {
        foreach (Item item in items)
        {
            if (item is StackableItem stackableItem && stackableItem.Data.itemID == itemID)
            {
                return stackableItem;
            }
        }
        return null;
    }

    // Các phương thức tìm kiếm và sắp xếp sẽ được thêm vào sau
    // Sắp xếp item theo tên
    public void SortItemsByName(bool ascending = true)
    {
        if (ascending)
        {
            items.Sort((a, b) => string.Compare(a.Data.itemName, b.Data.itemName, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            items.Sort((a, b) => string.Compare(b.Data.itemName, a.Data.itemName, StringComparison.OrdinalIgnoreCase));
        }
        OnInventoryChanged?.Invoke();
    }

    // Sắp xếp item theo ID
    public void SortItemsByID(bool ascending = true)
    {
        if (ascending)
        {
            items.Sort((a, b) => string.Compare(a.Data.itemID, b.Data.itemID, StringComparison.Ordinal));
        }
        else
        {
            items.Sort((a, b) => string.Compare(b.Data.itemID, a.Data.itemID, StringComparison.Ordinal));
        }
        OnInventoryChanged?.Invoke();
    }

    // Sắp xếp theo tiêu chí tùy chỉnh
    public void SortItems(Comparison<Item> comparison)
    {
        items.Sort(comparison);
        OnInventoryChanged?.Invoke();
    }
}
