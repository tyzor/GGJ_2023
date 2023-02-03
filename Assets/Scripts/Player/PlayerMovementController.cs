using Cinemachine;
using GGJ.Inputs;
using UnityEngine;

namespace GGJ.Player
{
    [RequireComponent(typeof(Rigidbody))]

    public class PlayerMovementController : MonoBehaviour
    {
        public static bool CanMove { get; set; } = true;

        [SerializeField, Min(0f)]
        private float moveSpeed;

        private bool hasInput => _currentXInput != 0 || _currentYInput != 0;
        
        private float _currentXInput, _currentYInput;
        private Vector3 _inputDir;

        private Transform _cameraTransform;
        private new Transform transform;
        private new Rigidbody rigidbody;

        private void OnEnable()
        {
            InputDelegator.OnMoveChanged += OnMovementChanged;
        }



        // Start is called before the first frame update
        private void Start()
        {
            transform = gameObject.transform;
            rigidbody = GetComponent<Rigidbody>();
            
            _cameraTransform = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            var currentVelocity = rigidbody.velocity;

            if (hasInput == false || CanMove == false)
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
            
            transform.forward = _inputDir;
        }

        //============================================================================================================//
        private void OnMovementChanged((float x, float y) values)
        {
            _currentXInput = values.x;
            _currentYInput = values.y;
        }
        //============================================================================================================//
    }
}
