using System;
using System.Collections.Generic;
using GGJ.Interactables;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Unity.AI.Navigation;

namespace GGJ.Levels
{
    public class Room : MonoBehaviour
    {
        public FolderRoom FolderRoomData { get; private set; }

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
        [SerializeField]
        private Transform[] fileSpawnLocations;

        //============================================================================================================//
        public void SetupRoom(
            in Transform playerTransform,
            FolderRoom folderRoom,
            DoorInteractable exitDoorInteractablePrefab, 
            DoorInteractable doorInteractablePrefab, 
            FileInteractable fileInteractablePrefab)
        {
            // Navmesh generation
            NavMeshSurface navMesh = GetComponent<NavMeshSurface>();
            navMesh.BuildNavMesh();

            gameObject.name = folderRoom.GetAbsolutePath();

            FolderRoomData = folderRoom;
            
            if (folderRoom.ParentFolder != null)
            {
                var exit = Instantiate(exitDoorInteractablePrefab, exitLocation.position, Quaternion.identity, transform);
                //FIXME I need a ref to the room!!
                exit.Init(folderRoom.ParentFolder);
            }

            //Folder Spawns
            //------------------------------------------------//
            var options = new List<Transform>(folderSpawnLocations);
            for (int i = 0; i < folderRoom.Subfolders.Length; i++)
            {
                var targetIndex = Random.Range(0, options.Count);
                var folderLocation = options[targetIndex];
                
                var door = Instantiate(doorInteractablePrefab, folderLocation.position, Quaternion.identity, transform);
                //FIXME I need a ref to the room!!
                door.Init(folderRoom.Subfolders[i]);

                options.RemoveAt(targetIndex);
            }

            //File Spawns
            //------------------------------------------------//
            if(folderRoom.Files != null && folderRoom.Files.Length > 0)
            {
                options = new List<Transform>(fileSpawnLocations);
                for (int i = 0; i < folderRoom.Files.Length; i++)
                {
                    var targetIndex = Random.Range(0, options.Count);
                    var fileLocation = options[targetIndex];

                    var file = Instantiate(fileInteractablePrefab, fileLocation.position, Quaternion.identity,
                        transform);
                    //FIXME I need a ref to the room!!
                    file.Init(folderRoom.Files[i]);

                    options.RemoveAt(targetIndex);
                }
            }
            //------------------------------------------------//

            playerTransform.position = playerSpawnLocation.position;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
        
        //Unity Editor Functions
        //============================================================================================================//
        
        #region Unity Editor Functions

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;
            
            if(exitLocation)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(exitLocation.position, 0.5f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerSpawnLocation.position, 0.5f);

            if(folderSpawnLocations != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var spawnLocation in folderSpawnLocations)
                {
                    Gizmos.DrawWireSphere(spawnLocation.position, 0.5f);
                }
            }

            if (fileSpawnLocations == null)
                return;
            
            Gizmos.color = Color.cyan;
            foreach (var spawnLocation in fileSpawnLocations)
            {
                Gizmos.DrawWireSphere(spawnLocation.position, 0.5f);
            }
        }
#endif

        #endregion //Unity Editor Functions

        //============================================================================================================//
    }
}