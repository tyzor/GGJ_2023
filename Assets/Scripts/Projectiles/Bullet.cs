using System.Collections;
using System.Collections.Generic;
using GGJ.Destructibles;
using UnityEngine;
using GGJ.Player;
using GGJ.Utilities;
using GGJ.Enemies;

namespace GGJ.Projectiles
{
    public class Bullet : MonoBehaviour, ICanBeHit
    {
        [SerializeField] public float speed = 2.0f;

        private int damage = 1;

        private GameObject _owner; // the object that created this
        private Collider _ownerCollider;

        // Caching size data for collision detection
        private Bounds _bounds;
        private float _radius;

        public bool isFriendly = false;

        //Unity Functions
        //============================================================================================================//
            
        private void Start()
        {
            _bounds = GetComponent<MeshRenderer>().bounds;
            _radius = _bounds.extents.x;

            // Random direction at load
            /*
        Vector2 vec = Random.insideUnitCircle;
        direction = new Vector3(vec.x , 0, vec.y);
        transform.forward = direction;
        */
        }

        //Moves this GameObject 2 units a second in the forward direction
        private void Update()
        {
            // TODO -- add trail renderer!!!!

            //transform.Translate(this.direction * Time.deltaTime * speed);
            transform.Translate(transform.forward * (Time.deltaTime * speed), Space.World);
            Collider[] collisions = Physics.OverlapSphere(transform.position, _radius, Physics.AllLayers);
            foreach (Collider col in collisions)
                OnBulletCollision(col);
            /*if(collisions.Length > 0)
            speed = speed * -1;
            */

        }
        
        //============================================================================================================//

        private void OnBulletCollision(Collider collider)
        {
            // ignore collisions with whatever created the projectile
            // TODO -- this might interfere with reflected projectiles
            if (collider == _ownerCollider)
                return;

            var canBeHit = collider.GetComponent<ICanBeHit>();

            if (canBeHit is PlayerHealth playerHealth)
            {
                Debug.Log("PLAYER HIT");
                playerHealth.DoDamage(damage);
                ProjectileManager.DestroyBullet(this);

            } else if (isFriendly && canBeHit is EnemyBase enemyBase)
            {
                Debug.Log("PROJECTILE HIT ENEMY");
                enemyBase.DoDamage(damage);
                ProjectileManager.DestroyBullet(this);
            }
            else if (canBeHit is EnemyBase)
            {
                // DO NOTHING -- bullets pass through enemies
            } else 
            {
                // We'll assume wall or something unimportant for now
                // Kill projectile
                ProjectileManager.DestroyBullet(this);
            }

        }

        //============================================================================================================//
        public void LaunchBullet(GameObject owner, Vector2 dir, float speed = 2.0f, int damage = 1)
        {
            _owner = owner;
            _ownerCollider = owner.GetComponent<Collider>();
            this.speed = speed;
            this.damage = damage;
            transform.forward = new Vector3(dir.x, 0, dir.y);

            VFXManager.CreateVFX(VFX.SPIN_CHARGE, transform.position, transform);
        }

        public GameObject GetOwner()
        {
            return _owner;
        }

        public void ChangeOwner(GameObject owner)
        {
            _owner = owner;
            _ownerCollider = owner.GetComponent<Collider>();
        }

    }
}