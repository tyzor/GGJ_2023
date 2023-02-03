using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.AI.Navigation;

namespace GGJ.Levels
{
    public class Room : MonoBehaviour
    {
        public Vector3 PlayerSpawnPosition => playerSpawnLocation.position;
        public int MaxFolderCount => folderSpawnLocations.Length;
        public bool CannotRepeat => cannotRepeat;
        

        [SerializeField]
        private string roomName;
        [FormerlySerializedAs("canRepeat")] [SerializeField]
        private bool cannotRepeat;

        
        [SerializeField]
        private Transform playerSpawnLocation;
        [SerializeField]
        private Transform exitLocation;
        [SerializeField]
        private Transform[] folderSpawnLocations;
        public void SetupRoom(in RoomConnectionData roomConnectionData)
        {
            // Navmesh generation
            NavMeshSurface navMesh = GetComponent<NavMeshSurface>();
            navMesh.BuildNavMesh();

            //TODO Setup folder connection data
        }
    }
}