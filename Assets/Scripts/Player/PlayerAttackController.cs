using System;
using GGJ.Destructibles;
using GGJ.Enemies;
using GGJ.Inputs;
using GGJ.Projectiles;
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
        public float enemyHitCooldown;
        public bool canReflect;
    }
    
    public class PlayerAttackController : MonoBehaviour
    {
        public bool IsAttacking => isAttacking;
        public bool IsCharging => _isPressed;

        [SerializeField]
        private AttackData[] attackInfo;

        private float _pressStartTime;

        private bool _isPressed;

        // the time our player has left in the attack
        private float attackTimeLeft;
        // TODO -- use player state to track what they are doing (for bull rush)
        private bool isAttacking;
        private AttackData currentAttack;

        //FIXME This will need to separate to reduce follow issues
        [SerializeField] private Transform _spinAttackAnchor;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
        }

        private void Update()
        {
            //TODO Add timer to diminish RAM

            if (isAttacking == false && PlayerMovementController.CanMove == false)
                PlayerMovementController.CanMove = true;

            if (isAttacking == false)
                return;

            // We are currently attacking
            if (attackTimeLeft > 0)
            {
                PlayerMovementController.CanMove = false;
                Collider[] collisions = Physics.OverlapSphere(transform.position, currentAttack.attackRadius);
                foreach (Collider collider in collisions)
                    OnAttackCollision(collider, currentAttack);

                ProjectileManager.ReflectAllProjectiles(transform.position, currentAttack.attackRadius, this.gameObject);

                attackTimeLeft -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
            }
        }

        //PlayerAttackController Functions
        //============================================================================================================//

        private void DoAttack(int index, in AttackData attackData)
        {
            isAttacking = true;
            attackTimeLeft = attackData.attackTime;
            currentAttack = attackData;
            Debug.Log($"Did Attack {attackData.name}");   

            VFX vfxToPlay;
            switch (index)
            {
                case 0:
                    vfxToPlay = VFX.SPIN_ATTACK;
                    break;
                case 1:
                    vfxToPlay = VFX.SPIN_ATTACK;
                    break;
                case 2:
                    vfxToPlay = VFX.SPIN_ATTACK;
                    break;
                default:
                    throw new NotImplementedException();

            }
            var fxGameObject = VFXManager.CreateVFX(vfxToPlay, transform.position, _spinAttackAnchor);
        }
        
        private void OnAttackCollision(Collider collider, AttackData attackData)
        {
            var canBetHit = collider.GetComponent<ICanBeHit>();

            if (canBetHit == null)
                return;

            switch (canBetHit)
            {
                case EnemyBase enemyBase:
                    // TODO -- attack should only deal damage once?
                    Debug.Log($"Hit enemy {enemyBase.gameObject.name} - Damage {attackData.attackDamage}", enemyBase);
                    enemyBase.DoDamage(attackData.attackDamage);
                    enemyBase.StartHitCooldown(attackData.enemyHitCooldown);
                    break;
                case Bullet bullet:
                    Debug.Log("Hit bullet");
                    // TODO -- handle bullet deflection here
                    break;
                default:
                    return;
                
            }
        }
        
        //Callbacks
        //============================================================================================================//

        private void OnAttackPressed(bool isPressed)
        {
            //If the player is attempting to interact with an object, we will ignore the attack
            if (PlayerController.CanAttack == false && isPressed)
                return;

            //If the attack was never started, do not attempt to complete the attack
            if (_isPressed == false && isPressed == false)
                return;
                
            _isPressed = isPressed;
            
            if (isPressed)
            {
                PlayerMovementController.CanMove = false;
                _pressStartTime = Time.time;
            }
            else
            {
                PlayerMovementController.CanMove = true;
                var endTime =  Time.time - _pressStartTime;

                //If we haven't hit the min threshold, then no need to bother
                if (endTime < attackInfo[0].chargeTimeMin)
                    return;

                for (int i = 0; i < attackInfo.Length; i++)
                {
                    var attackData = attackInfo[i];

                    if (endTime < attackData.chargeTimeMin || endTime > attackData.chargeTimeMax)
                        continue;
                    
                    DoAttack(i, attackData);
                    return;
                }

                //If we've gone through the list, it means we're beyond the max
                var index = attackInfo.Length - 1;
                DoAttack(index, attackInfo[index]);
            }

            
        }

        //Unity Editor Functions
        //============================================================================================================//
        
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