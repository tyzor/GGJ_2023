using System;
using GGJ.Inputs;
using UnityEngine;

namespace GGJ.Player
{
    [Serializable]
    public struct AttackData
    {
        public string name;
        
        [Min(0)]public float chargeTimeMin;
        [Min(0)]public float chargeTimeMax;

        [Space(10f)]
        [Min(0)]
        public float attackRadius;
        [Min(0)]
        public float attackTime;
        [Min(0)]
        public float attackDamage;
    }
    
    public class PlayerAttackController : MonoBehaviour
    {
        public float Maxintensity = 10;
        
        [SerializeField]
        private AttackData[] attackInfo;

        private float _pressStartTime;

        private bool _isPressed;

        public ParticleSystem particles;
        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        private void Update()
        {
            //TODO Add timer to diminish RAM
        }

        //PlayerAttackController Functions
        //============================================================================================================//

        private void DoAttack(in AttackData attackData)
        {
            Debug.Log($"Did Attack {attackData.name}");
        }
        
        
        //Callbacks
        //============================================================================================================//
        
        private void OnAttackPressed(bool isPressed)
        {
            _isPressed = isPressed;

            if (isPressed)
            {
                PlayerMovementController.CanMove = false;
                _pressStartTime = Time.time;
                // intensity +=0.1f;
                // if (intensity >Maxintensity)
                // {
                //     intensity = Maxintensity;
                // }
                //Debug.Log(intensity);
            }
            else
            {
                PlayerMovementController.CanMove = true;
                var endTime =  Time.time - _pressStartTime;
                Debug.Log(endTime);

                //If we haven't hit the min threshold, then no need to bother
                if (endTime < attackInfo[0].chargeTimeMin)
                    return;

                for (int i = 0; i < attackInfo.Length; i++)
                {
                    var attackData = attackInfo[i];

                    if (endTime < attackData.chargeTimeMin || endTime > attackData.chargeTimeMax)
                        continue;
                    
                    DoAttack(attackData);
                    return;
                }

                //If we've gone through the list, it means we're beyond the max
                DoAttack(attackInfo[attackInfo.Length - 1]);

                /*switch (endTime)
                {
                    case float i when i > 0 && i< 0.5f:
                        Debug.Log(0);
                        break;
                    case float i when i > 0.5 && i< 1.0f:
                        Debug.Log(1);
                        break;
                    case float i when i > 1.0 && i< 1.5f:
                        Debug.Log(2);
                        break;
                    case float i when i > 1.5 && i< 2.0f:
                        Debug.Log(3);
                        break;
                }*/
                //intensity =0;
                //Determine how long we were pressing 
                //Do appropriate attack
            }

            
        }
    }
}