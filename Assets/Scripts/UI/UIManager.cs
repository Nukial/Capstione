using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using TMPro; // Thêm namespace TMPro

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject settingsPanel;
    public GameObject aimPanel;
    // Thêm các panel khác tại đây

    [Header("Player Status UI")]
    public GameObject playerStatusUI; // Tham chiếu đến UI trạng thái người chơi

    [Header("Pickup Prompt")]
    public GameObject pickupPromptUI; // Tham chiếu đến UI thông báo nhặt item
    public TMP_Text pickupPromptText; // Text hiển thị tên item

    [Header("Item Tooltip")]
    public GameObject tooltipPanel;        // Panel hiển thị thông tin item
    public TMP_Text itemNameText;          // Text hiển thị tên item
    public TMP_Text itemDescriptionText;   // Text hiển thị mô tả item

    [Header("Toggle Keys")]
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode settingsKey = KeyCode.Escape;
    public KeyCode aimKey = KeyCode.Mouse1; // Ví dụ: Click phải để tầm nhắm

    private bool isInventoryOpen = false;
    private bool isSettingsOpen = false;
    private bool isAimActive = false;

    private StarterAssets.StarterAssetsInputs playerInputs;
    public bool GetisInventoryOpen() => isInventoryOpen;

    void Start()
    {
        // Khởi động tất cả các panel ở trạng thái đóng
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (aimPanel != null) aimPanel.SetActive(false);
        if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (playerStatusUI != null) playerStatusUI.SetActive(true); // Đảm bảo playerStatusUI được bật khi bắt đầu

        // Lấy component StarterAssetsInputs từ chính GameObject này
        playerInputs = transform.GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
        if (playerInputs == null)
        {
            Debug.LogError("StarterAssetsInputs không được tìm thấy trên " + gameObject.name);
        }

        // Khởi tạo trạng thái con trỏ khi bắt đầu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleInventoryToggle();
        HandleSettingsToggle();
        HandleAimToggle();
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            TogglePanel(ref isInventoryOpen, inventoryPanel);
        }
    }

    private void HandleSettingsToggle()
    {
        if (Input.GetKeyDown(settingsKey))
        {
            TogglePanel(ref isSettingsOpen, settingsPanel);
        }
    }

    private void HandleAimToggle()
    {
        if (Input.GetKeyDown(aimKey))
        {
            ToggleAim();
        }
    }

    private void TogglePanel(ref bool isOpen, GameObject panel)
    {
        if (panel == null || playerInputs == null)
        {
            Debug.LogWarning("Panel hoặc StarterAssetsInputs chưa được gán cho " + gameObject.name);
            return;
        }

        isOpen = !isOpen;
        panel.SetActive(isOpen);

        if (isOpen)
        {
            OpenUI();
        }
        else
        {
            CloseUI();
        }

        // Kiểm tra nếu panel là Inventory Panel
        if (panel == inventoryPanel)
        {
            if (playerStatusUI != null)
            {
                // Bật/tắt playerStatusUI ngược lại với trạng thái của Inventory
                playerStatusUI.SetActive(!isOpen);
            }
        }
    }

    private void ToggleAim()
    {
        if (aimPanel == null || playerInputs == null)
        {
            Debug.LogWarning("Aim Panel hoặc StarterAssetsInputs chưa được gán cho " + gameObject.name);
            return;
        }
        if (isInventoryOpen == true)
        {
            Debug.Log("Inventory đang mở");
            return;
        }

        isAimActive = !isAimActive;
        aimPanel.SetActive(isAimActive);

        if (isAimActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;

            // Tạm dừng các hệ thống nhập liệu khác nếu cần
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;

            // Kích hoạt lại các hệ thống nhập liệu khác nếu cần
        }
    }

    private void OpenUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        playerInputs.cursorLocked = false;
        playerInputs.cursorInputForLook = false;

        playerInputs.LookInput(Vector2.zero);

        // Tắt các hệ thống nhập liệu khác nếu cần
    }

    private void CloseUI()
    {
        //Close Hide Item Tool tip
        HideItemTooltip();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerInputs.cursorLocked = true;
        playerInputs.cursorInputForLook = true;

        // Kích hoạt lại các hệ thống nhập liệu khác nếu cần
    }

    // Phương thức để hiển thị thông tin item khi hover vào
    public void ShowItemTooltip(Item item)
    {
        if (tooltipPanel != null && item != null)
        {
            tooltipPanel.SetActive(true);

            if (itemNameText != null)
            {
                itemNameText.text = item.Data.itemName;
            }
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = item.Data.description;
            }

            // Cập nhật vị trí của tooltip theo vị trí chuột
            Vector2 anchoredPosition;
            RectTransform canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null,
                out anchoredPosition);

            // Thêm khoảng cách để tooltip không che mất con trỏ
            float offset = 10f; // điều chỉnh khoảng cách này theo ý muốn
            anchoredPosition.x += offset;
            anchoredPosition.y -= offset;

            // Lấy kích thước của tooltip
            RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            Vector2 tooltipSize = tooltipRect.sizeDelta;

            // Điều chỉnh vị trí để tooltip không nằm ngoài màn hình
            float pivotX = tooltipRect.pivot.x;
            float pivotY = tooltipRect.pivot.y;

            // Tính toán vị trí của các cạnh tooltip so với canvas
            float leftEdge = anchoredPosition.x - (tooltipSize.x * pivotX);
            float rightEdge = anchoredPosition.x + (tooltipSize.x * (1 - pivotX));
            float topEdge = anchoredPosition.y + (tooltipSize.y * (1 - pivotY));
            float bottomEdge = anchoredPosition.y - (tooltipSize.y * pivotY);

            // Lấy kích thước của canvas
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            // Điều chỉnh vị trí X
            if (rightEdge > canvasWidth / 2)
            {
                anchoredPosition.x = (canvasWidth / 2) - (tooltipSize.x * (1 - pivotX));
            }
            else if (leftEdge < -canvasWidth / 2)
            {
                anchoredPosition.x = (-canvasWidth / 2) + (tooltipSize.x * pivotX);
            }

            // Điều chỉnh vị trí Y
            if (topEdge > canvasHeight / 2)
            {
                anchoredPosition.y = (canvasHeight / 2) - (tooltipSize.y * (1 - pivotY));
            }
            else if (bottomEdge < -canvasHeight / 2)
            {
                anchoredPosition.y = (-canvasHeight / 2) + (tooltipSize.y * pivotY);
            }

            // Cập nhật vị trí của tooltip
            tooltipRect.anchoredPosition = anchoredPosition;
        }
    }


    // Phương thức để ẩn thông tin item khi rời khỏi
    public void HideItemTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    // Các phương thức hiện có để hiển thị/ẩn thông báo nhặt item
    public void ShowPickupPrompt(string itemID, int quality = 1)
    {
        if (pickupPromptUI != null && pickupPromptText != null)
        {
            pickupPromptUI.SetActive(true);

            // Lấy tên item từ ItemDatabase
            ItemData itemData = ItemDatabase.Instance.GetItemDataByID(itemID);
            string itemName = itemData != null ? itemData.itemName : "Unknown Item";

            pickupPromptText.text = $"Nhấn F để nhặt {itemName} x{quality}";
        }
    }

    public void HidePickupPrompt()
    {
        if (pickupPromptUI != null)
        {
            pickupPromptUI.SetActive(false);
        }
    }
}
