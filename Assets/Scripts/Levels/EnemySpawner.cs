using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GGJ.Enemies;

namespace GGJ.Levels 
{
    public class EnemySpawner : MonoBehaviour
    {
        public float MaxSpawnRadius = 20.0f;
        public int MaxEnemies = 5;
        public int MinEnemies = 1;

        public int GetRandomEnemyAmount()
        {
            return Random.Range(MinEnemies,MaxEnemies+1);
        }

        // Returns a spawn location give the current values
        // TODO -- is the "out" syntax usage valid here?
        public bool GetValidSpawnLocation(out Vector3 spawnPoint)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(0f,MaxSpawnRadius);
            Vector3 spawnPos = new Vector3(randomDir.x,0,randomDir.y) * distance + transform.position;
            NavMeshHit pt;
            if(NavMesh.SamplePosition(spawnPos, out pt, 2.0f, 1))
            {
                spawnPoint = pt.position;
                return true;
            }
        
            // Could not find a valid spawn location
            Debug.Log($"EnemySpawner failed to place enemy at {spawnPos}");
            spawnPoint = Vector3.zero;
            return false;
        }

    }

}