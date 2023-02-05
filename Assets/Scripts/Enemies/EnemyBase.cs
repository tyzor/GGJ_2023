using System;
using System.Collections;
using UnityEngine;
using GGJ.Destructibles;
using GGJ.Player;
using GGJ.Audio;

namespace GGJ.Enemies
{
    public class EnemyBase : HealthBase
    {
        public static event Action OnEnemyDied;
        protected static Transform _player;
        private Collider _hitCollider;

        private bool _HitCooldownActive;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (_player == null)
                _player = FindObjectOfType<PlayerHealth>()?.transform;
            if(_hitCollider == null)
            _hitCollider = GetComponent<Collider>();
        }

        protected override void Kill()
        {
            SFXController.PlaySound(SFX.ENEMY_DEATH);
            // TODO - Spawn RAM?
            Destroy(gameObject);
            OnEnemyDied?.Invoke();
        }

        public void StartHitCooldown(float time) 
        {
            if(!_HitCooldownActive)
            {
                _HitCooldownActive = true;
                StartCoroutine(EnemyHitTimer(time));
            }
        }

        IEnumerator EnemyHitTimer(float time)
        {
            _hitCollider.enabled = false;
            yield return new WaitForSeconds(time);
            _hitCollider.enabled = true;
            _HitCooldownActive = false;

        }

    }
}
