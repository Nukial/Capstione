using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ToolItem : Item
{
    public float Durability { get; private set; }
    public GameObject ToolPrefab;

    public ToolItem(ToolItemData data) : base(data)
    {
        Durability = data.maxDurability;
        ToolPrefab = data.toolPrefabs;
    }

    public void UseTool(float amount, Player player)
    {
        Durability -= amount;
        if (Durability <= 0)
        {
            Durability = 0;
            // Xử lý khi công cụ bị hỏng
            Debug.Log($"{Data.itemName} đã hỏng.");

            if (player != null)
            {
                // Xóa khỏi thanh công cụ
                Hotbar hotbar = player.GetHotbar();
                if (hotbar != null)
                {
                    for (int i = 0; i < hotbar.hotbarSize; i++)
                    {
                        if (hotbar.GetItemInSlot(i) == this)
                        {
                            hotbar.RemoveItemFromHotbarSlot(i);
                            break;
                        }
                    }
                }
                player.GetInventory().RemoveItem(this);
            }
        }
    }


    public void RepairTool(float amount)
    {
        Durability += amount;
        float maxDurability = ((ToolItemData)Data).maxDurability;
        if (Durability > maxDurability)
        {
            Durability = maxDurability;
        }
    }
}
