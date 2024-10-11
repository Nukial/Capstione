using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    // Singleton instance
    public static ItemDatabase Instance { get; private set; }

    // Dictionary để lưu trữ ItemData theo itemID
    private Dictionary<string, ItemData> itemDataDictionary = new Dictionary<string, ItemData>();

    // Danh sách tất cả ItemData
    public List<ItemData> AllItemData { get; private set; } = new List<ItemData>();

    private void Awake()
    {
        // Thiết lập singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllItemData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllItemData()
    {
        // Tải tất cả các ItemData từ thư mục Resources/Items
        ItemData[] items = Resources.LoadAll<ItemData>("Items");

        foreach (ItemData item in items)
        {
            if (!itemDataDictionary.ContainsKey(item.itemID))
            {
                itemDataDictionary.Add(item.itemID, item);
                AllItemData.Add(item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemID detected: {item.itemID} for item {item.itemName}");
            }
        }

        Debug.Log($"Loaded {itemDataDictionary.Count} items into the database.");
    }

    // Phương thức để lấy ItemData theo itemID
    public ItemData GetItemDataByID(string itemID)
    {
        if (itemDataDictionary.TryGetValue(itemID, out ItemData itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogError($"Item with ID {itemID} not found in the database.");
            return null;
        }
    }

    // Phương thức để lấy ItemData theo itemName
    public ItemData GetItemDataByName(string itemName)
    {
        foreach (ItemData itemData in AllItemData)
        {
            if (itemData.itemName == itemName)
            {
                return itemData;
            }
        }
        Debug.LogError($"Item with name {itemName} not found in the database.");
        return null;
    }
}
