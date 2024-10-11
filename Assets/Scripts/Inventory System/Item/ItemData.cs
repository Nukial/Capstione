using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
}
