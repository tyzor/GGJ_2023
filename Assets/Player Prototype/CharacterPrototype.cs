using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Prototype
{
    public enum CameraType
    {
        FREE_LOOK,
        TOP_DOWN
    }
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterPrototype : MonoBehaviour, GameInputs.IGameplayActions
    {

        public CameraType cameraType;
        
        [SerializeField] private CinemachineFreeLook _freeLookCamera;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        
        
        [SerializeField, Min(0f)]
        private float moveSpeed;

        private bool hasInput => _currentXInput != 0 || _currentYInput != 0;
        
        private int _currentXInput, _currentYInput;
        private Vector3 _inputDir;

        private Transform _cameraTransform;
        private new Transform transform;
        private new Rigidbody rigidbody;

        // Start is called before the first frame update
        private void Start()
        {
            transform = gameObject.transform;
            rigidbody = GetComponent<Rigidbody>();
            
            _cameraTransform = Camera.main.transform;

            switch (cameraType)
            {
                case CameraType.FREE_LOOK:
                    _freeLookCamera.Priority = 100;
                    _virtualCamera.Priority = 0;
                    break;
                case CameraType.TOP_DOWN:
                    _freeLookCamera.Priority = 0;
                    _virtualCamera.Priority = 100;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Inputs.Input.GameInputs.Gameplay.SetCallbacks(this);
            Inputs.Input.GameInputs.Gameplay.Enable();
        }

        private void FixedUpdate()
        {
            var currentVelocity = rigidbody.velocity;

            if (hasInput == false)
            {
                currentVelocity.x = 0f;
                currentVelocity.z = 0f;
                rigidbody.velocity = currentVelocity;
                return;
            }
            
            var newVelocity = _inputDir * moveSpeed;
            newVelocity.y = currentVelocity.y;
            
            rigidbody.velocity = newVelocity;
        }

        // Update is called once per frame
        private void Update()
        {
            if (hasInput == false)
                return;
            
            var dir = new Vector3(_currentXInput, 0, _currentYInput).normalized;
            _inputDir = Vector3.ProjectOnPlane(_cameraTransform.TransformDirection(dir), Vector3.up).normalized;
            Debug.DrawRay(transform.position, _inputDir * 5, Color.blue);
            
            
            //var lookDir = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            
            transform.forward = _inputDir;
        }

        //============================================================================================================//
        
        public void OnHorizontalMovement(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;

            _currentXInput = Mathf.RoundToInt(context.ReadValue<float>());
        }

        public void OnVerticalMovement(InputAction.CallbackContext context)
        {
            if (context.performed == false)
                return;
            
            _currentYInput = Mathf.RoundToInt(context.ReadValue<float>());
        }
        
        //============================================================================================================//
    }
}
