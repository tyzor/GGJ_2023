using System;
using UnityEngine;
using GGJ.Destructibles;

public class EnemyBase : HealthBase
{
    public GameObject _player {get; private set; } // reference to the current player -- good for targetting
    // Start is called before the first frame update
    public virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    public static event Action OnEnemyDied;
    protected override void Kill()
    {
        // TODO - Spawn RAM?
        Destroy(gameObject);

        OnEnemyDied?.Invoke();
    }
}
