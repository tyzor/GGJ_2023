using System;
using GGJ.Levels;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Interactables
{
    public class DoorInteractable : InteractableBase
    {
        public static event Action<string> LoadNewRoom;
        
        //TODO Determine if this is the right data to be storing
        private FolderRoom _targetRoom;

        private string TEMP_folderName;
        //============================================================================================================//
        
        public void Init(string folderName/*Room room*/)
        {
            TEMP_folderName = folderName;
        }

        // InteractableBase Overrides
        //============================================================================================================//
        protected override void OnStart()
        {

        }

        public override void Interact()
        {
            /*Debug.LogError("THIS IS A WARNING\nDoors are not fully implemented. Needs to be setup with Dungeon Systems.");
            
            Assert.IsNotNull(_targetRoom, $"Trying to interact with {gameObject.name}, but no room was assigned");*/
            
            //TODO Let RoomManager set _targetRoom as active room

            //PlayerTransform.position = _targetRoom.PlayerSpawnPosition;
            LoadNewRoom?.Invoke(TEMP_folderName);
        }
        
        //============================================================================================================//
    }
}