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

    // Update is called once per frame
    void Update()
    {

    }

    public static event Action OnEnemyDied;
    
    protected override void Kill()
    {
        OnEnemyDied?.Invoke();
    }
}
