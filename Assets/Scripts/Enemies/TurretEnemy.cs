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
            if (attackType == AttackType.RandomPattern)
            {
                attackType = (AttackType)Random.Range(1, 4);
            }

            startRotation = _turretHead.transform.rotation;

            attackTimer = Random.Range(0f, attackCooldown);
        }

        // Update is called once per frame
        void Update()
        {
            if (shooting)
                return;
            
            attackTimer -= Time.deltaTime;

            Vector3 dirToPlayer = Vector3.ProjectOnPlane((_player.position - _turretHead.transform.position).normalized,
                Vector3.up);
            _turretHead.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up) * startRotation;

            if (attackTimer < 0)
            {
                StartCoroutine(ShootCoroutine(attackCooldown));
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
                ProjectileManager.CreateProjectile(gameObject, aimPoint - _thisPos, this.bulletSpeed,
                    this.bulletDamage);

            }




        }
        //============================================================================================================//

        private bool shooting;
        private IEnumerator ShootCoroutine(float cooldownTime)
        {
            shooting = true;
            attackTimer = cooldownTime;
            
            var halfCooldown = cooldownTime / 2f;

            var vfx = VFXManager.CreateVFX(VFX.SHOOT_CHARGE,
                _turretHead.transform.position/* + (transform.forward * 0.9f) + (Vector3.up * 0.5f)*/,
                _turretHead.transform);

            vfx.transform.localPosition += new Vector3(-0.5f,0.8f, 0f);
            var main = vfx.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = 1f;

            for (float t = 0; t < halfCooldown; t+=Time.deltaTime)
            {
                LookInDirection();
                yield return null;
            }
            
            DoAttack();

            Destroy(vfx);
            
            yield return new WaitForSeconds(halfCooldown);

            attackTimer = Random.Range(0f, cooldownTime);
            shooting = false;
        }
        
        private void LookInDirection()
        {
            Vector3 dirToPlayer = Vector3.ProjectOnPlane((_player.position - _turretHead.transform.position).normalized,
                Vector3.up);
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
        //============================================================================================================//

    }
}