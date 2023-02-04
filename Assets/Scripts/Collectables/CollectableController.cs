using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Collectables
{
    public class CollectableController : MonoBehaviour
    {
        private static CollectableController _instance;
        
        [SerializeField]
        private CollectableBase collectablePrefab;

        [SerializeField, Min(0f)]
        private float launchSpeed;
        [SerializeField, Min(0f)]
        private float pickupDelay = 1f;

        [SerializeField, Space(10f)] 
        private CollectableBehaviourData collectableBehaviourData;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        //============================================================================================================//
        
        public static void CreateCollectable(Vector3 position, int count)
        {
            _instance.CreateCollectables(position, count, _instance.pickupDelay);
        }
        public static void CreateCollectable(Vector3 position, int count, float delay)
        {
            _instance.CreateCollectables(position, count, delay);
        }

        private void CreateCollectables(Vector3 position, int count, float delay)
        {
            for (int i = 0; i < count; i++)
            {
                var newCollectable = Instantiate(collectablePrefab, position, quaternion.identity, transform);

                var dir = Random.insideUnitCircle.normalized;
                
                newCollectable.Launch(collectableBehaviourData, new Vector3(dir.x, 0, dir.y), launchSpeed, delay);
            }
        }
        
        //Unity Editor
        //============================================================================================================//
        
#if UNITY_EDITOR
        
        [SerializeField, Min(1), Header("DEBUGGING")]
        private int spawnCount;
        [SerializeField]
        private Vector3 spawnPosition;
        
        [ContextMenu("Spawn Test Collectables")]
        private void SpawnCollectables()
        {
            if (Application.isPlaying == false)
                return;
            
            for (int i = 0; i < spawnCount; i++)
            {
                var newCollectable = Instantiate(collectablePrefab, spawnPosition, quaternion.identity, transform);

                var dir = Random.insideUnitCircle.normalized;
                
                newCollectable.Launch(collectableBehaviourData, new Vector3(dir.x, 0, dir.y), launchSpeed, pickupDelay);
            }
        }
#endif
        //============================================================================================================//
    }
}