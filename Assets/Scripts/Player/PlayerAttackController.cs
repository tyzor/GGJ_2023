using System;
using GGJ.Destructibles;
using GGJ.Enemies;
using GGJ.Inputs;
using GGJ.Projectiles;
using UnityEngine;
using GGJ.Utilities;
using GGJ.Audio;

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
        public float playerAttackCooldown; // amount of time for player to rest
        public bool canReflect;
        public bool hasImmunity; // Provides immunity to damage while using
        public bool isRushAttack;
        public float rushDistance;
        
    }
    
    public class PlayerAttackController : MonoBehaviour
    {
        // NEW STATE MACHINE -- cleaner way
        private enum STATE
        {
            IDLE,
            CHARGING_ATTACK,
            ATTACKING,
            ATTACKING_RUSH,
            COOLING_DOWN
        }
        private STATE _currentState;

        public bool IsAttacking => isAttacking;
        public bool IsCharging => _currentState == STATE.CHARGING_ATTACK;
        
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
        [SerializeField] private int RAMDrainTickDamage = 1;
        [SerializeField] private float chargeMoveMultiplier = 0.5f;
        
        private ParticleSystem _activeParticleSystem;

        // Prevent spamming of attack button
        private float attackCooldownTimer;

        //Unity Functions
        //============================================================================================================//
        
        

        private void OnEnable()
        {
            InputDelegator.OnAttackPressed += OnAttackPressed;
            InputDelegator.OnMoveChanged += OnMoveChanged;
        }

        private void Update()
        {
            //TryCleanParticles();

            switch(_currentState)
            {
                case STATE.IDLE:
                    
                    // If we can attack and are holding the button we transition to charge state
                    if(_isPressed && PlayerController.CanAttack)
                    {                        
                        _pressStartTime = Time.time;
                        RAMDrainTimer = RAMDrainInterval;
                        Globals.MoveMultiplier = this.chargeMoveMultiplier;
                
                        // Play charging animation
                        TryCleanParticles(true);
                        _activeParticleSystem = VFXManager.CreateVFX(VFX.SPIN_CHARGE, transform.position, transform)
                            .GetComponent<ParticleSystem>();

                        _currentState = STATE.CHARGING_ATTACK;
                    }

                    break;
                case STATE.CHARGING_ATTACK:

                    // Button was released
                    if(!_isPressed)
                    {
                        var endTime =  Time.time - _pressStartTime;

                        //If we haven't hit the min threshold, then no need to bother
                        if (endTime < attackInfo[0].chargeTimeMin)
                        {
                            Globals.MoveMultiplier = 1f;
                            _currentState = STATE.IDLE;  
                            return;                          
                        }

                        int attackIndex = 0;
                        for (int i = 0; i < attackInfo.Length; i++)
                        {
                            var attackData = attackInfo[i];
                            if (endTime < attackData.chargeTimeMin || endTime > attackData.chargeTimeMax)
                                continue;
                            
                            attackIndex = i;
                        }

                        //If we've gone through the list, it means we're beyond the max
                        attackIndex = Mathf.Clamp(attackIndex, 0, attackInfo.Length - 1);
                        DoAttack(attackInfo[attackIndex]);
                        Globals.MoveMultiplier = 1f;
                        _currentState = STATE.ATTACKING;
                        return;

                    }

                    // Move speed change
                    Globals.MoveMultiplier = this.chargeMoveMultiplier;

                    // RAM Drain
                    RAMDrainTimer -= Time.deltaTime;
                    if(RAMDrainTimer < 0)
                    {
                        GetComponent<PlayerHealth>().DoDamage(RAMDrainTickDamage,false);
                        RAMDrainTimer = RAMDrainInterval;
                    }

                    break;

                case STATE.ATTACKING:

                    // We are currently attacking
                    if (attackTimeLeft > 0)
                    {
                        PlayerMovementController.CanMove = false;
                        Collider[] collisions = Physics.OverlapSphere(transform.position, currentAttack.attackRadius);
                        foreach (Collider collider in collisions)
                            OnAttackCollision(collider, currentAttack);

                        ProjectileManager.ReflectAllProjectiles(transform.position, currentAttack.attackRadius, gameObject);

                        attackTimeLeft -= Time.deltaTime;

                        // Handling rush code
                        if(IsRushing)
                        {
                            // Move until we hit our rush point
                            Vector3 distance = rushPoint - transform.position;
                            Vector3 newPos = transform.position + transform.forward * (rushSpeed * Time.deltaTime);
                        
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
                        _activeParticleSystem?.Stop();
                        
                        // Here we set the cooldown before another attack can be made
                        attackCooldownTimer = currentAttack.playerAttackCooldown;

                        _currentState = STATE.COOLING_DOWN;
                        return;
                    }

                    break;

                case STATE.COOLING_DOWN:
                    if(attackCooldownTimer <= 0)
                    {
                        _currentState = STATE.IDLE;
                        return;
                    }
                    attackCooldownTimer -= Time.deltaTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            InputDelegator.OnAttackPressed -= OnAttackPressed;
            InputDelegator.OnMoveChanged -= OnMoveChanged;
        }

        //PlayerAttackController Functions
        //============================================================================================================//

        
        private void DoAttack(in AttackData attackData)
        {
            isAttacking = true;
            attackTimeLeft = attackData.attackTime;
            currentAttack = attackData;
            PlayerHealth.canTakeDamage = attackData.hasImmunity;
            IsRushing = attackData.isRushAttack && (inputData.sqrMagnitude > .001f);
            if(IsRushing)
            {
                SFXController.PlaySound(SFX.PLAYER_CHARGING);
                
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

                SFXController.PlaySound(SFX.PLAYER_ATTACK_CHARGED);

            } else {
                SFXController.PlaySound(SFX.PLAYER_ATTACK);
            }

            Debug.Log($"Did Attack {attackData.name}");

            //Create Particles
            //------------------------------------------------//
            TryCleanParticles(true);
            _activeParticleSystem = VFXManager.CreateVFX(VFX.SPIN_ATTACK, transform.position, _spinAttackAnchor)
                .GetComponent<ParticleSystem>();
            var scale = (attackData.attackRadius / attackInfo[0].attackRadius);
            _activeParticleSystem.transform.localScale = new Vector3(scale, 1, scale);
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

        //Particles
        //============================================================================================================//

        private void TryCleanParticles(bool forceClean = false)
        {

            if (forceClean && _activeParticleSystem)
            {
                Destroy(_activeParticleSystem.gameObject);
                return;
            }
            if (_activeParticleSystem == null || _activeParticleSystem.particleCount > 0)
                return;

            Destroy(_activeParticleSystem.gameObject);
        }
        
        //Callbacks
        //============================================================================================================//

        private void OnAttackPressed(bool isPressed)
        {

            
            _isPressed = isPressed;
            return;

            // OLD CODE
            /*

            //If the attack was never started, do not attempt to complete the attack
            if (_isPressed == false && isPressed == false)
                return;
                
            _isPressed = isPressed;
            
            if (isPressed)
            {
                TryCleanParticles(true);
                _activeParticleSystem = VFXManager.CreateVFX(VFX.SPIN_CHARGE, transform.position, transform)
                    .GetComponent<ParticleSystem>();
                //SFXController.PlaySound(SFX.PLAYER_CHARGING);
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
            */
            
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