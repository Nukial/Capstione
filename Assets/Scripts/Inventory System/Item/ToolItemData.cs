using UnityEngine;

[CreateAssetMenu(fileName = "NewToolItem", menuName = "Items/Tool Item")]
public class ToolItemData : ItemData
{
    [Header("Tool Properties")]
    public float maxDurability;
    public GameObject toolPrefabs;
}
