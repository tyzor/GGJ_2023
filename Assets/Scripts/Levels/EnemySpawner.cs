using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GGJ.Enemies;

namespace GGJ.Levels 
{
    public class EnemySpawner : MonoBehaviour
    {
        public enum EnemySpawnerType { Cluster, Spread };

        public float MaxSpawnRadius = 10.0f;
        public int MaxEnemies = 5;
        public int MinEnemies = 1;

        // List of valid spawn locations -- used to get good distribution
        private List<Vector3> spawnPoints;
        private int spawnPointIndex = 0; // tracker to indicate which spawn point to use next

        private EnemySpawnerType spawnerType = EnemySpawnerType.Cluster;

        public void InitSpawnPoints() {

            if(spawnPoints != null)
                return;

            // initialize our spawn point array
            if(spawnPoints == null)
            {
                spawnPoints = new List<Vector3>();
                
                // Divide area into grid, choose random point in each grid
                float size = MaxSpawnRadius * 2;
                if(size < 1.0f) 
                    size = 1.0f; // Need at least 1x1 box to put into

                int sideSize = (int)Mathf.Ceil(size);
                int squareNum = sideSize * sideSize;
                Vector2 center = new Vector2(transform.position.x,transform.position.z);
                for(int i=0;i<squareNum;i++)
                {
                    int x = i%sideSize - sideSize/2;
                    int y = i/sideSize - sideSize/2;
                    Vector2 newPoint = new Vector2(Random.Range(.2f,.8f)+x, Random.Range(.2f,.8f)+y);

                    //Vector3 debugPt = transform.position + new Vector3(newPoint.x, transform.position.y, newPoint.y);
                    //Debug.DrawLine(debugPt, debugPt + Vector3.up*50f, Color.yellow, 10.0f);

                    // See if there is a valid point on the NavMesh
                    Vector3 samplePos = new Vector3(transform.position.x + newPoint.x, transform.position.y, transform.position.z + newPoint.y);
                    NavMeshHit pt;
                    if(NavMesh.SamplePosition(samplePos, out pt, 2.0f, 1))
                    {
                        // check if inside our spawn area
                        if(Vector3.Distance(pt.position, transform.position) < MaxSpawnRadius)
                        {
                            // TODO -- maybe check if it is too close to another spawnpoint
                            spawnPoints.Add(pt.position);
                            //Debug.DrawLine(pt.position, pt.position + Vector3.up*50f, Color.yellow, 5.0f);
                        }
                    }
                }

                // Add the center of the spawn point as a backup
                if(spawnPoints.Count <= 0)
                    spawnPoints.Add(transform.position);

                // Order the points by distance from center
                OrderSpawnPoints();
                spawnPointIndex = 0;
            }
        }

        public void SetSpawnerType(EnemySpawnerType type)
        {
            this.spawnerType = type;
            OrderSpawnPoints(); // reorganize the points array
        }

        private void OrderSpawnPoints()
        {
            if(this.spawnerType == EnemySpawnerType.Cluster)
                spawnPoints.Sort((a,b)=>(int)(Vector3.Distance(a,transform.position) - Vector3.Distance(b,transform.position)));
            else if(this.spawnerType == EnemySpawnerType.Spread)
            {
                // TODO -- use a fisher-yates shuffle? or something more random
                var count = spawnPoints.Count;
                var last = count - 1;
                for (var i = 0; i < last; ++i) {
                    var r = UnityEngine.Random.Range(i, count);
                    var tmp = spawnPoints[i];
                    spawnPoints[i] = spawnPoints[r];
                    spawnPoints[r] = tmp;
                }
                
            }

        }

        public int GetRandomEnemyAmount()
        {
            return Random.Range(MinEnemies,MaxEnemies+1);
        }

        // Returns a spawn location give the current values
        // TODO -- is the "out" syntax usage valid here?
        public bool GetValidSpawnLocation(out Vector3 spawnPoint)
        {
            spawnPoint = spawnPoints[spawnPointIndex];
            spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Count;
            return true;
/*
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
        */
        }

    }

}