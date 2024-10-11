using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections.Generic;

public class Hotbar : MonoBehaviour
{
    public int hotbarSize = 4;
    private List<Item> hotbarItems;
    private int currentHotbarIndex = 0;

    public EquipmentManager equipmentManager;
    public Inventory playerInventory;

    void Start()
    {
        hotbarItems = new List<Item>(new Item[hotbarSize]);

        // Khởi tạo các ô trang bị với giá trị rỗng
        for (int i = 0; i < hotbarSize; i++)
        {
            hotbarItems[i] = null;
        }

        // Trang bị mục đầu tiên nếu có
        EquipCurrentItem();
    }

    void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
#if ENABLE_INPUT_SYSTEM
        float scrollValue = Mouse.current.scroll.ReadValue().y;
#else
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
#endif
        if (scrollValue > 0f)
        {
            ScrollUp();
        }
        else if (scrollValue < 0f)
        {
            ScrollDown();
        }
    }

    private void ScrollUp()
    {
        currentHotbarIndex = (currentHotbarIndex + 1) % hotbarSize;
        EquipCurrentItem();
    }

    private void ScrollDown()
    {
        currentHotbarIndex = (currentHotbarIndex - 1 + hotbarSize) % hotbarSize;
        EquipCurrentItem();
    }

    private void EquipCurrentItem()
    {
        Item itemToEquip = hotbarItems[currentHotbarIndex];

        if (itemToEquip != null && itemToEquip is ToolItem toolItem)
        {
            equipmentManager.EquipRightHand(toolItem.ToolPrefab);
        }
        else
        {
            equipmentManager.UnequipRightHand();
        }
    }

    public void AssignItemToNextAvailableSlot(Item item, int startSlotIndex = 0)
    {
        if (item == null)
        {
            Debug.LogWarning("Item không hợp lệ.");
            return;
        }

        // Kiểm tra xem item đã có trong hotbar chưa
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbarItems[i] == item)
            {
                Debug.Log($"Item {item.Data.itemName} đã có trong hotbar tại slot {i}.");
                return; // Item đã có trong hotbar, dừng lại.
            }
        }

        // Tìm slot trống để gán item
        for (int i = startSlotIndex; i < hotbarSize; i++)
        {
            if (hotbarItems[i] == null)
            {
                hotbarItems[i] = item;
                Debug.Log($"Item {item.Data.itemName} đã được thêm vào hotbar tại slot {i}.");
                EquipCurrentItem(); // Đưa công cụ lên tay.
                return;
            }
        }

        // Nếu không có slot trống
        Debug.Log("No empty slot available in hotbar to assign the item.");
    }

    public void RemoveItemFromHotbarSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < hotbarSize)
        {
            hotbarItems[slotIndex] = null;
            equipmentManager.UnequipRightHand();
        }
    }

    // Tùy chọn: Các phương thức để lấy item trong ô hoặc mục đang được trang bị
    public Item GetItemInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < hotbarSize)
        {
            return hotbarItems[slotIndex];
        }
        return null;
    }

    public Item GetCurrentEquippedItem()
    {
        return hotbarItems[currentHotbarIndex];
    }

    public int GetCurrenIndex() => currentHotbarIndex;
}
