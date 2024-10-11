using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ToolUsageController : MonoBehaviour
{
    public Animator animator;
    public Player player;
    public StarterAssetsInputs starterAssetsInputs;
    public EquipmentManager equipmentManager;

    private bool isToolInUse = false; // Biến để kiểm soát trạng thái sử dụng công cụ
    private bool isGathering = false; // Biến để kiểm soát trạng thái gathering

    void Update()
    {
        HandleToolUsage();
    }

    void HandleToolUsage()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current.leftButton.wasPressedThisFrame)
#else
        if (Input.GetMouseButtonDown(0))
#endif
        {
            UseTool();
        }
    }

    void UseTool()
    {
        if (player.uiManager.GetisInventoryOpen()) return;

        // Kiểm tra nếu công cụ đang được sử dụng, ngăn không cho sử dụng lại
        if (isToolInUse) return;

        Item currentItem = player.GetHotbar().GetCurrentEquippedItem();

        if (currentItem != null)
        {
            ToolItem toolItem = currentItem as ToolItem;
            if (toolItem != null)
            {
                animator.SetTrigger("UseTool");
                toolItem.UseTool(1f, player);
            }
        }
    }

    public void LockMove()
    {
        starterAssetsInputs.isMove = false;
        starterAssetsInputs.move = Vector2.zero;
    }

    public void UnLockMove()
    {
        starterAssetsInputs.isMove = true;
    }

    public void OnToolAnimationStart()
    {
        Debug.Log("Start");
        isToolInUse = true; // Đặt trạng thái công cụ đang được sử dụng
        LockMove();
        StartUsingTool();
    }

    public void OnToolAnimationEnd()
    {
        UnLockMove();
        isToolInUse = false; // Cho phép sử dụng công cụ lại sau khi hoạt ảnh kết thúc
        StopUsingTool();
    }

    // Phương thức để bắt đầu Gathering
    public void Gathering()
    {
        if (isGathering) return;

        isGathering = true;
        LockMove();
        animator.SetTrigger("Gathering");
    }

    public void OnGatheringAnimationStart()
    {
        // Có thể thêm logic nếu cần khi bắt đầu hoạt ảnh Gathering
    }

    public void OnGatheringAnimationEnd()
    {
        UnLockMove();
        isGathering = false;

        // Gọi phương thức nhặt item trong Player
        player.TryPickupItem();
    }

    public void StartUsingTool()
    {
        if (equipmentManager.currentRightHandItem != null)
        {
            ToolController toolController = equipmentManager.currentRightHandItem.GetComponent<ToolController>();
            if (toolController != null)
            {
                toolController.UseTool();
            }
        }
    }

    public void StopUsingTool()
    {
        if (equipmentManager.currentRightHandItem != null)
        {
            ToolController toolController = equipmentManager.currentRightHandItem.GetComponent<ToolController>();
            if (toolController != null)
            {
                toolController.NoUseTool();
            }
        }
    }
}
