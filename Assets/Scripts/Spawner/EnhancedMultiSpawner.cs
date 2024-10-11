using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EnhancedMultiSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSettings
    {
        [Tooltip("Prefab để spawn.")]
        public GameObject prefab; // Prefab để spawn

        [Tooltip("Chọn các Texture của Terrain hợp lệ để spawn.")]
        public List<string> allowedTerrainTextures = new List<string>(); // Danh sách tên Texture của Terrain hợp lệ để spawn

        [Tooltip("Giới hạn chiều cao để spawn.")]
        public float minHeight = 0f; // Chiều cao tối thiểu để spawn

        [Tooltip("Giới hạn chiều cao để spawn.")]
        public float maxHeight = 100f; // Chiều cao tối đa để spawn

        [Tooltip("Giới hạn độ nghiêng tối thiểu để spawn.")]
        public float minSlope = 0f; // Độ nghiêng tối thiểu để spawn (tính bằng độ)

        [Tooltip("Giới hạn độ nghiêng tối đa để spawn.")]
        public float maxSlope = 90f; // Độ nghiêng tối đa để spawn (tính bằng độ)

        [Tooltip("Nghiêng prefab theo độ dốc của Terrain.")]
        public bool alignToTerrain = true; // Nghiêng prefab theo độ dốc của Terrain

        [Tooltip("Số lượng tối đa prefab này được spawn.")]
        public int maxSpawnedObjects = 10; // Số lượng tối đa prefab được spawn

        [Tooltip("Thời gian giữa các lần respawn.")]
        public float respawnTime = 5f; // Thời gian giữa các lần respawn

        [Tooltip("Bán kính kiểm tra collider để tránh spawn chồng lấp.")]
        public float spawnRadius = 1f; // Bán kính kiểm tra collider để tránh spawn chồng lấp
    }

    [System.Serializable]
    public class TerrainSelection
    {
        public Terrain terrain; // Reference đến Terrain
        public bool include = true; // Bao gồm Terrain này cho việc spawn
    }

    [Header("Cài Đặt Spawn")]
    public List<SpawnSettings> spawnSettingsList = new List<SpawnSettings>(); // Danh sách các cài đặt spawn cho từng prefab

    [Header("Terrain Selection")]
    public List<TerrainSelection> terrainSelections = new List<TerrainSelection>(); // Danh sách các Terrain để bao gồm/bỏ qua

    [Header("General Settings")]
    public bool spawningEnabled = true; // Bật/tắt việc spawn
    public bool spawnInEditor = false; // Cho phép spawn trong Editor

    // Dictionary để quản lý các object đã spawn theo prefab trong runtime
    [HideInInspector]
    public Dictionary<GameObject, List<GameObject>> spawnedObjectsDict = new Dictionary<GameObject, List<GameObject>>();

    private Dictionary<GameObject, float> respawnTimers = new Dictionary<GameObject, float>(); // Bộ đếm thời gian respawn cho từng prefab

    // Danh sách tất cả các Texture duy nhất từ tất cả các Terrain trong scene
    [HideInInspector]
    public List<string> allTerrainTextures = new List<string>();

    void Awake()
    {
        GatherAllTerrainsAndTextures();
    }

    void Start()
    {
        InitializeSpawnedObjectsDict();

        if (!Application.isPlaying && !spawnInEditor)
            return;

        SpawnInitialObjects();
    }

    void Update()
    {
        if (!Application.isPlaying && !spawnInEditor)
            return;

        if (spawningEnabled)
        {
            foreach (var settings in spawnSettingsList)
            {
                if (settings.prefab == null)
                    continue;

                if (!respawnTimers.ContainsKey(settings.prefab))
                {
                    respawnTimers[settings.prefab] = 0f;
                }

                respawnTimers[settings.prefab] += Time.deltaTime;

                if (respawnTimers[settings.prefab] >= settings.respawnTime)
                {
                    respawnTimers[settings.prefab] = 0f;
                    CleanUpSpawnedObjects(settings.prefab);
                    SpawnObjectsUntilLimit(settings);
                }
            }
        }
    }

    /// <summary>
    /// Thu thập tất cả các Terrain và Texture trong scene.
    /// </summary>
    public void GatherAllTerrainsAndTextures()
    {
        allTerrainTextures.Clear();
        HashSet<string> textureSet = new HashSet<string>();

        // Clear và populate terrain selections
        terrainSelections.Clear();
        Terrain[] terrains = Terrain.activeTerrains;
        foreach (Terrain terrain in terrains)
        {
            terrainSelections.Add(new TerrainSelection { terrain = terrain, include = true });

            // Thu thập các Texture của Terrain
            foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
            {
                if (layer != null && !string.IsNullOrEmpty(layer.name))
                {
                    textureSet.Add(layer.name);
                }
            }
        }

        allTerrainTextures.AddRange(textureSet);
    }

    /// <summary>
    /// Khởi tạo dictionary quản lý các object đã spawn.
    /// </summary>
    void InitializeSpawnedObjectsDict()
    {
        foreach (var settings in spawnSettingsList)
        {
            if (settings.prefab == null)
                continue;

            if (!spawnedObjectsDict.ContainsKey(settings.prefab))
            {
                spawnedObjectsDict[settings.prefab] = new List<GameObject>();
            }
        }
    }

    /// <summary>
    /// Spawn các object ban đầu dựa trên cài đặt spawn.
    /// </summary>
    void SpawnInitialObjects()
    {
        foreach (var settings in spawnSettingsList)
        {
            if (settings.prefab == null)
                continue;

            for (int i = 0; i < settings.maxSpawnedObjects; i++)
            {
                SpawnObject(settings);
            }
        }
    }

    /// <summary>
    /// Spawn các object cho đến khi đạt giới hạn tối đa.
    /// </summary>
    public void SpawnObjectsUntilLimit(SpawnSettings settings)
    {
        int currentCount = CountSpawnedObjects(settings.prefab);
        while (currentCount < settings.maxSpawnedObjects)
        {
            SpawnObject(settings);
            currentCount++;
        }
    }

    /// <summary>
    /// Loại bỏ các object đã bị hủy hoặc null khỏi danh sách.
    /// </summary>
    void CleanUpSpawnedObjects(GameObject prefab)
    {
        if (spawnedObjectsDict.ContainsKey(prefab))
        {
            spawnedObjectsDict[prefab].RemoveAll(item => item == null);
        }
    }

    /// <summary>
    /// Đếm số lượng object hiện tại đã spawn cho một prefab cụ thể.
    /// </summary>
    public int CountSpawnedObjects(GameObject prefab)
    {
        if (Application.isPlaying)
        {
            if (spawnedObjectsDict.ContainsKey(prefab))
            {
                return spawnedObjectsDict[prefab].Count;
            }
            return 0;
        }
        else
        {
#if UNITY_EDITOR
            // Trong Editor, đếm số lượng child dưới Parent GameObject
            GameObject parent = GetOrCreateParent(prefab.name);
            if (parent != null)
            {
                return parent.transform.childCount;
            }
#endif
            return 0;
        }
    }

    /// <summary>
    /// Spawn một object dựa trên cài đặt spawn.
    /// </summary>
    public void SpawnObject(SpawnSettings settings)
    {
        List<Terrain> includedTerrains = GetIncludedTerrains();
        if (includedTerrains == null || includedTerrains.Count == 0)
        {
            Debug.LogWarning("Không có Terrain nào được chọn để spawn.");
            return;
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;
        int maxAttempts = 100; // Số lần thử tìm vị trí hợp lệ
        int attempts = 0;

        Terrain selectedTerrain = null;

        do
        {
            selectedTerrain = includedTerrains[Random.Range(0, includedTerrains.Count)];
            spawnPosition = GetRandomPositionOnTerrain(selectedTerrain);

            if (settings.alignToTerrain)
            {
                // Nghiêng rotation theo normal của terrain
                Vector3 terrainNormal = GetTerrainNormal(selectedTerrain, spawnPosition);
                spawnRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
            }
            else
            {
                spawnRotation = Quaternion.identity;
            }

            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogWarning($"Không thể tìm thấy vị trí spawn hợp lệ cho prefab {settings.prefab.name}.");
                return;
            }

        } while (!IsValidSpawnPosition(settings, selectedTerrain, spawnPosition));

        GameObject parentObj = GetOrCreateParent(settings.prefab.name);

        GameObject newObj = Instantiate(settings.prefab, spawnPosition, spawnRotation, parentObj.transform);
        newObj.name = settings.prefab.name;

        if (Application.isPlaying)
        {
            if (spawnedObjectsDict.ContainsKey(settings.prefab))
            {
                spawnedObjectsDict[settings.prefab].Add(newObj);
            }
        }
    }

    /// <summary>
    /// Lấy danh sách các Terrain được chọn để spawn.
    /// </summary>
    List<Terrain> GetIncludedTerrains()
    {
        List<Terrain> included = new List<Terrain>();
        foreach (var selection in terrainSelections)
        {
            if (selection.include && selection.terrain != null)
            {
                included.Add(selection.terrain);
            }
        }
        return included;
    }

    /// <summary>
    /// Lấy một vị trí ngẫu nhiên trên Terrain.
    /// </summary>
    Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        float randomX = Random.Range(0f, terrainWidth);
        float randomZ = Random.Range(0f, terrainLength);

        // Tính toán vị trí thế giới thực sự bằng cách thêm vị trí của Terrain
        Vector3 worldPosition = new Vector3(randomX, 0f, randomZ) + terrain.transform.position;

        float terrainHeightAtPos = terrain.SampleHeight(worldPosition);

        Vector3 position = new Vector3(randomX, terrainHeightAtPos, randomZ) + terrain.transform.position;
        return position;
    }

    /// <summary>
    /// Lấy normal của Terrain tại vị trí spawn.
    /// </summary>
    Vector3 GetTerrainNormal(Terrain terrain, Vector3 position)
    {
        // Lấy vị trí chuẩn hóa trên terrain
        Vector3 terrainPos = position - terrain.transform.position;
        Vector3 normalizedPosition = new Vector3(
            terrainPos.x / terrain.terrainData.size.x,
            0,
            terrainPos.z / terrain.terrainData.size.z
        );

        return terrain.terrainData.GetInterpolatedNormal(normalizedPosition.x, normalizedPosition.z);
    }


    /// <summary>
    /// Kiểm tra xem vị trí spawn có hợp lệ không dựa trên các điều kiện đã thiết lập.
    /// Điều kiện mới: Kiểm tra độ nghiêng của Terrain tại vị trí spawn.
    /// </summary>
    bool IsValidSpawnPosition(SpawnSettings settings, Terrain terrain, Vector3 position)
    {
        // Kiểm tra chiều cao
        if (position.y < settings.minHeight || position.y > settings.maxHeight)
            return false;

        // Kiểm tra Texture của Terrain tại vị trí spawn
        int textureIndex = GetTerrainTextureIndex(terrain, position);
        if (textureIndex == -1)
            return false;

        string textureName = GetTextureName(terrain, textureIndex);
        if (!settings.allowedTerrainTextures.Contains(textureName))
            return false;

        // Kiểm tra độ nghiêng của Terrain tại vị trí spawn
        Vector3 terrainNormal = GetTerrainNormal(terrain, position);
        float slope = Vector3.Angle(Vector3.up, terrainNormal); // Tính độ nghiêng bằng độ

        if (slope < settings.minSlope || slope > settings.maxSlope)
            return false;

        // Kiểm tra collider chồng chéo
        Collider[] colliders = Physics.OverlapSphere(position, settings.spawnRadius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != terrain.gameObject && col.enabled)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Lấy chỉ số Texture của Terrain tại vị trí spawn.
    /// </summary>
    int GetTerrainTextureIndex(Terrain terrain, Vector3 position)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = position - terrain.transform.position;

        float relativeX = terrainPos.x / terrainData.size.x;
        float relativeZ = terrainPos.z / terrainData.size.z;

        int mapX = Mathf.RoundToInt(relativeX * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt(relativeZ * terrainData.alphamapHeight);

        mapX = Mathf.Clamp(mapX, 0, terrainData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrainData.alphamapHeight - 1);

        float[,,] alphaMap = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        float maxAlpha = 0f;
        int maxIndex = -1;

        for (int i = 0; i < terrainData.alphamapLayers; i++)
        {
            if (alphaMap[0, 0, i] > maxAlpha)
            {
                maxAlpha = alphaMap[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    /// <summary>
    /// Lấy tên Texture của Terrain dựa trên chỉ số Texture.
    /// </summary>
    string GetTextureName(Terrain terrain, int textureIndex)
    {
        if (textureIndex < 0 || textureIndex >= terrain.terrainData.terrainLayers.Length)
            return string.Empty;

        return terrain.terrainData.terrainLayers[textureIndex].name;
    }

    /// <summary>
    /// Tạo hoặc lấy GameObject cha để tổ chức các object đã spawn.
    /// </summary>
    public GameObject GetOrCreateParent(string prefabName)
    {
        string parentName = prefabName + "_Parent";
        GameObject parent = GameObject.Find(parentName);
        if (parent == null)
        {
            parent = new GameObject(parentName);
            parent.transform.parent = this.transform;
        }
        return parent;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Spawn các object trong Editor dựa trên cài đặt spawn.
    /// </summary>
    [ContextMenu("Spawn In Editor")]
    public void EditorSpawn()
    {
        GatherAllTerrainsAndTextures();

        if (spawnSettingsList == null || spawnSettingsList.Count == 0)
        {
            Debug.LogWarning("Chưa chỉ định bất kỳ cài đặt spawn nào.");
            return;
        }

        foreach (var settings in spawnSettingsList)
        {
            if (settings.prefab == null)
            {
                Debug.LogWarning("Một prefab trong danh sách spawn settings chưa được chỉ định.");
                continue;
            }

            // Nếu không có Terrain nào được chọn để bao gồm, bỏ qua spawn
            if (GetIncludedTerrains() == null || GetIncludedTerrains().Count == 0)
            {
                Debug.LogWarning($"Không có Terrain nào được chọn để spawn cho prefab {settings.prefab.name}.");
                continue;
            }

            // Xóa các object đã spawn trước đó
            ClearSpawnedObjectsByType(settings.prefab);

            // Spawn lại các object
            SpawnObjectsUntilLimit(settings);
        }

        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Xóa tất cả các object đã spawn khỏi scene.
    /// </summary>
    [ContextMenu("Clear Spawned Objects")]
    public void ClearSpawnedObjects()
    {
        foreach (var settings in spawnSettingsList)
        {
            if (settings.prefab == null)
                continue;

            GameObject parent = GetOrCreateParent(settings.prefab.name);
            if (parent != null)
            {
                // Sử dụng DestroyImmediate để xóa các đối tượng trong Editor
                while (parent.transform.childCount > 0)
                {
                    GameObject child = parent.transform.GetChild(0).gameObject;
                    DestroyImmediate(child);
                }
            }

            // Xóa danh sách trong spawnedObjectsDict nếu có
            if (spawnedObjectsDict.ContainsKey(settings.prefab))
            {
                spawnedObjectsDict[settings.prefab].Clear();
            }
        }
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Xóa các object đã spawn theo loại (theo prefab).
    /// </summary>
    /// <param name="prefab">Prefab của các object cần xoá.</param>
    public void ClearSpawnedObjectsByType(GameObject prefab)
    {
        if (prefab == null)
            return;

        GameObject parent = GetOrCreateParent(prefab.name);
        if (parent != null)
        {
            while (parent.transform.childCount > 0)
            {
                GameObject child = parent.transform.GetChild(0).gameObject;
                DestroyImmediate(child);
            }
        }

        // Xóa danh sách trong spawnedObjectsDict nếu có
        if (spawnedObjectsDict.ContainsKey(prefab))
        {
            spawnedObjectsDict[prefab].Clear();
        }

        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Tự động cập nhật danh sách Terrain và Texture khi có sự thay đổi.
    /// </summary>
    private void OnValidate()
    {
        GatherAllTerrainsAndTextures();
    }
#endif
}
