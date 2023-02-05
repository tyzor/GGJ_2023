using System;
using GGJ.Levels;
using GGJ.Utilities;
using GGJ.Utilities.FolderGeneration;
using TMPro;
using UnityEngine;

namespace GGJ.Interactables
{
    public class FileInteractable : InteractableBase
    {
        //TODO Needs to return the file data
        public static event Action<FileInteractable> OnPickedUpFile;
        public static event Action<FileInteractable> OnDroppedFile;
        public static event Action<FileInteractable> OnRecycledFile;
        //Properties
        //============================================================================================================//
        

        //TODO Add File Data
        public File FileData { get; private set; }
        
        [SerializeField]
        private TMP_Text folderText;
        [SerializeField]
        private Transform textContainer;

        private float _startingHeight;
        private bool _isInteracting;

        private new Transform transform;

        //============================================================================================================//
        
        public void Init(File file)
        {
            transform = gameObject.transform;
            
            FileData = file;
            var fileName = file.GetFileNameExtension();
            gameObject.name = $"{fileName}_{nameof(FileInteractable)}";

            SetFileName(fileName);

            var fileTransform = FileModelLibrary.GetModel().transform;
            fileTransform.SetParent(transform, true);
            fileTransform.localPosition = Vector3.zero;
        }
        
        //Unity Functions
        //============================================================================================================//
        
        protected override void OnStart()
        {
            transform = gameObject.transform;
            _startingHeight = transform.position.y;
        }

        private void Update()
        {
            if (_isInteracting == false)
                return;
            
            transform.forward = Vector3.forward;
        }

        private void OnDestroy()
        {
            OnRecycledFile?.Invoke(this);
        }

        //============================================================================================================//
        public override void Interact()
        {
            _isInteracting = !_isInteracting;

            if (_isInteracting)
            {
                //We don't want the player to lose track of this object while in motion, so stop trying to exit
                IgnoreExits = true;
                //Move it above the players head
                transform.position = PlayerTransform.position + Vector3.up * 1.5f;
                transform.SetParent(PlayerTransform, true);
                
                //Announce that a file was picked up for interested parties
                OnPickedUpFile?.Invoke(this);
            }
            else
            {
                IgnoreExits = false;
                var currentPos = transform.position;
                currentPos.y = _startingHeight;

                transform.position = currentPos;
                transform.SetParent(RoomManager.CurrentRoom.transform, true);
                
                OnDroppedFile?.Invoke(this);
            }
        }
        //============================================================================================================//
        
        private void SetFileName(in string name)
        {
            folderText.text = name;
            folderText.ForceMeshUpdate();
            
            var xSize = (folderText.textBounds.size.x / 5f) * 1.25f;
            var scale = textContainer.localScale;
            scale.x = xSize;
            textContainer.localScale = scale;
        }
    }
}