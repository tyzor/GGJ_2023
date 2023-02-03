using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : EnemyBase
{

    private enum AttackType { SpreadPattern, AtPlayer };
    [SerializeField]
    private AttackType attackType;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float attackCooldown = 2.0f; // Attack cooldown in seconds
    private float attackTimer;


    // Start is called before the first frame update
    void Start()
    {
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
                float angle = (Mathf.PI / 6) * (i+1);
                Vector2 direction = new Vector2(Mathf.Sin(angle),Mathf.Cos(angle)).normalized;
                GameObject bulletObj = Instantiate(bulletPrefab,transform.position, Quaternion.identity);
                bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction);
            }
        }

        // if atplayer
    }
}
