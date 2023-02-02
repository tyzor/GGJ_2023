using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GGJ.Levels
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField]
        private Room rootRoom;
        [SerializeField]
        private Room[] roomPrefabs;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            Assert.IsNotNull(rootRoom, $"Cannot start game without {nameof(rootRoom)} set");
            Assert.IsNotNull(roomPrefabs, $"Cannot start game without {nameof(roomPrefabs)} having values");
        }

        private void Start()
        {
        
        }

        //============================================================================================================//

        public Room GetRoom(int roomIndex)
        {
            return roomPrefabs[roomIndex];
        }

        public DungeonProfile dungeonProfile;
        [ContextMenu("TestDungeonGeneration")]
        public void TestDungeonGeneration()
        {
            FindObjectOfType<ComputerFileSystemGenerator>().GenerateFolderStructure(dungeonProfile, roomPrefabs);
            //var data = dungeonProfile.GenerateDungeon(rootRoom, roomPrefabs);
            //Debug.Log(data);
        }

        //Unity Editor Functions
        //============================================================================================================//

        #region Unity Editor Functions

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (roomPrefabs == null)
                return;
            
            if (roomPrefabs.Contains(rootRoom))
                throw new Exception($"{nameof(rootRoom)} cannot be included as a {nameof(roomPrefabs)} option!!!");
        }
#endif

        #endregion //Unity Editor Functions

        //============================================================================================================//
    }
}