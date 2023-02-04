using System;
using System.Collections;
using UnityEngine;
using GGJ.Destructibles;
using GGJ.Player;

public class EnemyBase : HealthBase
{
    protected static Transform _player;
    private Collider _hitCollider;
    // Start is called before the first frame update
    public virtual void Start()
    {
        if(_player == null)
            _player = FindObjectOfType<PlayerHealth>().transform;

        if(_hitCollider == null)
            _hitCollider = GetComponent<Collider>();
    }


    public static event Action OnEnemyDied;
    protected override void Kill()
    {
        // TODO - Spawn RAM?
        Destroy(gameObject);

        OnEnemyDied?.Invoke();
    }

    // Enemy has been hit by an attack -- start an invuln timer
    public void StartHitCooldown(float time)
    {
        StartCoroutine(makeInvulnerable(time));
    }

    IEnumerator makeInvulnerable(float time)
    {
        _hitCollider.enabled = false;
        yield return new WaitForSeconds(time);
        _hitCollider.enabled = true;
    }
}
