using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Interactables;
using GGJ.Player;
using GGJ.Utilities;
using GGJ.Utilities.Extensions;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Levels
{
    public class RoomManager : MonoBehaviour
    {
        //When a new room is instantiated
        public static event Action<int> OnNewRoomLoaded;
        //When a room that was already created, is re-loaded
        public static event Action OnRoomLoaded;
        //When we turn off a room object
        public static event Action OnRoomDisabled;

        //============================================================================================================//
        
        public static Room CurrentRoom { get; private set; }

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

        private Dictionary<int, Room> _dungeonRooms;
        private Transform _playerTransform;
        
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

        public void SetRoom(int roomLayoutIndex, FolderRoom folderRoom)
        {
            //If we're already in a room, disable the old one before opening a new one
            if (CurrentRoom != null)
            {
                CurrentRoom.SetActive(false);
                OnRoomDisabled?.Invoke();
            }

            //If the room was already loaded, just enable it
            if (_dungeonRooms.TryGetValue(folderRoom.FolderRoomListIndex, out var room))
            {
                room.SetActive(true);

                CurrentRoom = room;
                _playerTransform.position = room.PlayerSpawnPosition;
                OnRoomLoaded?.Invoke();
                return;
            }

            //If this is a new room, we'll instantiate it here
            CurrentRoom = Instantiate(GetRoom(roomLayoutIndex));
            
            if (_playerTransform == null)
                _playerTransform = FindObjectOfType<PlayerController>().transform;
            
            CurrentRoom.SetupRoom(_playerTransform,
                folderRoom, 
                exitDoorInteractablePrefab, 
                doorInteractablePrefab, 
                fileInteractablePrefab);
            
            _dungeonRooms.Add(folderRoom.FolderRoomListIndex, CurrentRoom);
            OnNewRoomLoaded?.Invoke(roomLayoutIndex);
        }

        public FolderRoom GenerateDungeon(in DungeonProfile dungeonProfile)
        {
            if (_dungeonRooms != null)
            {
                var roomObjects = _dungeonRooms.Values;
                foreach (var roomObject in roomObjects)
                {
                    Destroy(roomObject);
                }
                _dungeonRooms.Clear();
            }
            _dungeonRooms = new Dictionary<int, Room>(dungeonProfile.roomCount);
            return dungeonProfile.GenerateFolderStructure(roomPrefabs);
        }

        //Callbacks
        //============================================================================================================//
        
        private void OnLoadNewRoom(FolderRoom folderRoom)
        {
            RoomTransition.FadeScreen(0.75f, () =>
            {
                SetRoom(folderRoom.RoomLayoutIndex, folderRoom);
            });
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


        private Room _loadedRoom;
        public DungeonProfile dungeonProfile;
        [ContextMenu("TestDungeonGeneration")]
        public void TestDungeonGeneration()
        {
            // Despawn current room
            if(_loadedRoom)
            {
                DestroyImmediate(_loadedRoom.gameObject);
            }

            // Load in new room
            _loadedRoom = Instantiate(GetRoom(0), new Vector3(0, 0, 0), Quaternion.identity);
            _loadedRoom.SetupRoom(default,default,default,default,default);
            
            //var data = dungeonProfile.GenerateDungeon(rootRoom, roomPrefabs);
            //Debug.Log(data);
        }

#endif

        #endregion //Unity Editor Functions

        //============================================================================================================//
    }
}