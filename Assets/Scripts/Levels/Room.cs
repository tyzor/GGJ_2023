using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GGJ.Levels
{
    public class Room : MonoBehaviour
    {
        public int MaxFolderCount => folderSpawnLocations.Length;
        public int RoomId => roomId;
        public bool CannotRepeat => cannotRepeat;
        

        [SerializeField]
        private string roomName;
        //[SerializeField, Min(0)]
        private int roomId;
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
            //TODO Setup folder connection data
        }
    }
}