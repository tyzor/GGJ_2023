using UnityEngine;

namespace GGJ.Destructibles
{
    public abstract class HealthBase : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int startingHealth;
        private int _currentHealth;

        //============================================================================================================//
        // Start is called before the first frame update
        private void Start()
        {
            _currentHealth = startingHealth;
        }
        //============================================================================================================//

        public void ResetHealth()
        {
            _currentHealth = startingHealth;
        }

        public virtual void DoDamage(int damageAmount)
        {
            _currentHealth -= Mathf.Abs(damageAmount);
        
            if(_currentHealth <= 0)
                Kill();
        }

        public virtual void AddHealth(int toAdd)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + toAdd, 0, startingHealth);
        }

        protected abstract void Kill();
        
        //============================================================================================================//
    }
}
