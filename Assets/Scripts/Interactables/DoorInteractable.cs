using System;
using GGJ.Utilities.FolderGeneration;
using TMPro;
using UnityEngine;

namespace GGJ.Interactables
{
    public class DoorInteractable : InteractableBase
    {
        public static event Action<FolderRoom> LoadNewRoom;
        
        //TODO Determine if this is the right data to be storing
        private FolderRoom _targetRoom;

        [SerializeField]
        private TMP_Text folderText;
        [SerializeField]
        private Transform textContainer;

        //============================================================================================================//
        
        public void Init(FolderRoom folderRoom/*Room room*/)
        {
            gameObject.name = $"Door_To_[{folderRoom.FolderRoomListIndex}]{folderRoom.FolderName}";
            _targetRoom = folderRoom;

            SetFolderName(folderRoom.FolderName);
        }

        // InteractableBase Overrides
        //============================================================================================================//
        protected override void OnStart() { }

        public override void Interact() => LoadNewRoom?.Invoke(_targetRoom);
        
        //============================================================================================================//

        private void SetFolderName(in string name)
        {
            folderText.text = name;
            folderText.ForceMeshUpdate();
            
            var xSize = (folderText.textBounds.size.x / 10f) * 1.25f;
            var scale = textContainer.localScale;
            scale.x = xSize;
            textContainer.localScale = scale;
        }
    }
}