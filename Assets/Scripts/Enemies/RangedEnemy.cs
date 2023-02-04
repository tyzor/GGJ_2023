using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GGJ.Player;
using GGJ.Projectiles;

namespace GGJ.Enemies
{
    public class RangedEnemy : EnemyBase
    {

        private enum EnemyState
        {
            Idle,
            Pursuit,
            Retreat
        };

        private EnemyState enemyState;

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
        [SerializeField] private float bulletSpeed = 10.0f;
        private float attackTimer;

        [SerializeField] private GameObject _turretHead;
        private Quaternion startRotation;

        [SerializeField] 
        private LayerMask playerLayer;


        // Caching size data for collision detection
        private Bounds _bounds;
        private float _radius;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            this.agent = GetComponent<NavMeshAgent>();
            this.guardPosition = transform.position;
            _bounds = GetComponent<MeshRenderer>().bounds;
            _radius = _bounds.extents.x;
        }

        // Update is called once per frame
        void Update()
        {
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

            } else if(this.enemyState == EnemyState.Retreat)
            {
                Debug.Log("Ranged enemy is retreating");
                Vector3 direction = transform.position - _player.position;
                float distance = direction.magnitude;
                if(distance > retreatRange)
                {
                    this.enemyState = EnemyState.Pursuit;
                } else {

                    direction.y = 0;
                    direction.Normalize();
                    agent.speed = this.moveSpeed; // TODO -- this should be a retreat speed
                    
                    // Try to find a good spot to retreat
                    NavMeshHit pt;
                    if(NavMesh.SamplePosition(transform.position + -direction*distance, out pt, 2.0f, 1))
                    {
                        agent.destination = pt.position;
                    } else {
                        agent.destination = transform.position;
                    }
                }
                
                

            }

            // Check if enemy should attack
            if(this.enemyState != EnemyState.Idle)
            {
                attackTimer -= Time.deltaTime;

                // ALEX -- how to have this compensate for the rotation of the mesh?
                Vector3 dirToPlayer = Vector3.ProjectOnPlane((_player.position - _turretHead.transform.position).normalized, Vector3.up);
                _turretHead.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up) * startRotation;

                if(attackTimer < 0)
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
                    if (travelTime < 0.2f)
                    {
                        aimPoint = _player2 + vel2 * travelTime;
                    }
                    else
                    {
                        aimPoint = _player2;
                    }

                    //Debug.Log("aimpoint:" + aimPoint);
                    //Debug.Log("playerpos:"+ _player2);
                    Bullet bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, aimPoint - _thisPos, this.bulletSpeed);

                    attackTimer = attackCooldown;
                }


            }

            
    

        }

    }
}
