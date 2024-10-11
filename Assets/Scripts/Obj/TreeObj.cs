using UnityEngine;

public class TreeObj : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject logPrefab; // Prefab of the log object
    public int logQuantity = 1; // Number of logs to spawn each interaction
    public float spawnRadius = 1.0f; // Spawn radius for log objects
    public int maxSpawns = 3; // Maximum number of times logs can be spawned
    public string nameItem; // Name of the item in the ItemDatabase

    [Header("Effects Settings")]
    public ParticleSystem spawnEffect; // Effect when spawning logs
    public AudioClip spawnSound; // Sound when spawning logs
    public ParticleSystem destroyEffect; // Effect when destroying the tree
    public AudioClip destroySound; // Sound when destroying the tree
    public AudioClip axeSound; // Sound when interacting with the axe

    private int currentSpawns = 0; // Number of times logs have been spawned
    private bool isDestroyed = false; // Indicates if the tree has been destroyed
    private bool isProcessingCollision = false; // Flag to prevent multiple collision processing

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the interacting object is the Axe and the tree hasn't been destroyed
        if (collision.gameObject.CompareTag("Axe") && !isDestroyed)
        {
            // Prevent multiple processing of the same collision
            if (isProcessingCollision)
                return;

            isProcessingCollision = true; // Set the flag to indicate collision is being processed

            // Get the collision point
            Vector3 collisionPoint = collision.GetContact(0).point;

            // Play axe sound at the collision point
            if (axeSound != null)
            {
                AudioSource.PlayClipAtPoint(axeSound, collisionPoint);
            }

            if (currentSpawns < maxSpawns)
            {
                // Increment the spawn count
                currentSpawns++;

                // Spawn logs at the appropriate location
                SpawnLog();

                // If maximum spawns reached, destroy the tree
                if (currentSpawns >= maxSpawns)
                {
                    DestroyTree();
                }
            }

            // Reset the collision processing flag after a short delay
            StartCoroutine(ResetCollisionFlag());
        }
    }

    // Coroutine to reset the collision processing flag
    private System.Collections.IEnumerator ResetCollisionFlag()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame
        isProcessingCollision = false; // Reset the flag
    }

    private void SpawnLog()
    {
        if (logPrefab == null)
        {
            Debug.LogError("Log Prefab is not assigned.");
            return;
        }

        // Calculate a random spawn position around the tree
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = transform.position.y; // Keep the Y coordinate of the tree

        // Play spawn sound at the spawn position
        if (spawnSound != null)
        {
            AudioSource.PlayClipAtPoint(spawnSound, spawnPosition);
        }

        // Play spawn effect at the spawn position
        if (spawnEffect != null)
        {
            Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
        }

        // Instantiate the log at the spawn position
        GameObject log = Instantiate(logPrefab, spawnPosition, Quaternion.identity);

        // Add necessary components to the log
        MeshCollider meshCollider = log.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = log.AddComponent<MeshCollider>();
            meshCollider.convex = true;
        }

        Rigidbody rb = log.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = log.AddComponent<Rigidbody>();
        }

        ItemPickup pickup = log.GetComponent<ItemPickup>();
        if (pickup == null)
        {
            pickup = log.AddComponent<ItemPickup>();
        }

        // Set up ItemPickup information
        if (ItemDatabase.Instance != null)
        {
            ItemData itemData = ItemDatabase.Instance.GetItemDataByName(nameItem);
            if (itemData != null)
            {
                pickup.itemID = itemData.itemID.ToString();
                pickup.canPickup = true;
                pickup.quantity = logQuantity;
            }
            else
            {
                Debug.LogError($"Item '{nameItem}' not found in ItemDatabase.");
            }
        }
        else
        {
            Debug.LogError("ItemDatabase.Instance is null.");
        }
    }

    private void DestroyTree()
    {
        // Mark the tree as destroyed
        isDestroyed = true;

        // Play destroy effect at the tree's position
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Play destroy sound at the tree's position
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }

        // Destroy the tree game object
        Destroy(gameObject);
    }
}
