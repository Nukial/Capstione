using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject stonePrefab; // Prefab of the stone object
    public int stoneQuantity = 1; // Number of stones to spawn each interaction
    public float spawnRadius = 1.0f; // Spawn radius for stone objects
    public int maxSpawns = 3; // Maximum number of times stones can be spawned
    public string nameItem; // Name of the item in the ItemDatabase

    [Header("Effects Settings")]
    public ParticleSystem spawnEffect; // Effect when spawning stones
    public AudioClip spawnSound; // Sound when spawning stones
    public ParticleSystem destroyEffect; // Effect when destroying the rock
    public AudioClip destroySound; // Sound when destroying the rock
    public AudioClip pickaxeSound; // Sound when interacting with the pickaxe

    private int currentSpawns = 0; // Number of times stones have been spawned
    private bool isDestroyed = false; // Indicates if the rock has been destroyed
    private bool isProcessingCollision = false; // Flag to prevent multiple collision processing

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the interacting object is the Pickaxe and the rock hasn't been destroyed
        if (collision.gameObject.CompareTag("Pickaxe") && !isDestroyed)
        {
            // Prevent multiple processing of the same collision
            if (isProcessingCollision)
                return;

            isProcessingCollision = true; // Set the flag to indicate collision is being processed

            // Get the collision point
            Vector3 collisionPoint = collision.GetContact(0).point;

            // Play pickaxe sound at the collision point
            if (pickaxeSound != null)
            {
                AudioSource.PlayClipAtPoint(pickaxeSound, collisionPoint);
            }

            if (currentSpawns < maxSpawns)
            {
                // Increment the spawn count
                currentSpawns++;

                // Spawn stones at the appropriate location
                SpawnStone();

                // If maximum spawns reached, destroy the rock
                if (currentSpawns >= maxSpawns)
                {
                    DestroyRock();
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

    private void SpawnStone()
    {
        if (stonePrefab == null)
        {
            Debug.LogError("Stone Prefab is not assigned.");
            return;
        }

        // Calculate a random spawn position around the rock
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = transform.position.y; // Keep the Y coordinate of the rock

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

        // Instantiate the stone at the spawn position
        GameObject stone = Instantiate(stonePrefab, spawnPosition, Quaternion.identity);

        // Add necessary components to the stone
        MeshCollider meshCollider = stone.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = stone.AddComponent<MeshCollider>();
            meshCollider.convex = true;
        }

        Rigidbody rb = stone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = stone.AddComponent<Rigidbody>();
        }

        ItemPickup pickup = stone.GetComponent<ItemPickup>();
        if (pickup == null)
        {
            pickup = stone.AddComponent<ItemPickup>();
        }

        // Set up ItemPickup information
        if (ItemDatabase.Instance != null)
        {
            ItemData itemData = ItemDatabase.Instance.GetItemDataByName(nameItem);
            if (itemData != null)
            {
                pickup.itemID = itemData.itemID.ToString();
                pickup.canPickup = true;
                pickup.quantity = stoneQuantity;
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

    private void DestroyRock()
    {
        // Mark the rock as destroyed
        isDestroyed = true;

        // Play destroy effect at the rock's position
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Play destroy sound at the rock's position
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }

        // Destroy the rock game object
        Destroy(gameObject);
    }
}
