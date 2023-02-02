using System;
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
        
        private float _startingHeight;
        private bool _isInteracting;

        private new Transform transform;

        //============================================================================================================//
        
        public void Init(string fileName)
        {
            throw new NotImplementedException();
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
            
            transform.position = PlayerTransform.position + Vector3.up * 1.5f;
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
                
                //Announce that a file was picked up for interested parties
                OnPickedUpFile?.Invoke(this);
            }
            else
            {
                IgnoreExits = false;
                var currentPos = transform.position;
                currentPos.y = _startingHeight;

                transform.position = currentPos;
                OnDroppedFile?.Invoke(this);
            }
        }
        //============================================================================================================//
    }
}