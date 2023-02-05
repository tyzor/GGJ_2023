using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GGJ.Player;

namespace GGJ.Enemies
{
    public class BombEnemy : EnemyBase
    {

        private enum EnemyState
        {
            Idle,
            Pursuit
        };

        private EnemyState enemyState;

        private NavMeshAgent agent;

        [SerializeField] private float moveSpeed = 5.0f;

        // TODO -- for debugging we should show this with gizmos
        // maybe this would work better with an aggro sphere collider when the player enters?
        [SerializeField] private float aggroRange = 20.0f;
        [SerializeField] private int attackDamage = 2;

        // Enemy will patrol around this point when idle -- defaults to spawn point
        private Vector3 guardPosition;
        [SerializeField] private float patrolRadius = 2.0f;
        [SerializeField] private float patrolChangeSeconds = 2.0f;
        private float patrolTimer = 0;

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
            _bounds = GetComponent<Collider>().bounds;
            _radius = _bounds.size.x;
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
                // Chase player -- if player escapes maybe go back to idle?
                agent.speed = this.moveSpeed;
                agent.destination = _player.position;

                // ALEX -- FIX? non alloc version
                // Check if we are close enough to attack player
                Collider[] collisions = Physics.OverlapSphere(transform.position, _radius, playerLayer.value);
                foreach (Collider col in collisions)
                    if (OnCollision(col))
                        return;

            }
        }

        bool OnCollision(Collider collider)
        {

            // TODO -- this is probably not the best way of checking
            //   maybe should be looking for a HealthBase type of component on the object
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("PLAYER HIT BY BOMB");
                PlayerHealth health = collider.gameObject.GetComponent<PlayerHealth>();
                health.DoDamage(this.attackDamage);
                Destroy(gameObject);
                return true;
            }

            return false;
        }

    }
}
