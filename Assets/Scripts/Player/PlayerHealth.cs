using System;
using GGJ.Destructibles;

namespace GGJ.Player
{
    public class PlayerHealth : HealthBase
    {
        public static event Action OnPlayerDied;
        
        
        protected override void Kill()
        {
            OnPlayerDied?.Invoke();
        }
    }
}