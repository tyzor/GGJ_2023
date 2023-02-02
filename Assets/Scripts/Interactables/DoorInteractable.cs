using GGJ.Levels;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Interactables
{
    public class DoorInteractable : InteractableBase
    {
        //TODO Determine if this is the right data to be storing
        [SerializeField]
        private Room _targetRoom;

        //============================================================================================================//
        
        public void Init(Room room)
        {
            _targetRoom = room;
        }

        // InteractableBase Overrides
        //============================================================================================================//
        protected override void OnStart()
        {
            
        }

        public override void Interact()
        {
            Debug.LogError("THIS IS A WARNING\nDoors are not fully implemented. Needs to be setup with Dungeon Systems.");
            
            Assert.IsNotNull(_targetRoom, $"Trying to interact with {gameObject.name}, but no room was assigned");
            
            //TODO Let RoomManager set _targetRoom as active room

            PlayerTransform.position = _targetRoom.PlayerSpawnPosition;
        }
        
        //============================================================================================================//
    }
}