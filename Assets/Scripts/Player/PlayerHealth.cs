using System;
using GGJ.Collectables;
using GGJ.Destructibles;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerHealth : HealthBase
    {
        public static bool canTakeDamage {get; set;} = true;

        public static event Action OnPlayerDied;

        public override void DoDamage(int damageAmount)
        {
            if(!canTakeDamage)
                return;
            base.DoDamage(damageAmount);
            CollectableController.CreateCollectable(transform.position, damageAmount);
        }

        protected override void Kill()
        {
            Debug.Log("PLAYER IS DEAD!!!!!");
            OnPlayerDied?.Invoke();
        }

        public float currentHealthValue => (float)_currentHealth/startingHealth;

    }
}