using System.Collections;
using System.Collections.Generic;
using GGJ.Animations;
using UnityEngine;
using UnityEngine.AI;
using GGJ.Player;
using GGJ.Projectiles;
using GGJ.Utilities;

namespace GGJ.Enemies
{
    [RequireComponent(typeof(RangedEnemyAnimationController))]
    public class RangedEnemy : EnemyBase
    {

        public enum EnemyState
        {
            Idle,
            Pursuit,
            Retreat,
            cooldown
        };

        public EnemyState enemyState { get; private set; }

        private RangedEnemyAnimationController _rangedEnemyAnimationController;

        private NavMeshAgent agent;

        [SerializeField] private float moveSpeed = 5.0f;

        // TODO -- for debugging we should show this with gizmos
        // maybe this would work better with an aggro sphere collider when the player enters?
        [SerializeField] private float aggroRange = 20.0f;

        // Enemy will patrol around this point when idle -- defaults to spawn point
        private Vector3 guardPosition;
        [SerializeField] private float patrolRadius = 2.0f;
        [SerializeField] private float patrolChangeSeconds = 2.0f;
        private float patrolTimer = 0;

        [SerializeField] private float attackRange = 10.0f;
        
        [SerializeField] private float retreatRange = 5.0f;

        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float attackCooldown = 2.0f; // Attack cooldown in seconds
        [SerializeField] private float leadingShotBreakpoint = 0.8f; // Shots below this value don't calculating leading
        [SerializeField] private float bulletSpeed = 10.0f;
        [SerializeField] private int bulletDamage = 1;
        private float attackTimer;

        [SerializeField] private GameObject _turretHead;
        private Quaternion startRotation;

        [SerializeField] 
        private LayerMask playerLayer;


        // Caching size data for collision detection
        private Bounds _bounds;
        private float _radius;
        private float cooldownTimer;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            this.agent = GetComponent<NavMeshAgent>();
            _rangedEnemyAnimationController = GetComponent<RangedEnemyAnimationController>();
            this.guardPosition = transform.position;
            _bounds = GetComponent<Collider>().bounds;
            _radius = _bounds.extents.x;
            startRotation = _turretHead.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            //Force enemy to stop when firing
            //------------------------------------------------//
            if (enemyState == EnemyState.cooldown)
            {
                if (cooldownTimer > 0f)
                {
                    cooldownTimer -= Time.deltaTime;
                    return;
                }

                enemyState = EnemyState.Pursuit;
            }

            //------------------------------------------------//
            
            if (this.enemyState == EnemyState.Idle)
            {
                // Idle should "patrol" and check if a player is in radius
                float distance = Vector3.Distance(transform.position, _player.position);
                if (distance < aggroRange)
                {
                    this.attackTimer = this.attackCooldown;
                    this.enemyState = EnemyState.Pursuit;
                }
                else
                {

                    // patrol our guard point
                    // for now they will do a circle around the point
                    agent.speed = this.moveSpeed / 2.0f;

                    //if(agent.remainingDistance < 0.001f || agent.velocity.sqrMagnitude < 0.001f )
                    // agent needs a new patrol target
                    if (patrolTimer <= 0)
                    {
                        Vector2 dir = Random.insideUnitCircle.normalized;
                        Vector3 dir3 = new Vector3(dir.x, 0, dir.y);
                        Vector3 destination = this.guardPosition + dir3 * this.patrolRadius;
                        agent.destination = destination;
                        patrolTimer = this.patrolChangeSeconds;
                    }
                    else
                    {
                        patrolTimer -= Time.deltaTime;
                    }

                }

            }
            else if (this.enemyState == EnemyState.Pursuit)
            {
                // New idea -- pick a point in a radius around the player for the enemy to want to be at
                // This will bake the retreat mechanic into the turret
                
                Vector3 direction = transform.position - _player.position;
                float distance = direction.magnitude;
                Vector3 desiredPoint = _player.position + direction.normalized * this.retreatRange;
                
                // Try to find a good spot to retreat
                NavMeshHit pt;
                if(NavMesh.SamplePosition(desiredPoint, out pt, 2.0f, 1))
                {
                    agent.destination = pt.position;
                } else {
                    agent.destination = transform.position;
                }

                // Chase player -- if player escapes maybe go back to idle?
                agent.speed = this.moveSpeed; // TODO -- move this into chased speed variable?

            }

            // Check if enemy should attack
            if(this.enemyState != EnemyState.Idle)
            {
                attackTimer -= Time.deltaTime;

                LookInDirection(GetShootDirection());

                if(attackTimer < 0)
                {
                    StartCoroutine(ShootCoroutine(attackCooldown));
                }

            }

        }
        //============================================================================================================//

        private IEnumerator ShootCoroutine(float cooldownTime)
        {
            cooldownTimer = attackTimer = cooldownTime;
            agent.destination = transform.position;
            _rangedEnemyAnimationController.Play(ANIMATION.IDLE);
            enemyState = EnemyState.cooldown;
            
            var halfCooldown = cooldownTime / 2f;

            var vfx = VFXManager.CreateVFX(VFX.SHOOT_CHARGE,
                _turretHead.transform.position + (transform.forward * 0.5f) + (Vector3.up * 0.5f), 
                transform);

            for (float t = 0; t < halfCooldown; t+=Time.deltaTime)
            {
                transform.forward = (_player.position - _turretHead.transform.position).normalized;
                yield return null;
            }
            
            ProjectileManager.CreateProjectile(gameObject, GetShootDirection(), bulletSpeed, bulletDamage);

            Destroy(vfx);
            
            yield return new WaitForSeconds(halfCooldown);

            attackTimer = Random.Range(0f, cooldownTime);
        }

        private void LookInDirection(Vector3 direction)
        {
            // ALEX -- how to have this compensate for the rotation of the mesh?
            Vector3 dirToPlayer = Vector3.ProjectOnPlane(direction.normalized, Vector3.up);
            _turretHead.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up) * startRotation;
        }

        
        private Vector3 GetShootDirection()
        {
            // NEW LEADING TARGET CODE
            Vector2 _player2 = new Vector2(_player.position.x, _player.position.z);
            Vector2 _thisPos = new Vector2(transform.position.x, transform.position.z);
            float distance = Vector2.Distance(
                _thisPos,
                _player2
            ); //distance in between in meters
            // TODO -- move bullet velocity to variable
            float travelTime = distance / bulletSpeed; //time in seconds the shot would need to arrive at the target
            //Debug.Log(_player.GetComponent<Rigidbody>().velocity);
            Vector3 vel = _player.GetComponent<Rigidbody>().velocity;
            Vector2 vel2 = new Vector2(vel.x, vel.z);

            // Lets lead the target if the shot would be fast
            Vector2 aimPoint;
            if (travelTime < 0.8f)
            {
                aimPoint = _player2 + vel2 * travelTime;
            }
            else
            {
                aimPoint = _player2;
            }

            return aimPoint - _thisPos;
        }


    }
}