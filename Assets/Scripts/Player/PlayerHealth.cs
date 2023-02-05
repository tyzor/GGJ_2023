using System;
using GGJ.Collectables;
using GGJ.Destructibles;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerHealth : HealthBase
    {
		public static event Action<float> OnPlayerHealthChanged;
        public static event Action OnPlayerDied;
		
        public static bool canTakeDamage {get; set;} = true;

        public override void DoDamage(int damageAmount, bool playVFX = true)
        {
            if(!canTakeDamage)
                return;
            base.DoDamage(damageAmount, playVFX);
            CollectableController.CreateCollectable(transform.position, damageAmount);
            
            OnPlayerHealthChanged?.Invoke((float)_currentHealth/startingHealth);
        }

        protected override void Kill()
        {
            Debug.Log("PLAYER IS DEAD!!!!!");
            OnPlayerDied?.Invoke();
        }
    }
}