using System;
using System.Collections.Generic;
using GGJ.Interactables;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

        //============================================================================================================//
        public void SetupRoom(
            in Transform playerTransform,
            FolderRoom folderRoom,
            DoorInteractable exitDoorInteractablePrefab, 
            DoorInteractable doorInteractablePrefab, 
            FileInteractable fileInteractablePrefab)
        {
            gameObject.name = folderRoom.GetAbsolutePath();
            if (folderRoom.ParentFolder != null)
            {
                var exit = Instantiate(exitDoorInteractablePrefab, exitLocation.position, Quaternion.identity, transform);
                //FIXME I need a ref to the room!!
                exit.Init(folderRoom.ParentFolder.FolderName);
            }

            var options = new List<Transform>(folderSpawnLocations);
            for (int i = 0; i < folderRoom.Subfolders.Length; i++)
            {
                var targetIndex = Random.Range(0, options.Count);
                var folderLocation = options[targetIndex];
                
                var door = Instantiate(doorInteractablePrefab, folderLocation.position, Quaternion.identity, transform);
                //FIXME I need a ref to the room!!
                door.Init(folderRoom.Subfolders[i].FolderName);

                options.RemoveAt(targetIndex);
            }


            //TODO Need to Add files
            //fileInteractablePrefab


            playerTransform.position = playerSpawnLocation.position;
        }
        
        
    }
}