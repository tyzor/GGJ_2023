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

        // the time our player has left in the attack
        private float attackTimeLeft;
        private AttackData currentAttack;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        private void Update()
        {
            //TODO Add timer to diminish RAM

            // We are currently attacking
            if(attackTimeLeft > 0 )
            {
                Collider[] collisions = Physics.OverlapSphere(transform.position, currentAttack.attackRadius);
                foreach(Collider collider in collisions)
                    OnAttackCollision(collider, currentAttack);

                attackTimeLeft -= Time.deltaTime;
            }

        }

        //PlayerAttackController Functions
        //============================================================================================================//

        private void DoAttack(in AttackData attackData)
        {
            attackTimeLeft = attackData.attackTime;
            currentAttack = attackData;
            Debug.Log($"Did Attack {attackData.name}");   
        }
        
        
        //Callbacks
        //============================================================================================================//
        
        private void OnAttackCollision(Collider collider, AttackData attackData)
        {
            
            EnemyBase enemy = collider.gameObject.GetComponent<EnemyBase>();
            if(enemy)
            {
                // TODO -- attack should only deal damage once?
                Debug.Log("Hit enemy");
                enemy.DoDamage((int)attackData.attackDamage);
            }

            Bullet bullet = collider.gameObject.GetComponent<Bullet>();
            if(bullet)
            {
                Debug.Log("Hit bullet");
                // TODO -- handle bullet deflection here
            }

        }

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

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
                return;

            if(attackTimeLeft > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, currentAttack.attackRadius);
            }
        }
#endif

    }

}