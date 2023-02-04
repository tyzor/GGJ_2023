using System.Collections;
using System.Collections.Generic;
using GGJ.Player;
using UnityEngine;

namespace GGJ.Collectables
{
    public class CollectableBase : MonoBehaviour
    {
        private static Transform _playerTransform;

        
        [SerializeField]
        private float pickupDistance;
        [SerializeField]
        private float drag;
        [SerializeField]
        private float accel;
        
        private Vector3 velocity;
        private bool canBePickedUp;
        private float pickupCountdown;

        private bool movingToPlayer;
        private float _moveSpeed;

        private bool launched;

        //Unity Functions
        //============================================================================================================//
        
        // Start is called before the first frame update
        private void Start()
        {
            if (_playerTransform == null)
                _playerTransform = FindObjectOfType<PlayerController>().transform;
        }

        //TODO Set this up as a state machine
        // Update is called once per frame
        private void Update()
        {
            if (launched == false)
                return;
            
            if (canBePickedUp == false)
            {
                if (pickupCountdown > 0f)
                {
                    pickupCountdown -= Time.deltaTime;
                }
                else
                    canBePickedUp = true;

                //Drag
                velocity -= velocity * (Time.deltaTime * drag);
                
                transform.position += velocity * Time.deltaTime;
                return;
            }

            var dirToPlayer = _playerTransform.position - transform.position;
            
            if (movingToPlayer)
            {
                _moveSpeed += Time.deltaTime * accel;
                var currentPosition = transform.position;

                currentPosition = Vector3.MoveTowards(
                    currentPosition, 
                    _playerTransform.position,
                    _moveSpeed * Time.deltaTime);
                
                transform.position += currentPosition;

                return;
            }

            //Distance check
            if (dirToPlayer.magnitude > pickupDistance)
            {
                velocity -= velocity * (Time.deltaTime * drag);
                transform.position += velocity * Time.deltaTime;
                return;
            }

            movingToPlayer = true;
        }

        //============================================================================================================//

        public void Launch(Vector3 direction, float speed, float pickupDelay = 1f)
        {
            velocity = direction.normalized * speed;

            pickupCountdown = pickupDelay;

            launched = true;
        }


        //============================================================================================================//

        public static void CreateCollectableAt(Vector3 worldPosition)
        {
            
        }
    }
}
