using GGJ.Utilities;
using UnityEngine;

namespace GGJ.Destructibles
{
    public abstract class HealthBase : MonoBehaviour, ICanBeHit
    {
        [SerializeField, Min(1)]
        protected int startingHealth;
        protected int _currentHealth;

        //============================================================================================================//
        // Start is called before the first frame update
        protected virtual void Start()
        {
            _currentHealth = startingHealth;
        }
        //============================================================================================================//

        public void ResetHealth()
        {
            _currentHealth = startingHealth;
        }

        public virtual void DoDamage(int damageAmount, bool playVFX = true)
        {
            _currentHealth -= Mathf.Abs(damageAmount);
        
            if(_currentHealth <= 0)
                Kill();

            if(playVFX)
                VFXManager.CreateVFX(VFX.HIT_EFFECT, transform.position);
        }

        public virtual void AddHealth(int toAdd)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + toAdd, 0, startingHealth);
        }

        protected abstract void Kill();
        
        //============================================================================================================//
    }
}
