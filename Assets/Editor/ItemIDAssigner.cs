#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ItemIDAssigner : EditorWindow
{
    [MenuItem("Tools/Assign Item IDs")]
    public static void ShowWindow()
    {
        GetWindow<ItemIDAssigner>("Assign Item IDs");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Assign IDs to All Items"))
        {
            AssignIDs();
        }
    }

    private void AssignIDs()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Resources/Items" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(path);

            if (string.IsNullOrEmpty(itemData.itemID))
            {
                itemData.itemID = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(itemData);
                Debug.Log($"Assigned ID {itemData.itemID} to item {itemData.itemName}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
