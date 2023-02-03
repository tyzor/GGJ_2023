using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombEnemy : EnemyBase
{

    private enum EnemyState { Idle, Pursuit };
    private EnemyState enemyState;

    private NavMeshAgent agent;

    [SerializeField]
    private float moveSpeed = 5.0f;
    // TODO -- for debugging we should show this with gizmos
    // maybe this would work better with an aggro sphere collider when the player enters?
    [SerializeField]
    private float aggroRange = 20.0f;
    
    // Enemy will patrol around this point when idle -- defaults to spawn point
    private Vector3 guardPosition;
    [SerializeField]
    private float patrolRadius = 2.0f;
    [SerializeField]
    private float patrolChangeSeconds = 2.0f;
    private float patrolTimer = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.agent = GetComponent<NavMeshAgent>();
        this.guardPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.enemyState == EnemyState.Idle )
        {
            // Idle should "patrol" and check if a player is in radius
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if(distance < aggroRange)
            {
                this.enemyState = EnemyState.Pursuit;
            } else {

                // patrol our guard point
                // for now they will do a circle around the point
                agent.speed = this.moveSpeed / 2.0f;
                
                //if(agent.remainingDistance < 0.001f || agent.velocity.sqrMagnitude < 0.001f )
                // agent needs a new patrol target
                if(patrolTimer <= 0)
                {
                    Vector2 dir = Random.insideUnitCircle.normalized;
                    Vector3 dir3 = new Vector3(dir.x,0,dir.y);
                    Vector3 destination = this.guardPosition + dir3*this.patrolRadius;
                    agent.destination = destination;
                    patrolTimer = this.patrolChangeSeconds;
                } else {
                    patrolTimer -= Time.deltaTime;
                }
                

            }

        } else if(this.enemyState == EnemyState.Pursuit )
        {
            // Chase player -- if player escapes maybe go back to idle?
            agent.speed = this.moveSpeed; 
            agent.destination = this._player.transform.position;
        }
    }



}
