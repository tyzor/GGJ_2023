using System;
using GGJ.Destructibles;
using UnityEngine;

namespace GGJ.Player
{
    public class PlayerHealth : HealthBase
    {
        public static event Action OnPlayerDied;
        
        
        protected override void Kill()
        {
            Debug.Log("PLAYER IS DEAD!!!!!");
            OnPlayerDied?.Invoke();
        }
    }
}