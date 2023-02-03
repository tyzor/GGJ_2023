using System;
using GGJ.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ.Interactables
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        private const string PLAYER_TAG = "Player";

        private static IInteractableListener _playerInteractableListener;
        protected static Transform PlayerTransform;

        public Bounds ColliderBounds => _collider.bounds;
        private Collider _collider;
        private bool _playerInInteractRange;
        protected bool IgnoreExits { get; set; }

        //============================================================================================================//
        
        private void Start()
        {
            if (_playerInteractableListener == null)
            {
                var playerController = FindObjectOfType<PlayerController>();
                _playerInteractableListener = playerController;
                PlayerTransform = playerController.transform;
            }
            
            _collider = GetComponent<Collider>();
            Assert.IsTrue(_collider.isTrigger, $"{gameObject.name} Collider must be a trigger");
            
            OnStart();
        }

        private void OnTriggerEnter(Collider other)
        {
            //IF we're already in the player range, don't notify twice
            if (_playerInInteractRange)
                return;
            
            if (other.CompareTag(PLAYER_TAG) == false)
                return;

            _playerInInteractRange = true;
            _playerInteractableListener.OnEnterInteractRange(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IgnoreExits)
                return;
            if (other.CompareTag(PLAYER_TAG) == false)
                return;

            _playerInInteractRange = false;
            _playerInteractableListener.OnExitInteractRange(this);
        }

        private void OnDisable()
        {
            if (_playerInInteractRange == false)
                return;
            
            _playerInteractableListener.OnExitInteractRange(this);
        }
        
        //============================================================================================================//

        protected abstract void OnStart();
        
        //IInteractable Implementation
        //============================================================================================================//
        public abstract void Interact();
        
        //============================================================================================================//
    }
}