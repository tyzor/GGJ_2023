using System;
using GGJ.Levels;
using GGJ.Utilities.FolderGeneration;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Interactables
{
    public class DoorInteractable : InteractableBase
    {
        public static event Action<FolderRoom> LoadNewRoom;
        
        //TODO Determine if this is the right data to be storing
        private FolderRoom _targetRoom;

        //============================================================================================================//
        
        public void Init(FolderRoom folderRoom/*Room room*/)
        {
            _targetRoom = folderRoom;
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
            LoadNewRoom?.Invoke(_targetRoom);
        }
        
        //============================================================================================================//
    }
}