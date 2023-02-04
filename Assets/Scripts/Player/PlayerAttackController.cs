using System;
using GGJ.Inputs;
using UnityEngine;
using GGJ.Utilities;


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
        public int attackDamage;
        public float enemyHitTimer;
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
        // TODO -- use player state to track what they are doing (for bull rush)
        private bool isAttacking;
        private AttackData currentAttack;

        [SerializeField] private Animator _playerAnimator;
        
        [SerializeField] private Transform _spinAttackAnchor;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        // ALEX -- FIX ME
        private void Update()
        {
            //TODO Add timer to diminish RAM

            // We are currently attacking
            if(attackTimeLeft > 0 )
            {
                PlayerMovementController.CanMove = false;
                Collider[] collisions = Physics.OverlapSphere(transform.position, currentAttack.attackRadius);
                foreach(Collider collider in collisions)
                    OnAttackCollision(collider, currentAttack);

                attackTimeLeft -= Time.deltaTime;
            } else {
                if(isAttacking)
                {
                    PlayerMovementController.CanMove = true;
                    isAttacking = false;
                }
            }

            

        }

        //PlayerAttackController Functions
        //============================================================================================================//

        // ALEX -- FIX ME
        private void DoAttack(in AttackData attackData)
        {
            isAttacking = true;
            attackTimeLeft = attackData.attackTime;
            currentAttack = attackData;
            Debug.Log($"Did Attack {attackData.name}");   

            //animator.SetBool("Do Attack", true);
            _playerAnimator.Play("Spin_Attack");
            if(attackData.name == "Level 1")
            {
                GameObject fx = VFXManager.CreateVFX(VFX.SPIN_ATTACK, transform.position, _spinAttackAnchor);
            } else if(attackData.name == "Level 2")
            {
                GameObject fx = VFXManager.CreateVFX(VFX.SPIN_ATTACK2, transform.position, _spinAttackAnchor);
            } else if(attackData.name == "Level 3")
            {
                GameObject fx = VFXManager.CreateVFX(VFX.SPIN_ATTACK3, transform.position, _spinAttackAnchor);
            }
        }
        
        
        //Callbacks
        //============================================================================================================//
        
        private void OnAttackCollision(Collider collider, AttackData attackData)
        {
            var canBeHit = collider.GetComponent<ICanBeHit>();
            if (canBeHit == null)
                return;
            
            switch(canBeHit)
            {
                case EnemyBase enemyBase:
                    Debug.Log("Hit enemy");
                    enemy.DoDamage((int)attackData.attackDamage);
                    enemy.StartHitCooldown(.2f);
                    break;
                case Bullet bullet:
                    Debug.Log("Hit bullet");
                    // TODO -- handle bullet deflection here
                    break;
                default:
                    return;

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