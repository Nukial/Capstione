using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

public class InventoryUI : MonoBehaviour
{
    public Player player;
    public Inventory inventory;
    public InventorySlot inventorySlotPrefab;
    public Transform inventoryPanel;

    private ObjectPool<InventorySlot> slotPool;
    private List<InventorySlot> activeSlots = new List<InventorySlot>();

    private void Awake()
    {
        // Kiểm tra các tham chiếu cần thiết
        if (inventorySlotPrefab == null)
        {
            Debug.LogError("InventorySlot Prefab chưa được gán trong InventoryUI.");
            return;
        }

        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel chưa được gán trong InventoryUI.");
            return;
        }

        // Khởi tạo Object Pool
        slotPool = new ObjectPool<InventorySlot>(inventorySlotPrefab, inventoryPanel);
        Debug.Log("Object Pool đã được khởi tạo.");
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeInventory());
    }

    private IEnumerator InitializeInventory()
    {
        // Nếu không tìm thấy Player ngay lập tức, chờ đợi cho đến khi nó có mặt
        while (player == null)
        {
            yield return new WaitForSeconds(0.5f);
            player = transform.GetComponentInParent<Player>();
        }
        while (player.GetInventory() == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        inventory = player.GetInventory();
        // Đăng ký sự kiện khi inventory thay đổi
        inventory.OnInventoryChanged += UpdateInventoryUI;
        UpdateInventoryUI();
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện nếu playerInventory không null
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventory == null)
        {
            return;
        }

        // Trả các slot hiện tại về pool
        foreach (var slot in activeSlots)
        {
            slot.Clear(); // Reset trạng thái của slot
            slotPool.ReturnToPool(slot);
        }
        activeSlots.Clear();

        // Lấy danh sách item
        List<Item> items = inventory.GetAllItems();

        // Tạo các slot item
        foreach (var item in items)
        {
            InventorySlot slot = slotPool.Get();
            slot.Setup(item, OnItemSlotClicked, OnItemSlotHoverEnter, OnItemSlotHoverExit);
            activeSlots.Add(slot);

            // Đặt slot vào inventoryPanel
            slot.transform.SetParent(inventoryPanel, false);
        }
    }


    private void OnItemSlotClicked(Item item)
    {
        Debug.Log($"Đã click vào {item.Data.itemName}");

        // Kiểm tra loại item và thực hiện hành động tương ứng
        if (item is ToolItem toolItem)
        {
            // Ví dụ: Trang bị công cụ vào Hotbar
            player.GetHotbar().AssignItemToNextAvailableSlot(toolItem);
        }
        else if (item is WeaponItem weaponItem)
        {
            // Ví dụ: Trang bị vũ khí vào Hotbar
            player.GetHotbar().AssignItemToNextAvailableSlot(weaponItem);
        }
        else if (item is StackableItem stackableItem)
        {
            // Ví dụ: Sử dụng vật phẩm tiêu hao

        }
        else if (item is FoodItem foodItem)
        {
            foodItem.Consume(player.playerStats);
            inventory.RemoveItem(item);
        }
        else if (item is DrinkItem drinkItem) 
        {
            drinkItem.Consume(player.playerStats);
            inventory.RemoveItem(item);
        }
        else
        {
            // Xử lý cho các loại item khác
            Debug.Log("Loại item không được hỗ trợ.");
        }
    }


    private void OnItemSlotHoverEnter(Item item)
    {
        Debug.Log($"Hover vào {item.Data.itemName}");
        // Hiển thị thông tin item
        if (player.uiManager != null)
        {
            player.uiManager.ShowItemTooltip(item);
        }
    }

    private void OnItemSlotHoverExit()
    {
        Debug.Log("Rời khỏi item");
        // Ẩn thông tin item
        if (player.uiManager != null)
        {
            player.uiManager.HideItemTooltip();
        }
    }
}
