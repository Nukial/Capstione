#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EnhancedMultiSpawner))]
public class EnhancedMultiSpawnerEditor : Editor
{
    SerializedProperty spawnSettingsListProp;
    SerializedProperty terrainSelectionsProp;
    SerializedProperty allTerrainTexturesProp;
    SerializedProperty spawningEnabledProp;
    SerializedProperty spawnInEditorProp;

    void OnEnable()
    {
        spawnSettingsListProp = serializedObject.FindProperty("spawnSettingsList");
        terrainSelectionsProp = serializedObject.FindProperty("terrainSelections");
        allTerrainTexturesProp = serializedObject.FindProperty("allTerrainTextures");
        spawningEnabledProp = serializedObject.FindProperty("spawningEnabled");
        spawnInEditorProp = serializedObject.FindProperty("spawnInEditor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnhancedMultiSpawner spawner = (EnhancedMultiSpawner)target;

        // Nút để refresh terrains và textures
        if (GUILayout.Button("Refresh Terrains and Textures"))
        {
            spawner.GatherAllTerrainsAndTextures();
            EditorUtility.SetDirty(spawner);
        }

        EditorGUILayout.Space();

        // Hiển thị Terrain Selection
        EditorGUILayout.LabelField("Terrain Selection", EditorStyles.boldLabel);
        for (int i = 0; i < terrainSelectionsProp.arraySize; i++)
        {
            SerializedProperty terrainSelection = terrainSelectionsProp.GetArrayElementAtIndex(i);
            SerializedProperty terrainProp = terrainSelection.FindPropertyRelative("terrain");
            SerializedProperty includeProp = terrainSelection.FindPropertyRelative("include");

            EditorGUILayout.BeginHorizontal();
            string terrainName = (terrainProp.objectReferenceValue != null) ? ((Terrain)terrainProp.objectReferenceValue).name : "Unnamed Terrain";
            includeProp.boolValue = EditorGUILayout.ToggleLeft(terrainName, includeProp.boolValue);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // Hiển thị Spawn Settings
        EditorGUILayout.LabelField("Spawn Settings", EditorStyles.boldLabel);
        for (int i = 0; i < spawnSettingsListProp.arraySize; i++)
        {
            SerializedProperty spawnSetting = spawnSettingsListProp.GetArrayElementAtIndex(i);
            SerializedProperty prefabProp = spawnSetting.FindPropertyRelative("prefab");
            SerializedProperty allowedTexturesProp = spawnSetting.FindPropertyRelative("allowedTerrainTextures");
            SerializedProperty minHeightProp = spawnSetting.FindPropertyRelative("minHeight");
            SerializedProperty maxHeightProp = spawnSetting.FindPropertyRelative("maxHeight");
            SerializedProperty minSlopeProp = spawnSetting.FindPropertyRelative("minSlope");
            SerializedProperty maxSlopeProp = spawnSetting.FindPropertyRelative("maxSlope");
            SerializedProperty alignProp = spawnSetting.FindPropertyRelative("alignToTerrain");
            SerializedProperty maxSpawnProp = spawnSetting.FindPropertyRelative("maxSpawnedObjects");
            SerializedProperty respawnTimeProp = spawnSetting.FindPropertyRelative("respawnTime");
            SerializedProperty spawnRadiusProp = spawnSetting.FindPropertyRelative("spawnRadius");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prefabProp, new GUIContent($"Prefab {i + 1}"));
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                // Nếu có spawned objects, xóa chúng trước khi remove
                GameObject prefab = (GameObject)prefabProp.objectReferenceValue;
                if (prefab != null)
                {
                    spawner.ClearSpawnedObjectsByType(prefab);
                }

                spawnSettingsListProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (prefabProp.objectReferenceValue != null)
            {
                // Allowed Terrain Textures
                EditorGUILayout.PropertyField(allowedTexturesProp, new GUIContent("Allowed Terrain Textures"), true);

                // Nếu chưa có texture nào được chọn, cung cấp nút để chọn tất cả
                if (allowedTexturesProp.arraySize == 0 && spawner.allTerrainTextures.Count > 0)
                {
                    if (GUILayout.Button("Select All Textures"))
                    {
                        allowedTexturesProp.arraySize = spawner.allTerrainTextures.Count;
                        for (int t = 0; t < spawner.allTerrainTextures.Count; t++)
                        {
                            allowedTexturesProp.GetArrayElementAtIndex(t).stringValue = spawner.allTerrainTextures[t];
                        }
                    }
                }

                // Hiển thị danh sách tất cả các Texture Terrain với các checkbox
                EditorGUILayout.LabelField("Select Allowed Textures:", EditorStyles.label);
                foreach (string texture in spawner.allTerrainTextures)
                {
                    bool isAllowed = allowedTexturesProp.Contains(texture);
                    bool newIsAllowed = EditorGUILayout.ToggleLeft(texture, isAllowed);
                    if (newIsAllowed && !isAllowed)
                    {
                        allowedTexturesProp.arraySize++;
                        allowedTexturesProp.GetArrayElementAtIndex(allowedTexturesProp.arraySize - 1).stringValue = texture;
                    }
                    else if (!newIsAllowed && isAllowed)
                    {
                        int indexToRemove = allowedTexturesProp.IndexOf(texture);
                        if (indexToRemove != -1)
                        {
                            allowedTexturesProp.DeleteArrayElementAtIndex(indexToRemove);
                        }
                    }
                }

                // Constraints về chiều cao
                EditorGUILayout.PropertyField(minHeightProp, new GUIContent("Min Height"));
                EditorGUILayout.PropertyField(maxHeightProp, new GUIContent("Max Height"));

                // Constraints về độ nghiêng
                EditorGUILayout.PropertyField(minSlopeProp, new GUIContent("Min Slope (°)"));
                EditorGUILayout.PropertyField(maxSlopeProp, new GUIContent("Max Slope (°)"));

                // Align to Terrain
                EditorGUILayout.PropertyField(alignProp, new GUIContent("Align To Terrain"));

                // Maximum Spawned Objects
                EditorGUILayout.PropertyField(maxSpawnProp, new GUIContent("Max Spawned Objects"));

                // Respawn Time
                EditorGUILayout.PropertyField(respawnTimeProp, new GUIContent("Respawn Time"));

                // Spawn Radius
                EditorGUILayout.PropertyField(spawnRadiusProp, new GUIContent("Spawn Radius"));

                // Thêm nút Spawn và Xoá theo loại trong Inspector
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Spawn Now"))
                {
                    // Gọi SpawnObjectsUntilLimit để spawn đến giới hạn
                    spawner.SpawnObjectsUntilLimit(spawner.spawnSettingsList[i]);
                }
                if (GUILayout.Button("Clear Spawned"))
                {
                    GameObject prefab = (GameObject)prefabProp.objectReferenceValue;
                    spawner.ClearSpawnedObjectsByType(prefab);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // Nút để thêm một Spawn Setting mới
        if (GUILayout.Button("Add New Spawn Setting"))
        {
            spawnSettingsListProp.arraySize++;
            SerializedProperty newSpawnSetting = spawnSettingsListProp.GetArrayElementAtIndex(spawnSettingsListProp.arraySize - 1);
            newSpawnSetting.FindPropertyRelative("prefab").objectReferenceValue = null;
            newSpawnSetting.FindPropertyRelative("allowedTerrainTextures").ClearArray();
            newSpawnSetting.FindPropertyRelative("minHeight").floatValue = 0f;
            newSpawnSetting.FindPropertyRelative("maxHeight").floatValue = 100f;
            newSpawnSetting.FindPropertyRelative("minSlope").floatValue = 0f;
            newSpawnSetting.FindPropertyRelative("maxSlope").floatValue = 90f;
            newSpawnSetting.FindPropertyRelative("alignToTerrain").boolValue = true;
            newSpawnSetting.FindPropertyRelative("maxSpawnedObjects").intValue = 10;
            newSpawnSetting.FindPropertyRelative("respawnTime").floatValue = 5f;
            newSpawnSetting.FindPropertyRelative("spawnRadius").floatValue = 1f;
        }

        EditorGUILayout.Space();

        // Hiển thị General Settings
        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spawningEnabledProp, new GUIContent("Spawning Enabled"));
        EditorGUILayout.PropertyField(spawnInEditorProp, new GUIContent("Spawn In Editor"));

        EditorGUILayout.Space();

        // Áp dụng các thay đổi
        serializedObject.ApplyModifiedProperties();
    }
}

/// <summary>
/// Các phương thức mở rộng cho SerializedProperty.
/// </summary>
public static class SerializedPropertyExtensions
{
    /// <summary>
    /// Kiểm tra xem một danh sách SerializedProperty có chứa một giá trị string cụ thể hay không.
    /// </summary>
    public static bool Contains(this SerializedProperty list, string value)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            if (list.GetArrayElementAtIndex(i).stringValue == value)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Tìm chỉ số của một giá trị string cụ thể trong danh sách SerializedProperty.
    /// Trả về -1 nếu không tìm thấy.
    /// </summary>
    public static int IndexOf(this SerializedProperty list, string value)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            if (list.GetArrayElementAtIndex(i).stringValue == value)
                return i;
        }
        return -1;
    }
}
#endif
