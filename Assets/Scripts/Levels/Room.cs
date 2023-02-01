using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Levels
{
    public class Room : MonoBehaviour
    {
        public int MaxFolderCount => folderSpawnLocations.Length;
        public int RoomId => roomId;
        public bool CanRepeat => canRepeat;
        

        [SerializeField]
        private string roomName;
        //[SerializeField, Min(0)]
        private int roomId;
        [SerializeField]
        private bool canRepeat;

        
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