using UnityEngine;

[CreateAssetMenu(fileName = "NewStackableItem", menuName = "Items/Stackable Item")]
public class StackableItemData : ItemData
{
    [Header("Stackable Properties")]
    public int maxStackSize;
}
