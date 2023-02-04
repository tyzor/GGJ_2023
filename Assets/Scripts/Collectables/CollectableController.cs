using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Collectables
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField]
        private CollectableBase collectablePrefab;

        [SerializeField, Min(1)]
        private int spawnCount;
        [SerializeField]
        private Vector3 spawnPosition;

        [SerializeField, Min(0f)]
        private float launchSpeed;
        [SerializeField, Min(0f)]
        private float pickupDelay = 1f;

        [ContextMenu("Spawn Test Collectables")]
        private void SpawnCollectables()
        {
            if (Application.isPlaying == false)
                return;
            
            for (int i = 0; i < spawnCount; i++)
            {
                var newCollectable = Instantiate(collectablePrefab, spawnPosition, quaternion.identity, transform);

                var dir = Random.insideUnitCircle.normalized;
                
                newCollectable.Launch(new Vector3(dir.x, 0, dir.y), launchSpeed, pickupDelay);
            }
        }
    }
}