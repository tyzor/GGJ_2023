using System.Collections;
using System.Collections.Generic;
using GGJ.Projectiles;
using UnityEngine;
using GGJ.Utilities;

namespace GGJ.Enemies
{
    public class TurretEnemy : EnemyBase
    {

        private enum AttackType
        {
            RandomPattern,
            SpreadPattern,
            CardinalPattern,
            AtPlayer
        };

        [SerializeField] private AttackType attackType;

        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float attackCooldown = 2.0f; // Attack cooldown in seconds
        [SerializeField] private float leadingShotBreakpoint = 0.8f; // Shots below this value don't calculating leading
        
        [SerializeField] private float bulletSpeed = 10.0f;
        [SerializeField] private int bulletDamage = 1;

        private float attackTimer;

        [SerializeField] private GameObject _turretHead;
        private Quaternion startRotation;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            attackTimer = attackCooldown;
            // Pick a random type
            if(attackType == AttackType.RandomPattern)
            {
                attackType = (AttackType)Random.Range(1, 4);
            }

            startRotation = _turretHead.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            attackTimer -= Time.deltaTime;

            Vector3 dirToPlayer = Vector3.ProjectOnPlane((_player.position - _turretHead.transform.position).normalized, Vector3.up);
            _turretHead.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up) * startRotation;

            if (attackTimer < 0)
            {

                DoAttack();
                attackTimer = attackCooldown;

            }
        }

        private void DoAttack()
        {
            // Get attack type

            // if spread
            if (attackType == AttackType.SpreadPattern)
            {
                // spawn a fan of projectiles
                for (int i = 0; i < 6; i++)
                {
                    // Get direction
                    float angle = (2.0f * Mathf.PI / 6.0f) * i + (Mathf.PI / 6.0f);
                    Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    ProjectileManager.CreateProjectile(gameObject, direction, this.bulletSpeed, this.bulletDamage);                    
                }

            }
            // up/down left/right pattern
            else if (attackType == AttackType.CardinalPattern)
            {
                // spawn in straight directions
                for (int i = 0; i < 4; i++)
                {
                    // Get direction
                    float angle = i * (Mathf.PI / 2.0f);
                    Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    ProjectileManager.CreateProjectile(gameObject, direction, this.bulletSpeed, this.bulletDamage);
                }
            }
            // Shooting directly at player
            else if (attackType == AttackType.AtPlayer)
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
                if (travelTime < this.leadingShotBreakpoint)
                {
                    aimPoint = _player2 + vel2 * travelTime;
                }
                else
                {
                    aimPoint = _player2;
                }

                //Debug.Log("aimpoint:" + aimPoint);
                //Debug.Log("playerpos:"+ _player2);
                ProjectileManager.CreateProjectile(gameObject, aimPoint - _thisPos, this.bulletSpeed, this.bulletDamage);

            }




        }
    }
}
