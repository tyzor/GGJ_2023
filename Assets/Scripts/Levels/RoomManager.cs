using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Interactables;
using GGJ.Player;
using GGJ.Prototype;
using GGJ.Utilities.Extensions;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Levels
{
    public class RoomManager : MonoBehaviour
    {
        public Room CurrentRoom { get; private set; }

        [SerializeField,Header("Rooms")]
        private Room rootRoomPrefab;
        [SerializeField]
        private Room[] roomPrefabs;

        //TODO These should be a collection
        [SerializeField, Header("Objects")] 
        private DoorInteractable doorInteractablePrefab;
        [SerializeField]
        private DoorInteractable exitDoorInteractablePrefab;
        [SerializeField]
        private FileInteractable fileInteractablePrefab;

        //------------------------------------------------//

        private Transform _playerTransform;
        
        private Room _currentRoom;
        

        //Unity Functions
        //============================================================================================================//
        private void OnEnable()
        {
            DoorInteractable.LoadNewRoom += OnLoadNewRoom;
        }



        private void Awake()
        {
            Assert.IsNotNull(rootRoomPrefab, $"Cannot start game without {nameof(rootRoomPrefab)} set");
            Assert.IsNotNull(roomPrefabs, $"Cannot start game without {nameof(roomPrefabs)} having values");
        }

        private void OnDisable()
        {
            DoorInteractable.LoadNewRoom -= OnLoadNewRoom;
        }
        //============================================================================================================//

        public Room GetRoom(int roomIndex)
        {
            if (roomIndex < 0)
                return rootRoomPrefab;
            
            return roomPrefabs[roomIndex];
        }

        public void SetRoom(int index, FolderRoom folderRoom)
        {
            if(_currentRoom != null)
                Destroy(_currentRoom.gameObject);

            _currentRoom = Instantiate(GetRoom(index));
            
            if (_playerTransform == null)
                _playerTransform = FindObjectOfType<PlayerController>().transform;
            
            _currentRoom.SetupRoom(_playerTransform,
                folderRoom, 
                exitDoorInteractablePrefab, 
                doorInteractablePrefab, 
                fileInteractablePrefab);
            
        }

        public FolderRoom GenerateDungeon(in DungeonProfile dungeonProfile)
        {
            return dungeonProfile.GenerateFolderStructure(roomPrefabs);
        }

        //Callbacks
        //============================================================================================================//
        
        private void OnLoadNewRoom(FolderRoom folderRoom)
        {
            SetRoom(folderRoom.RoomLayoutIndex, folderRoom);
        }

        //Unity Editor Functions
        //============================================================================================================//

        #region Unity Editor Functions

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (roomPrefabs == null)
                return;
            
            if (roomPrefabs.Contains(rootRoomPrefab))
                throw new Exception($"{nameof(rootRoomPrefab)} cannot be included as a {nameof(roomPrefabs)} option!!!");
        }
#endif

        #endregion //Unity Editor Functions

        //============================================================================================================//
    }
}