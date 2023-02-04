using System;
using UnityEngine;
using GGJ.Destructibles;
using GGJ.Player;

public class EnemyBase : HealthBase
{
    protected static Transform _player;
    // Start is called before the first frame update
    public virtual void Start()
    {
        if(_player == null)
            _player = FindObjectOfType<PlayerHealth>().transform;
    }


    public static event Action OnEnemyDied;
    protected override void Kill()
    {
        // TODO - Spawn RAM?
        Destroy(gameObject);

        OnEnemyDied?.Invoke();
    }
}
