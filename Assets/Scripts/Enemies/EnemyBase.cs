using System;
using System.Collections;
using UnityEngine;
using GGJ.Destructibles;
using GGJ.Player;

namespace GGJ.Enemies
{
    public class EnemyBase : HealthBase
    {
        public static event Action OnEnemyDied;
        protected static Transform _player;
        private Collider _hitCollider;

        // Start is called before the first frame update
        public virtual void Start()
        {
            if (_player == null)
                _player = FindObjectOfType<PlayerHealth>().transform;
            if(_hitCollider == null)
            _hitCollider = GetComponent<Collider>();
        }

        protected override void Kill()
        {
            // TODO - Spawn RAM?
            Destroy(gameObject);

        OnEnemyDied?.Invoke();
    }
}
