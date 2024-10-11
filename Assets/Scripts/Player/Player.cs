using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Player : MonoBehaviour
{
    private Inventory playerInventory;
    public Inventory GetInventory() => this.playerInventory;

    public InventoryUI inventoryUI;
    public EquipmentManager equipmentManager;
    private Hotbar hotbar;
    public Hotbar GetHotbar() => hotbar;

    // Thêm các biến mới cho việc nhặt item
    private ItemPickup nearbyItemPickup;
    public UIManager uiManager; // Tham chiếu đến UIManager để hiển thị thông báo
    public PlayerStats playerStats;
    public ToolUsageController usageController;
    bool isLoot = false;

    void Awake()
    {
        // Khởi tạo playerInventory
        if (playerInventory == null)
        {
            playerInventory = new Inventory(20);
            Debug.Log("Player Inventory Initialized in Awake.");
        }

        // Kiểm tra inventoryUI
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI is not assigned in the Player script.");
        }

        // Kiểm tra UIManager
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not assigned in the Player script.");
        }

        //Kiểm tra hotbar
        if (hotbar == null)
        {
            hotbar = GetComponent<Hotbar>();
            if (hotbar == null)
            {
                Debug.LogError("Thành phần Hotbar không được gắn vào người chơi.");
            }
        }

        // Truyền tham chiếu đến hotbar
        if (hotbar != null)
        {
            hotbar.playerInventory = playerInventory;
            hotbar.equipmentManager = equipmentManager;
        }
    }

    void Start()
    {
        // Thêm item vào inventory để test chức năng
        AddTestItems();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // Sử dụng Input System mới nếu được bật
        if (Keyboard.current.fKey.wasPressedThisFrame && isLoot)
        {
            usageController.Gathering();
            isLoot = false;
        }
#else
        // Sử dụng Input System cũ
        if (Input.GetKeyDown(KeyCode.F) && isLoot)
        {
            usageController.Gathering();
            isLoot = false;
        }
#endif
    }

    void AddTestItems()
    {
        // Lấy instance của ItemDatabase
        ItemDatabase itemDatabase = ItemDatabase.Instance;

        // Kiểm tra xem ItemDatabase đã được khởi tạo chưa
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not initialized.");
            return;
        }

        // Ví dụ: Thêm một số item vào inventory
        ItemData itemData2 = itemDatabase.GetItemDataByID("04f12a59-fb98-4a08-892f-e536fc56a07d");
        ItemData itemData3 = itemDatabase.GetItemDataByID("e591774a-2c8f-4968-9e15-e81ad3056c05");
        ItemData itemData4 = itemDatabase.GetItemDataByID("448ca3b7-aed8-495d-b9a2-7d80d84a17d7");
        ItemData itemData5 = itemDatabase.GetItemDataByID("6149fb2f-4dde-4bd8-bc2d-f09c67644cca");
        // Kiểm tra xem ItemData có tồn tại không

        if (itemData2 != null)
        {
            Item item2 = CreateItemFromData(itemData2);
            playerInventory.AddItem(item2);
        }
        if (itemData3 != null)
        {
            Item item3 = CreateItemFromData(itemData3);
            playerInventory.AddItem(item3);
        }
        if (itemData4 != null)
        {
            Item item4 = CreateItemFromData(itemData4);
            playerInventory.AddItem(item4);
        }
        if (itemData5 != null)
        {
            Item item5 = CreateItemFromData(itemData5);
            playerInventory.AddItem(item5);
        }
    }

    Item CreateItemFromData(ItemData itemData, int quantity = 1)
    {
        if (itemData is StackableItemData stackableItemData)
        {
            return new StackableItem(stackableItemData, quantity);
        }
        else if (itemData is ToolItemData toolItemData)
        {
            return new ToolItem(toolItemData);
        }
        else if (itemData is WeaponItemData weaponItemData)
        {
            return new WeaponItem(weaponItemData);
        }
        else if (itemData is FoodItemData foodItemData)
        {
            return new FoodItem(foodItemData);

        }
        else if (itemData is DrinkItemData drinkItemData)
        {
            return new DrinkItem(drinkItemData);
        }
        else
        {
            return new BasicItem(itemData);
        }
    }

    void UseTool(ToolItem tool)
    {
        // Giả sử mỗi lần sử dụng giảm 10 độ bền
        tool.UseTool(10, this);

        if (tool.Durability <= 0)
        {
            // Xóa công cụ khỏi kho khi hỏng
            playerInventory.RemoveItem(tool);
            Debug.Log($"{tool.Data.itemName} has been removed from inventory.");
        }
    }

    // Phương thức để thiết lập item gần nhất
    public void SetNearbyItem(ItemPickup itemPickup)
    {
        // Nếu đã có một item gần rồi, có thể thêm logic để chọn item ưu tiên
        nearbyItemPickup = itemPickup;
        // Hiển thị thông báo nhặt item
        if (uiManager != null && itemPickup != null)
        {
            uiManager.ShowPickupPrompt(itemPickup.itemID, itemPickup.quantity); // Bạn có thể thay đổi để hiển thị tên item
            isLoot = true; // Có thể thực hiện hành động loot
        }
    }

    // Phương thức để xóa item gần nhất
    public void ClearNearbyItem(ItemPickup itemPickup)
    {
        if (nearbyItemPickup == itemPickup)
        {
            nearbyItemPickup = null;

            // Ẩn thông báo nhặt item
            if (uiManager != null)
            {
                uiManager.HidePickupPrompt();
                isLoot = false;
            }
        }
    }

    // Phương thức để nhặt item
    public void TryPickupItem()
    {
        if (nearbyItemPickup != null)
        {
            // Thực hiện nhặt item
            bool success = nearbyItemPickup.Pickup(this);

            if (success)
            {
                // Ẩn thông báo nhặt sau khi nhặt thành công
                uiManager.HidePickupPrompt();

                // Xóa item gần nhất
                nearbyItemPickup = null;
            }
            else
            {
                Debug.Log("Pickup failed.");
            }
        }
    }
}
