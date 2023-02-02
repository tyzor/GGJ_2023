using System.Collections.Generic;
using System.Linq;
using GGJ.Inputs;
using GGJ.Interactables;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerController : MonoBehaviour, IInteractableListener
    {
        private List<IInteractable> _currentInteractablesInRange;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            InputDelegator.OnAttackPressed += OnInteractPressed;
        }


        private void OnDisable()
        {
            InputDelegator.OnAttackPressed -= OnInteractPressed;
        }

        //IInteractableListener Implementation
        //============================================================================================================//
        public void OnEnterInteractRange(IInteractable interactable)
        {
            if (_currentInteractablesInRange == null)
                _currentInteractablesInRange = new List<IInteractable>();


            _currentInteractablesInRange.Add(interactable);
        }

        public void OnExitInteractRange(IInteractable interactable)
        {
            _currentInteractablesInRange.Remove(interactable);
        }

        //Callbacks
        //============================================================================================================//

        private void OnInteractPressed(bool _)
        {
            if (_currentInteractablesInRange == null || _currentInteractablesInRange.Count == 0)
                return;

            //Look for doors, and interact. Prevent dropping holding items.
            //------------------------------------------------//
            var door = _currentInteractablesInRange
                .FirstOrDefault(x => x is DoorInteractable);

            if (door != null)
            {
                door.Interact();
                return;
            }
            //------------------------------------------------//

            //FIXME Need a way of making sure we don't drop the file if we're going through a door
            foreach (var interactable in _currentInteractablesInRange)
            {
                interactable?.Interact();
            }
        }

        //============================================================================================================//
    }
}
