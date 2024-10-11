#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Lấy tham chiếu đến ItemData hiện tại
        ItemData itemData = (ItemData)target;

        // Vẽ các thuộc tính mặc định
        DrawDefaultInspector();

        // Thêm khoảng trống
        EditorGUILayout.Space();

        // Hiển thị hình ảnh Sprite lớn hơn
        if (itemData.icon != null)
        {
            // Lấy tỉ lệ chiều rộng phù hợp
            float aspectRatio = (float)itemData.icon.texture.width / itemData.icon.texture.height;
            float width = EditorGUIUtility.currentViewWidth - 40; // Trừ đi một chút để vừa với Inspector
            float height = width / aspectRatio;

            // Vẽ hình ảnh Sprite
            Rect rect = GUILayoutUtility.GetRect(width, height);
            EditorGUI.DrawPreviewTexture(rect, itemData.icon.texture);
        }
        else
        {
            EditorGUILayout.HelpBox("No Icon assigned.", MessageType.Warning);
        }
    }
}
#endif
