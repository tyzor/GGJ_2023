using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Inputs
{
    public class InputDelegator : MonoBehaviour, GameInputs.IGameplayActions
    {
        public static event Action<bool> OnAttackPressed;
        public static event Action<(float x, float y)> OnMoveChanged;
        
        private float _currentX, _currentY;

        private void Start()
        {
            Input.GameInputs.Gameplay.SetCallbacks(this);
            Input.GameInputs.Gameplay.Enable();
        }

        public void OnHorizontalMovement(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;

            _currentX = context.ReadValue<float>();
            
            OnMoveChanged?.Invoke((_currentX, _currentY));
        }

        public void OnVerticalMovement(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;

            _currentY = context.ReadValue<float>();
            
            OnMoveChanged?.Invoke((_currentX, _currentY));
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;

            Debug.Log(context);
            var isPressed = context.ReadValueAsButton();
            OnAttackPressed?.Invoke(isPressed);
        }
    }
}
