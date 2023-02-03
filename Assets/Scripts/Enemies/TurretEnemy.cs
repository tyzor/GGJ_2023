using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : EnemyBase
{

    private enum AttackType { SpreadPattern, CardinalPattern, AtPlayer };
    [SerializeField]
    private AttackType attackType;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float attackCooldown = 2.0f; // Attack cooldown in seconds
    private float attackTimer;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime;
        if(attackTimer < 0)
        {

            DoAttack();
            attackTimer = attackCooldown;

        }
    }

    private void DoAttack() {
        // Get attack type

        // if spread
        if(attackType == AttackType.SpreadPattern)
        {
            // spawn a fan of projectiles
            for(int i = 0; i<6; i++)
            {
                // Get direction
                float angle = (2.0f * Mathf.PI / 6.0f) * i + (Mathf.PI/6.0f);
                Vector2 direction = new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction, 4.0f);
            }
            
        }
        // up/down left/right pattern
        else if(attackType == AttackType.CardinalPattern)
        {
            // spawn in straight directions
            for(int i = 0; i<4; i++)
            {
                // Get direction
                float angle = i * (Mathf.PI/2.0f);
                Vector2 direction = new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction, 4.0f);
            }   
        }
        // Shooting directly at player
        else if(attackType == AttackType.AtPlayer)
        {
            
            // Get direction

            /* 
            // OLD DIRECT FIRE CODE
            Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.z - transform.position.z);
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction, 4.0f);
            */
         
            // NEW LEADING TARGET CODE
            Vector2 _player2 = new Vector2(_player.transform.position.x,_player.transform.position.z);
            float distance = Vector2.Distance(
                new Vector2(transform.position.x,transform.position.z),
                _player2
            );//distance in between in meters
            // TODO -- move bullet velocity to variable
            float travelTime = distance/4.0f;//time in seconds the shot would need to arrive at the target
            //Debug.Log(_player.GetComponent<Rigidbody>().velocity);
            Vector3 vel = _player.GetComponent<Rigidbody>().velocity;
            Vector2 vel2 = new Vector2(vel.x,vel.z);
            Vector3 aimPoint = _player2 + vel2*travelTime;
            Debug.Log("aimpoint:" + aimPoint);
            Debug.Log("playerpos:"+ _player2);
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, aimPoint-transform.position, 4.0f);
            
        }



    
    }
}
