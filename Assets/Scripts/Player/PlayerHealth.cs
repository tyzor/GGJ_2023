using System;
using GGJ.Collectables;
using GGJ.Destructibles;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerHealth : HealthBase
    {
        public static event Action OnPlayerDied;

        public override void DoDamage(int damageAmount)
        {
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