using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemID; // Sử dụng itemID
    public int quantity = 1;

    [Header("Pickup Settings")]
    public bool canPickup = true; // Kiểm tra xem vật phẩm có thể được nhặt hay không
    [SerializeField]
    private float pickUpRange = 2.0f; // Phạm vi để người chơi có thể nhặt vật phẩm

    [Header("Merge Settings")]
    [SerializeField]
    private float mergeInterval = 1.0f; // Thời gian giữa các lần gọi MergeNearbyItems (giây)

    [Header("Effects")]
    public AudioClip pickupSound; // Âm thanh khi nhặt vật phẩm
    public ParticleSystem pickupEffect; // Hiệu ứng khi nhặt vật phẩm

    private Renderer itemRenderer; // Tham chiếu đến Renderer của vật phẩm
    private Material originalMaterial; // Lưu trữ material gốc

    // Thêm thành phần Outline
    private Outline outline;

    // Tham chiếu đến Player khi trong phạm vi
    private Player playerInRange;

    private float mergeTimer; // Bộ đếm thời gian cho MergeNearbyItems

    private void Start()
    {
        // Lưu trữ material gốc và lấy Renderer
        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
        {
            originalMaterial = itemRenderer.material;
        }

        // Thêm và cấu hình Outline
        outline = gameObject.AddComponent<Outline>();
        outline.enabled = false; // Tắt viền mặc định
        outline.OutlineColor = Color.yellow; // Màu viền khi được highlight
        outline.OutlineWidth = 5; // Độ rộng viền

        // Gộp các vật phẩm giống nhau ở gần khi khởi tạo
        MergeNearbyItems();

        // Đặt giá trị ban đầu cho mergeTimer
        mergeTimer = mergeInterval;
    }

    private void FixedUpdate()
    {
        if (canPickup)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickUpRange);
            bool playerNearby = false;

            foreach (Collider collider in colliders)
            {
                Player nearbyPlayer = collider.GetComponentInParent<Player>();
                if (nearbyPlayer != null)
                {
                    playerNearby = true;

                    if (!outline.enabled)
                    {
                        // Bật hiệu ứng outline
                        EnableOutline();

                        // Gọi phương thức trong Player để thông báo có item gần
                        nearbyPlayer.SetNearbyItem(this);
                    }

                    if (playerInRange == null)
                    {
                        playerInRange = nearbyPlayer;
                    }
                    break; // Đã tìm thấy người chơi, không cần kiểm tra thêm
                }
            }

            if (!playerNearby)
            {
                if (outline.enabled)
                {
                    // Tắt hiệu ứng outline
                    DisableOutline();
                }

                if (playerInRange != null)
                {
                    // Gọi phương thức trong Player để thông báo không còn item gần
                    playerInRange.ClearNearbyItem(this);
                    playerInRange = null;
                }
            }
        }

        // Giảm giá trị của mergeTimer theo thời gian
        mergeTimer -= Time.deltaTime;

        // Kiểm tra nếu mergeTimer <= 0 thì gọi MergeNearbyItems và đặt lại mergeTimer
        if (mergeTimer <= 0f)
        {
            MergeNearbyItems();
            mergeTimer = mergeInterval; // Đặt lại mergeTimer
        }
    }

    private void MergeNearbyItems()
    {
        float mergeRange = 1.0f; // Phạm vi để tìm các vật phẩm giống nhau

        Collider[] colliders = Physics.OverlapSphere(transform.position, mergeRange);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject) // Loại trừ chính nó
            {
                ItemPickup otherItemPickup = collider.GetComponent<ItemPickup>();
                if (otherItemPickup != null && otherItemPickup.itemID == this.itemID && otherItemPickup.canPickup)
                {
                    // Gộp số lượng
                    this.quantity += otherItemPickup.quantity;
                    // Phá hủy vật phẩm kia
                    Destroy(otherItemPickup.gameObject);
                }
            }
        }
    }

    public bool Pickup(Player player)
    {
        if (!canPickup)
            return false;  // Trả về false nếu không thể nhặt

        // Kiểm tra xem ItemDatabase đã sẵn sàng chưa
        if (ItemDatabase.Instance == null)
        {
            Debug.LogError("ItemDatabase is not initialized.");
            return false;  // Trả về false nếu ItemDatabase không được khởi tạo
        }
        ItemData itemData = ItemDatabase.Instance.GetItemDataByID(itemID);
        if (itemData == null)
        {
            Debug.LogError($"ItemData not found.");
            return false;  // Trả về false nếu không tìm thấy dữ liệu vật phẩm
        }

        Inventory playerInventory = player.GetInventory();

        bool added = false;

        if (itemData is StackableItemData stackableItemData)
        {
            // Thêm vật phẩm vào inventory nếu là vật phẩm có thể chồng
            Item itemToAdd = CreateItemFromData(stackableItemData, quantity);
            added = playerInventory.AddItem(itemToAdd);
        }
        else
        {
            // Thêm nhiều lần vào inventory nếu vật phẩm không thể chồng và có số lượng lớn hơn 1
            for (int i = 0; i < quantity; i++)
            {
                Item itemToAdd = CreateItemFromData(itemData);
                added = playerInventory.AddItem(itemToAdd);

                if (!added)
                {
                    Debug.Log("Không thể nhặt vật phẩm. Túi đồ đã đầy.");
                    return false;
                }
            }
        }

        if (added)
        {
            // Phát âm thanh nhặt vật phẩm
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Hiệu ứng khi nhặt vật phẩm
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Phá hủy vật phẩm trên scene
            Destroy(gameObject);

            return true;  // Trả về true nếu nhặt vật phẩm thành công
        }
        else
        {
            Debug.Log("Không thể nhặt vật phẩm. Túi đồ đã đầy.");
            return false;  // Trả về false nếu không thể thêm vật phẩm vào inventory
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

    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
