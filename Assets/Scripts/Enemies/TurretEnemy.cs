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
            SpreadPattern,
            CardinalPattern,
            AtPlayer
        };

        [SerializeField] private AttackType attackType;

        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float attackCooldown = 2.0f; // Attack cooldown in seconds
        [SerializeField] private float bulletSpeed = 10.0f;
        private float attackTimer;

        [SerializeField] private GameObject _turretHead;
        private Quaternion startRotation;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            attackTimer = attackCooldown;
            attackType = (AttackType)Random.Range(0, 3);

            startRotation = _turretHead.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            attackTimer -= Time.deltaTime;

            Vector3 dirToPlayer = Vector3.ProjectOnPlane((_player.transform.position - _turretHead.transform.position).normalized, Vector3.up);
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
                    Bullet bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction, this.bulletSpeed);
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
                    Bullet bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    bulletObj.GetComponent<Bullet>().SpawnBullet(gameObject, direction, this.bulletSpeed);
                }
            }
            // Shooting directly at player
            else if (attackType == AttackType.AtPlayer)
            {

                // NEW LEADING TARGET CODE
                Vector2 _player2 = new Vector2(_player.transform.position.x, _player.transform.position.z);
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
                if (travelTime < 2.0f)
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


            }




        }
    }
}
