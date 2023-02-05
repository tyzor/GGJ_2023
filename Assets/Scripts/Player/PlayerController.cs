using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Inputs;
using GGJ.Interactables;
using UnityEngine;
using Input = GGJ.Inputs.Input;

namespace GGJ.Player
{
    public class PlayerController : MonoBehaviour, IInteractableListener
    {
        public static bool CanAttack { get; private set; } = true;
        private List<IInteractable> _currentInteractablesInRange;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            InputDelegator.OnAttackPressed += OnInteractPressed;
            InteractableBase.PlayerInteractableListener = this;
        }


        private void OnDisable()
        {
            InputDelegator.OnAttackPressed -= OnInteractPressed;
            InteractableBase.PlayerInteractableListener = null;
        }

        //IInteractableListener Implementation
        //============================================================================================================//
        public void OnEnterInteractRange(IInteractable interactable)
        {
            if (_currentInteractablesInRange == null)
                _currentInteractablesInRange = new List<IInteractable>();


            _currentInteractablesInRange.Add(interactable);
            CanAttack = false;
        }

        public void OnExitInteractRange(IInteractable interactable)
        {
            _currentInteractablesInRange.Remove(interactable);

            CanAttack = _currentInteractablesInRange.Count == 0;
        }

        //Callbacks
        //============================================================================================================//

        private void OnInteractPressed(bool isPressed)
        {
            if (isPressed == false)
                return;
                
            if (_currentInteractablesInRange == null || _currentInteractablesInRange.Count == 0)
                return;

            //Look for doors, and interact. Prevents dropping held items.
            //------------------------------------------------//
            var door = _currentInteractablesInRange
                .FirstOrDefault(x => x is DoorInteractable);

            if (door != null)
            {
                door.Interact();
                return;
            }
            //------------------------------------------------//

            var count = _currentInteractablesInRange.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                var interactable = _currentInteractablesInRange[i];
                interactable?.Interact();
            }
        }

        //============================================================================================================//
    }
}
