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
        public bool hasImmunity; // Provides immunity to damage while using
        public bool isRushAttack;
        public float rushDistance;
        
    }
    
    public class PlayerAttackController : MonoBehaviour
    {
        public bool IsAttacking => isAttacking;
        public bool IsCharging => _isPressed;
        
        private Vector2 inputData;
        private bool IsRushing;

        [SerializeField]
        private AttackData[] attackInfo;

        private float _pressStartTime;

        private bool _isPressed;

        // the time our player has left in the attack
        private float attackTimeLeft;
        // TODO -- use player state to track what they are doing (for bull rush)
        private bool isAttacking;
        private AttackData currentAttack;
        private Vector3 rushPoint; // target endpoint for the rush attack
        private float rushSpeed;

        //FIXME This will need to separate to reduce follow issues
        [SerializeField] private Transform _spinAttackAnchor;
        [SerializeField] private float RAMDrainInterval = 1.0f;
        private float RAMDrainTimer;
        private int RAMDrainTickDamage = 1;

        //Unity Functions
        //============================================================================================================//
        
        private void Start()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
            InputDelegator.OnMoveChanged += OnMoveChanged;
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, transform.forward, Color.blue);
            
            // RAM Drain
            if(IsCharging)
            {
                RAMDrainTimer -= Time.deltaTime;
                if(RAMDrainTimer < 0)
                {
                    GetComponent<PlayerHealth>().DoDamage(RAMDrainTickDamage);
                    RAMDrainTimer = RAMDrainInterval;
                }
            }

            if (isAttacking == false) // && PlayerMovementController.CanMove == false)
            {
                //Debug.Break();
                PlayerMovementController.CanMove = true;
            }

            if (isAttacking == false)
                return;

            // We are currently attacking
            if (attackTimeLeft > 0)
            {
                PlayerMovementController.CanMove = false;
                Collider[] collisions = Physics.OverlapSphere(transform.position, currentAttack.attackRadius);
                foreach (Collider collider in collisions)
                    OnAttackCollision(collider, currentAttack);

                attackTimeLeft -= Time.deltaTime;

                // Handling rush code
                if(IsRushing)
                {
                    // Move until we hit our rush point
                    Vector3 distance = rushPoint - transform.position;
                    Vector3 newPos = transform.position + transform.forward * rushSpeed * Time.deltaTime;
                
                    float remainingDistanceSqr = (rushPoint-newPos).sqrMagnitude;
                    if(remainingDistanceSqr < distance.sqrMagnitude)
                        transform.position = newPos;

                }

            }
            else
            {
                // Attack is over restore player control
                isAttacking = false;
                IsRushing = false;
                PlayerHealth.canTakeDamage = true;
                PlayerMovementController.CanMove = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        //PlayerAttackController Functions
        //============================================================================================================//

        private void DoAttack(int index, in AttackData attackData)
        {
            isAttacking = true;
            attackTimeLeft = attackData.attackTime;
            currentAttack = attackData;
            PlayerHealth.canTakeDamage = attackData.hasImmunity;
            IsRushing = attackData.isRushAttack && (inputData.sqrMagnitude > .001f);
            if(IsRushing)
            {
                transform.forward = new Vector3(inputData.x, 0, inputData.y).normalized;
                rushPoint = transform.position + transform.forward.normalized * attackData.rushDistance;
                rushSpeed = attackData.rushDistance / attackData.attackTime;
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.forward, out hit, attackData.rushDistance))
                {
                    rushPoint = hit.point;
                }
                GetComponent<Rigidbody>().isKinematic = true;
                Debug.DrawLine(rushPoint + Vector3.up*100.0f,rushPoint, Color.yellow, 5.0f);
                
            }

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
                    Debug.Log($"Hit enemy {enemyBase.gameObject.name} - Damage {attackData.attackDamage}", enemyBase);
                    enemyBase.DoDamage(attackData.attackDamage);
                    enemyBase.StartHitCooldown(attackData.enemyHitCooldown);
                    break;
                case Bullet bullet:
                    // Bullet reflection is handled by ProjectileManager
                    
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
                RAMDrainTimer = RAMDrainInterval;
            }
            else
            {
                PlayerMovementController.CanMove = false;
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

        private void OnMoveChanged((float x, float y) values)
        {
            this.inputData = new Vector2(values.x, values.y);
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