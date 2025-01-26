using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private bool useRandomRotation = false;
    [SerializeField] private bool spawnAsChild = false;
    [SerializeField] private bool destroyAfterTime = false;
    [SerializeField] private float destroyDelay = 5f;

    public void SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to spawn!");
            return;
        }

        // Calculate spawn position
        Vector3 spawnPosition = transform.position + spawnOffset;
        
        // Calculate rotation
        Quaternion spawnRotation = useRandomRotation ? 
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : 
            Quaternion.identity;

        // Instantiate the prefab
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

        // Make it a child of this object if specified
        if (spawnAsChild)
        {
            spawnedObject.transform.SetParent(transform);
        }

        // Destroy after delay if specified
        if (destroyAfterTime)
        {
            Destroy(spawnedObject, destroyDelay);
        }
    }

    // Optional: Visualize the spawn point in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + spawnOffset, 0.2f);
    }
}